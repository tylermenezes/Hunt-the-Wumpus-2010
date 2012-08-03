using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.PhysicsCore
{
    public enum ForceType
    {
        Gravity,
        Tension,
        Normal,
        Spring,
        Drag
    };
    public struct Force
    {
        public Vector2 con;
        public Vector2 f;
        public ForceType t;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        /// <param name="conPt">the contact point of the force, relative to the pre-rotated object. (so, conPt is POST-ROTATED!!!)</param>
        /// <param name="type"></param>
        public Force(Vector2 force, Vector2 conPt, ForceType type)
        {
            f = force;
            t = type;
            con = conPt;
        }
        public Force(Vector2 force, ForceType type)
            : this(force, Vector2.Zero, type)
        {
        }
    }

    public delegate Force ForceGenerator(RigidBody a);

    public delegate float TorqueGenerator(RigidBody a);

    public static class DefaultGenerators
    {
        public static float DRAG_COEFF = .15f; //.15
        public static float ANGDRAG_COEFF = 1f;//15f;

        public static ForceGenerator Gravity = new ForceGenerator(
                    delegate(RigidBody a)
                    {
                        if (a.InvMass != 0)
                            return new Force(a.Gravity * a.Mass, ForceType.Gravity);
                        return new Force(Vector2.Zero, ForceType.Gravity); //just ignore gravity if infinite mass
                    });

        //public static ForceGenerator Gravity = new ForceGenerator(GravityGenerator);
        /*public static Force GravityGenerator(PolyBody a)
        {
           return new Force(a.Attributes.Gravity, ForceType.Gravity);
        }*/

        public static ForceGenerator Drag = new ForceGenerator(
                    delegate(RigidBody a)
                    {
                        return new Force(-DRAG_COEFF * a.Velocity, ForceType.Drag);
                    });

        public static TorqueGenerator AngDrag = new TorqueGenerator(
            delegate(RigidBody a)
            {
                return -ANGDRAG_COEFF*a.AngularVelocity; 
            });

        static DefaultGenerators()
        {
            //static constructor. put default inits in here, if it gets too messy. YES MESSY IS OBJECTIVE NOW OKAY. ALSO 'IT' IS NOT AMBIGIGUOUS.
        }
    }

    public abstract class BinaryForceComponent //springs, etc.
    {
        protected RigidBody bodyA, bodyB;
        public RigidBody BodyA { get { return bodyA; } }
        public RigidBody BodyB { get { return bodyB; } }

        /// <summary>
        /// be CAREFULL with these! they are relative to the body's ORGIN, and BEFORE rotation!!
        /// </summary>
        public Vector2 ConA, ConB;

        public ForceGenerator ForceGenA;
        public ForceGenerator ForceGenB;

    }
    public class Spring : BinaryForceComponent
    {
        public float ks; //spring constant
        public float kd; //dampening constant
        public float rLen;

        public float GetLength()
        {
            var con1 = MathUtil.RotatePt(ConA, BodyA.Rotation);
            var con2 = MathUtil.RotatePt(ConB, BodyB.Rotation);

            var end1 = BodyA.Position + con1;
            var end2 = BodyB.Position + con2;

            return (end1 - end2).Length();
        }
        /// <summary>
        /// Creates a virtual force generator with spring properties
        /// </summary>
        /// <param name="a">body 1</param>
        /// <param name="b">body 2</param>
        /// <param name="restLen">the rest length of the spring</param>
        /// <param name="hConst">hooke's constant</param>
        /// <param name="dConst">dampening factor</param>
        public Spring(RigidBody a, RigidBody b, float restLen, float hConst, float dConst)
            : this(a, b, Vector2.Zero, Vector2.Zero, restLen, hConst, dConst)
        {
        }
        /// <summary>
        /// Creates a virtual force generator with spring properties
        /// </summary>
        /// <param name="a">body 1</param>
        /// <param name="b">body 2</param>
        /// <param name="conA">connection point #1 of spring relative to body1</param>
        /// <param name="conB">connection point #2 of spring relative to body2</param>
        /// <param name="restLen">the rest length of the spring</param>
        /// <param name="hConst">hooke's constant</param>
        /// <param name="dConst">dampening factor</param>
        public Spring(RigidBody a, RigidBody b, Vector2 conA, Vector2 conB, float restLen, float hConst, float dConst)
        {
            bodyA = a;
            bodyB = b;
            ks = hConst;
            kd = dConst;
            rLen = restLen;

            this.ConA = conA;
            this.ConB = conB;

            /*ForceGenA = new ForceGenerator(delegate(PolyBody body)
                {
                    Vector2 dx = (BodyA.State.Position - BodyB.State.Position);
                    Vector2 lNorm = Vector2.Normalize(dx);
                    float len = dx.Length();
                    Vector2 fs = -ks * (len - rLen) * lNorm; //spring force
                    Vector2 fd = -kd * Vector2.Dot((BodyA.State.Velocity - BodyB.State.Velocity), lNorm) * lNorm; //dampening force

                    return new Force(fs + fd, ForceType.Spring);
                });*/
            ForceGenA = new ForceGenerator(GetForceA);
            ForceGenB = new ForceGenerator(GetForceB);
        }

        Force GetForceA(RigidBody notreallyneeded)
        {
            var con1 = MathUtil.RotatePt(ConA, BodyA.Rotation);
            var con2 = MathUtil.RotatePt(ConB, BodyB.Rotation);

            var end1 = BodyA.Position + con1;
            var end2 = BodyB.Position + con2;

            var end1Vel = BodyA.Velocity + MathUtil.VelOfPoint(ConA, BodyA.AngularVelocity);
            var end2Vel = BodyB.Velocity + MathUtil.VelOfPoint(ConB, BodyB.AngularVelocity);


            Vector2 dx = (end1 - end2);
            Vector2 lNorm = Vector2.Normalize(dx);
            float len = dx.Length();
            Vector2 fs = -ks * (len - rLen) * lNorm; //spring force
            Vector2 fd = -kd * Vector2.Dot((end1Vel - end2Vel), lNorm) * lNorm; //dampening force


            return new Force(fs + fd, con1, ForceType.Spring);
        }
        Force GetForceB(RigidBody notreallyneeded)
        {
            var con2 = MathUtil.RotatePt(ConB, BodyB.Rotation);

            return new Force(-GetForceA(null).f, con2, ForceType.Spring);
        }
        public void SetLengthToCurrent()
        {
            this.rLen = GetLength();
        }
    }


    public class EMAttractor : BinaryForceComponent
    {
        public float ks; //em constant


        private float getClosingVelocity()
        {
            return (BodyA.Velocity - BodyB.Velocity).Length();
        }

        public float GetLength()
        {
            return (BodyA.Position - BodyB.Position).Length();
        }

        public EMAttractor(RigidBody a, RigidBody b, float hConst)
        {
            bodyA = a;
            bodyB = b;
            ks = hConst;

            ForceGenA = new ForceGenerator(GetForceA);
            ForceGenB = new ForceGenerator(GetForceB);
        }
        Force GetForceA(RigidBody notreallyneeded)
        {
            Vector2 dx = (BodyA.Position - BodyB.Position);
            Vector2 lNorm = Vector2.Normalize(dx);
            float invlenSq = 1 / dx.LengthSquared();
            Vector2 fs = -ks * (invlenSq) * lNorm; //spring force

            return new Force(fs, ForceType.Spring);
        }
        Force GetForceB(RigidBody notreallyneeded)
        {
            return new Force(-GetForceA(null).f, ForceType.Spring);
        }
    }

}
