using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.PhysicsCore
{
    public struct RigidBodyState
    {
        public bool IsFixed;
        public Vector2 Position;
        public Vector2 Velocity;
        public float Rotation;
        public float AngularVelocity;

        public RigidBodyState(Vector2 pos, Vector2 vel, float rot, float ang_v, bool isFixed)
        {
            Position = pos;
            Velocity = vel;
            Rotation = rot;
            AngularVelocity = ang_v;
            IsFixed = isFixed;
        }
    }
    public struct Attributes
    {
        public Vector2 Gravity;
        public float Friction;
        public float Mass;
        public float Time;
        public float Restitution;

        public Attributes(Vector2 grav, float mass, float friction, float timeStep, float restitution)
        {
            this.Gravity = grav;
            this.Mass = mass;
            this.Friction = friction;
            this.Time = timeStep;
            this.Restitution = restitution;
        }
    }

    public interface Primative
    {
        Vector2 Position { get; set; }
        float Rotation { get; set; }

        Projection Project(Vector2 a);
        Vector2 GetCentroid();
    }


    public interface Polygon : Primative
    {
        Vector2[] RawVertices { get; set; }
        /// <summary>
        ///  Vertices MUST BE defined clockwise! LEST YOU BREAK THE GAME. LOST THE GAME LOL.
        /// </summary>
        /// <returns></returns>
        Vector2[] GetTransformedVertices();
        Vector2[] GetRotatedVertices();
        Vector2[] GetNormals();
        BBox GetBoundingBoxRaw();
        BBox GetBoundingBoxTransformed();
    }

    public delegate void PhysicsEventHandler(RigidBody sender, object delta);

    public struct CollisionEventData
    {
        public Vector2 Axis;
        public float Disp;
        public CollisionEventData(Vector2 axis, float disp)
        {
            Axis = axis;
            Disp = disp;
        }
    }

    public static class SleepIDManager
    {
        private static int i = 0;
        public static int GetNewSleepID()
        {
            return i++;
        }
    }

    public abstract class RigidBody
    {
        public Texture2D tex;

        /// <summary>
        /// use this to store anything (ie, labels)
        /// </summary>
        public object Tag;

        protected float inertialTensor;
        protected bool godOrdainedInertia = false;


        protected Matrix rotMat = Matrix.Identity;
        protected Matrix transMat = Matrix.Identity;

        #region physics events
        // some useful events //
        public event PhysicsEventHandler VelocitySet;
        public event PhysicsEventHandler PositionSet;
        public event PhysicsEventHandler AngularVelocitySet;
        public event PhysicsEventHandler RotationSet;
        public event PhysicsEventHandler Collided;
        public event PhysicsEventHandler Deleted;

        bool alreadyActivated = false;

        int _pureEnergy = 2; //2=pure-active, 1=secondary-active, 0=passed
        public int PureEnergy
        {
            get
            {
                if (!CanSleep)
                    return 2;
                else
                    return _pureEnergy;
            }
        }

        public void OnCollided(RigidBody obj, Vector2 axis, float disp)
        {
            if (Collided != null) Collided(obj, new CollisionEventData(axis, disp));

            if (!obj.IsSleeping && (obj.PureEnergy==2 || obj.PureEnergy==1 || this.GetCurrActivity() > SleepEpsilon || obj.GetCurrActivity() > SleepEpsilon)) //if (!obj.IsSleeping && ((this.GetCurrActivity() > SleepEpsilon) || (disp < -.0001f)) && (this.sleepID != obj.sleepID))
            {
                this.OMGWAKEUP();
            }
            else if (sleeping)
            {
                this.HaltThisIsThePolice();
            }


            switch (this.PureEnergy)
            {
                case 2:
                    _pureEnergy = 1;
                    break;
                case 1:
                    if (obj.PureEnergy == 2) _pureEnergy = 2;
                    else _pureEnergy = 0;
                    break;
                case 0:
                    if (obj.PureEnergy == 1 && !alreadyActivated)
                        _pureEnergy = 1;
                    else if (obj.PureEnergy == 2) _pureEnergy = 1;
                    break;

            }
        }
        public void OnDeleted()
        {
            if (Deleted != null) Deleted(null, null);
        }

        public void OnPen(Vector2 dPos, float dRot)
        {
            dPosAcc += dPos.Length();
            if (dPosAcc > .3f)
            {
                this.OMGWAKEUP();
            }
        }
        #endregion physics events


        #region basics

        protected bool fixRot = false;
        protected bool fix = false;
        private bool pivot = false;
        protected float mass, rot, ang_v, time, friction, rest;
        protected Vector2 pos, vel, grav;

        /// <summary>
        /// net torques
        /// </summary>
        public float TNET;
        /// <summary>
        /// net forces
        /// </summary>
        public Vector2 FNET;
        public List<Force> currForces = new List<Force>(); //used for debug output
        public List<ForceGenerator> FGens = new List<ForceGenerator>();
        public List<TorqueGenerator> TGens = new List<TorqueGenerator>();


        public Vector2 Position
        {
            get { return pos; }
            set
            {
                if (!IsFixed && !IsPivot)
                {
                    var delta = value - pos;
                    pos = value;
                    transMat = Matrix.CreateTranslation(pos.X, pos.Y, 0);
                    if (PositionSet != null)
                        PositionSet(this, delta);
                }
            }
        }

        /// <summary>
        /// used for setting the position of a technically "fixed" object.
        /// </summary>
        public void SetMasterPosition(Vector2 newPos)
        {
            pos = newPos;
            transMat = Matrix.CreateTranslation(pos.X, pos.Y, 0);
        }

        /// <summary>
        /// used for setting the position of a technically "fixed rotation" object.
        /// </summary>
        public void SetMasterRotation(float newRot)
        {
            rot = newRot;
            rotMat = Matrix.CreateRotationZ(rot);
        }

        public bool IsPivot
        {
            get { return pivot; }
            set
            {
                pivot = value;
                if (pivot)
                {
                    IsFixed = false;
                    IsRotationFixed = false;
                    CanSleep = false;
                }
                else
                {
                    CanSleep = true;
                }
            }
        }

        /// <summary>
        /// counter clockwise
        /// </summary>
        public float Rotation
        {
            get { return rot; }
            set
            {
                rot = value;
                if (!fixRot)
                {
                    var delta = value - rot;
                    rot = value;
                    rotMat = Matrix.CreateRotationZ(rot);
                    if (RotationSet != null)
                        RotationSet(this, delta);
                }
            }
        }
        public virtual Vector2 Velocity
        {
            get { return vel; }
            set
            {
                var delta = value - vel;
                vel = value;
                if (VelocitySet != null)
                    VelocitySet(this, delta);
            }
        }

        public virtual float AngularVelocity
        {
            get { return ang_v; }
            set
            {
                var delta = value - ang_v;
                ang_v = value;
                if (AngularVelocitySet != null)
                    AngularVelocitySet(this, delta);
            }
        }

        public float Mass
        {
            get 
            {
                if (fix || pivot)
                {
                    return float.MaxValue;
                }
                else
                {
                    return mass;
                }
            }
            set
            {
                this.inertialTensor *= value / mass;
               
                mass = value;

                if (mass == float.MaxValue) fix = true;
                else fix = false;
            }
        }
        public virtual float InvMass
        {
            get
            {
                if (fix || pivot) return 0;
                else return 1 / mass;
            }
        }

        /*public virtual Vector2 RotationAxis
        {
            get { return this.Position; }
        }*/

        public virtual bool IsFixed
        {
            get { return fix; }
            set
            {
                fix = value;
                if (fix)
                {
                    this.GoToSleep();
                }
            }
        }
        public virtual bool IsRotationFixed
        {
            get { return fixRot; }
            set
            {
                fixRot = value;
            }
        }

        public virtual float Time
        {
            get { return time; }
            set { time = value; }
        }
        public virtual Vector2 Gravity
        {
            get { return grav; }
            set { grav = value; }
        }
        public virtual float Friction
        {
            get { return friction; }
            set { friction = value; }
        }
        public virtual float Restitution
        {
            get { return rest; }
            set { rest = value; }
        }



        public RigidBodyState GetState()
        {
            RigidBodyState s = new RigidBodyState();
            s.Position = this.Position;
            s.Rotation = this.Rotation;
            s.Velocity = this.Velocity;
            s.AngularVelocity = this.AngularVelocity;
            s.IsFixed = this.IsFixed;

            return s;
        }
        public Attributes GetAttributes()
        {
            Attributes a = new Attributes();
            a.Mass = this.Mass;
            a.Time = this.Time;
            a.Gravity = this.Gravity;
            a.Friction = this.Friction;

            return a;
        }

        public void SetState(RigidBodyState s)
        {
            this.SetMasterPosition(s.Position);
            this.SetMasterRotation(s.Rotation);
            this.Velocity = s.Velocity;
            this.AngularVelocity = s.AngularVelocity;
            this.IsFixed = s.IsFixed;
        }

        #endregion basics

        #region sleep

        public bool CanSleep = true;

        bool sleeping = false;
        public const float SleepEpsilon = 0.15f;
        private const float k = 2.5f;
        private float act = k * SleepEpsilon;
        public float ActivityBias = 0.98f; //how much past events are weighted
        float dPosAcc = 0f;

        public float timeSinceTouched = 0f;

        public float Activity
        {
            get { return act; }
        }
        public virtual void UpdateActivity(float dt)
        {
            act = act * ActivityBias + (1 - ActivityBias) * this.GetCurrActivity();
            if (act > k * SleepEpsilon) act = k * SleepEpsilon;

            if (timeSinceTouched > 1f) this.alreadyActivated = false;

            timeSinceTouched += dt;
        }
        public bool IsVegetated()
        {
            return timeSinceTouched > 5f;
        }
        public float GetCurrActivity()
        {
            return this.Velocity.LengthSquared() +this.AngularVelocity * this.AngularVelocity;
        }
        public bool IsSleepy()
        {
            return (this.Activity < SleepEpsilon);
        }
        public bool IsSleeping
        {
            get
            {
                return sleeping;
            }
        }
        private void HaltThisIsThePolice()
        {
            //DON'T MOVE!
            this.Velocity = Vector2.Zero;
            this.AngularVelocity = 0;
        }
        public void GoToSleep()
        {
            if (!sleeping && CanSleep)
            {
                HaltThisIsThePolice();
                sleeping = true;
                act = 0f;
                _pureEnergy = 0;
                alreadyActivated = false;
            }
        }
        public void OMGWAKEUP()
        {
            if (!IsFixed)
            {
                alreadyActivated = true;
                timeSinceTouched = 0f;

                dPosAcc = 0f;

                sleeping = false;
                act = k * SleepEpsilon + this.GetCurrActivity();
            }
        }


        #endregion sleep

        #region inertia
        public float GetInertiaMoment()
        {
            if (fixRot)
            {
                return float.MaxValue;
            }
            else
            {
                return this.inertialTensor;
            }
        }
        public float GetInverseInertiaMoment()
        {
            if (fixRot || fix)
            {
                return 0;
            }
            else
            {
                return 1 / GetInertiaMoment();
            }
        }
        public void MakeFixedRotation()
        {
            this.fixRot = true;
        }
        public void SetInertiaMoment(float I)
        {
            this.inertialTensor = I;
            godOrdainedInertia = true;
        }
        /// <summary>
        /// resets whatever the Inertia is set to, and recalcs to the actual inertia
        /// </summary>
        public void ClearGodOrdainedInertia()
        {
            godOrdainedInertia = false;
            this.UpdateInertiaMoment();
        }
        #endregion inertia

        #region PHYSICS

        //public List<ObjCollisionData> ColData = new List<ObjCollisionData>(); //used to store collision data per-object
        public List<ObjPenetrationData> PenData = new List<ObjPenetrationData>(); //used to store penetration data per-object

        public void AddForce(Force f)
        {
            this.FNET += f.f;
            this.currForces.Add(f);

            var tau = Vector3.Cross(f.con.ToVector3(), f.f.ToVector3());
            this.TNET += tau.Z;
        }
        public void AddTorque(float torque)
        {
            this.TNET += torque;
        }
        public void ResetForces()
        {
            this.FNET = Vector2.Zero;
            this.currForces.Clear();

            this.TNET = 0f;
        }
        public float GetMomentum()
        {
            return this.Mass * this.Velocity.Length();
        }
        public void ApplyImpulse(Vector2 impulse, Vector2 contact)
        {
            this.vel += impulse * this.InvMass;

            var impulsiveTorque = Vector3.Cross((contact - this.Position).ToVector3(), impulse.ToVector3());
            this.ang_v += impulsiveTorque.Z * this.GetInverseInertiaMoment();
        }
        #endregion PHYSICS

        #region abstracts
        public abstract float GetWidth();
        public abstract float GetHeight();
        public abstract void UpdateInertiaMoment();
        public abstract void UpdateCaches();

        public abstract Projection Project(Vector2 a);
        /// <summary>
        /// the bounding box of the transformed (pos+rot) object.
        /// </summary>
        /// <returns></returns>
        public abstract BBox GetBoundingBoxTransformed();
        /// <summary>
        /// the bounding box of the raw vertices of the object, as defined, before any transformations.
        /// </summary>
        /// <returns></returns>
        public abstract BBox GetBoundingBoxRaw();
        #endregion abstracts
    }

    public class PolyBody : RigidBody, Polygon
    {

        private Vector2[] normals;
        Vector2[] _rawVerts;

        /// <summary>
        /// whatever you do, make sure the orgin (0,0) is the centroid of the vertices. TODO: add normalization code that fixes this for any vertices
        /// </summary>
        public Vector2[] RawVertices
        {
            get
            {
                return _rawVerts;
            }
            set
            {
                _rawVerts = value;
                this.UpdateCaches();
            }
        }

        /// <summary>
        /// you whims. fine, this functions sets and positions the vertices for you. no need to worry about centering vertices about the orgin or anything. (this also sets the position of the object as the center of the vertices)
        /// </summary>
        /// <param name="verts"></param>
        public void ConstructFromVertices(Vector2[] verts)
        {
            if (verts.Length < 3) new Exception("verts must be >= 3.");

            var cent = verts.Aggregate((a, b) => a + b) / verts.Length;
            this.RawVertices = verts.Select(v => v - cent).ToArray();
            this.SetMasterPosition(cent);
        }



        public PolyBody(float mass, int verts)
        {
            if (verts < 3) new Exception("verts must be >= 3.");

            this.Mass = mass;
            this.Gravity = new Vector2(0, -9.8f);
            this.rest = 0.00f;
            this.friction = .15f;
            _rawVerts = new Vector2[verts];
        }

        public void MakeRect(float w, float h, Vector2 cent)
        {
            this._rawVerts = new Vector2[4];
            _rawVerts[0] = new Vector2(-w / 2, +h / 2);
            _rawVerts[1] = new Vector2(+w / 2, +h / 2);
            _rawVerts[2] = new Vector2(+w / 2, -h / 2);
            _rawVerts[3] = new Vector2(-w / 2, -h / 2);
            this.UpdateCaches();
            this.SetMasterPosition(cent);
        }
        public void MakeRect(float w, float h)
        {
            this._rawVerts = new Vector2[4];
            _rawVerts[0] = new Vector2(-w / 2, +h / 2);
            _rawVerts[1] = new Vector2(+w / 2, +h / 2);
            _rawVerts[2] = new Vector2(+w / 2, -h / 2);
            _rawVerts[3] = new Vector2(-w / 2, -h / 2);
            this.UpdateCaches();
            this.SetMasterPosition(Position);
        }
        public void MakeRegularFromSide(float s) //sidelen
        {
            float ang = (float)Math.PI * (RawVertices.Length - 2) / RawVertices.Length;
            float rad = (s / 2) / (float)Math.Cos(ang / 2);
            float a = (float)Math.PI - ang;
            float theta = -(float)Math.PI / 2 - a / 2;
            for (int i = 0; i < RawVertices.Length; i++)
            {
                float t = theta - a * i;
                _rawVerts[i] = new Vector2(rad * (float)Math.Cos(t), rad * (float)Math.Sin(t));
            }
            this.UpdateCaches();
        }
        public void MakeRegularFromRad(float rad) //radius
        {
            float ang = (float)Math.PI * (RawVertices.Length - 2) / RawVertices.Length;
            float a = (float)Math.PI - ang;
            float theta = -(float)Math.PI / 2 - a / 2;
            for (int i = 0; i < RawVertices.Length; i++)
            {
                float t = theta - a * i;
                _rawVerts[i] = new Vector2(rad * (float)Math.Cos(t), rad * (float)Math.Sin(t));
            }
            this.UpdateCaches();
        }

        public void ChangeWidth(float newWidth)
        {
            var scalingFactor = newWidth / GetWidth();
            ScaleWidth(scalingFactor);
        }
        public void ChangeHieght(float newHieght)
        {
            var scalingFactor = newHieght / GetHeight();
            ScaleHieght(scalingFactor);
        }


        public void ScaleWidth(float scalingFactor)
        {
            for (int i = 0; i < RawVertices.Length; i++)
            {
                _rawVerts[i].X *= scalingFactor;
            }
            this.UpdateCaches();
        }
        public void ScaleHieght(float scalingFactor)
        {
            for (int i = 0; i < RawVertices.Length; i++)
            {
                _rawVerts[i].Y *= scalingFactor;
            }
            this.UpdateCaches();
        }
        public Vector2[] GetTransformedVertices()
        {
            Matrix m = rotMat * transMat;
            Vector2[] tVert = new Vector2[RawVertices.Length];
            Vector2.Transform(RawVertices, ref m, tVert);
            return tVert;
        }
        public Vector2[] GetRotatedVertices()
        {
            Vector2[] tVert = new Vector2[RawVertices.Length];
            Vector2.Transform(RawVertices, ref rotMat, tVert);
            return tVert;
        }
        public override BBox GetBoundingBoxTransformed()
        {
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float minX = float.MaxValue;
            float minY = float.MaxValue;

            foreach (var v in this.GetTransformedVertices())
            {
                if (v.X < minX) minX = v.X;
                if (v.Y < minY) minY = v.Y;

                if (v.X > maxX) maxX = v.X;
                if (v.Y > maxY) maxY = v.Y;
            }
            return new BBox(minX, maxX, minY, maxY);
        }
        public override BBox GetBoundingBoxRaw()
        {
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float minX = float.MaxValue;
            float minY = float.MaxValue;

            foreach (var v in this.RawVertices)
            {
                if (v.X < minX) minX = v.X;
                if (v.Y < minY) minY = v.Y;

                if (v.X > maxX) maxX = v.X;
                if (v.Y > maxY) maxY = v.Y;
            }
            return new BBox(minX, maxX, minY, maxY);
        }
        public override Projection Project(Vector2 a)
        {
            Projection p = new Projection();
            p.Axis = a;
            p.Min = float.MaxValue;
            p.Max = float.MinValue;

            foreach (Vector2 v in this.GetTransformedVertices())
            {
                float d = Vector2.Dot(v, a);
                if (d < p.Min) p.Min = d;
                if (d > p.Max) p.Max = d;
            }
            return p;
        }
        public override void UpdateCaches()
        {
            UpdateInertiaMoment();
            UpdateNormals();
        }
        /// <summary>
        /// returns normals
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetNormals()
        {
            Vector2[] ax = new Vector2[this.normals.Length];
            Vector2.Transform(this.normals, ref rotMat, ax);
            return ax;
        }
        public Vector2[] UpdateNormals()
        {
            Vector2[] ax = new Vector2[RawVertices.Length];
            var verts = RawVertices;
            for (int i = 0; i < RawVertices.Length; i++)
            {
                var v1 = verts[i];
                var v2 = verts[(i + 1) % RawVertices.Length];
                //var side = v2 - v1;
                //var norm = new Vector2(side.Y, -side.X);
                var norm = new Vector2(v2.Y - v1.Y, v1.X - v2.X);
                ax[i] = Vector2.Normalize(norm);
            }

            this.normals = ax;
            return ax;
        }
        public Vector2 GetCentroid()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// only set this if you somehow screwed up the inertial tensoe. otherwise it gets recaled w/ every set to RawVerts
        /// </summary>
        public override void UpdateInertiaMoment()
        {
            //http://en.wikipedia.org/wiki/List_of_moments_of_inertia
            if (!godOrdainedInertia)
            {
                var rad = _rawVerts.Max(x => x.Length()); //again, this only works if RawVerts is centered on axis of rotation.
                this.inertialTensor = this.Mass * rad * rad / 2f;
            }
        }

        public override float GetWidth()
        {
            var bb = GetBoundingBoxTransformed();
            return bb.Width;
        }

        public override float GetHeight()
        {
            var bb = GetBoundingBoxTransformed();
            return bb.Height;
        }

    }


    public class Particle : PolyBody
    {
        public float rad = 1f;
        float ttl;
        float age=0f;

        public override void UpdateActivity(float dt)
        {
            age += dt;
            base.UpdateActivity(dt);
        }
        public bool IsOld()
        {
            return age > ttl;
        }
        public float Radius
        {
            get { return rad; }
            set 
            {
                rad = value;
                MakeRegularFromRad(rad);
            }
        }
        public override void UpdateInertiaMoment()
        {
            this.inertialTensor = 2f / 5f * rad * rad * mass;
        }
        public Particle(float mass, float rad, float secsToLive)
            : base(mass, 3)
        {
            ttl = secsToLive;

            this.CanSleep = false;
            //this.IsRotationFixed = true;
            this.Radius = rad;
            this.Mass = mass;

            this.Gravity = new Vector2(0, -9.8f);
            this.rest = 0.00f;
            this.friction = .2f;

            //UpdateInertiaMoment();
        }
    }

}

