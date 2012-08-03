using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.PhysicsCore
{


    public struct Projection
    {
        public float Min, Max;
        public Vector2 Axis;

        /// <summary>
        /// calculates the separation distance between projection intervals. will be negative if they overlap.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float IntervalDist(Projection a, Projection b)
        {
            if (a.Axis != b.Axis) throw new Exception("oh shit, someone divided by zero.\r\n I SWEAR IT WASN'T ME.");
            if (a.Min < b.Min) return b.Min - a.Max;
            return a.Min - b.Max;
        }
    }

    public struct BBox
    {
        public float xmin, xmax, ymin, ymax;
        public BBox(float XMin, float XMax, float YMin, float YMax)
        {
            xmin = XMin;
            xmax = XMax;
            ymin = YMin;
            ymax = YMax;
        }
        public float Width
        {
            get { return xmax - xmin; }
        }
        public float Height
        {
            get { return ymax - ymin; }
        }
        public Rectangle ToRectangle()
        {
            return new Rectangle((int)xmin, (int)ymax, (int)this.Width, (int)this.Height);
        }
        public static bool Intersects(BBox a, BBox b)
        {
            return Rectangle.Empty == Rectangle.Intersect(a.ToRectangle(), b.ToRectangle());
        }
    }

    public struct Contact
    {
        public RigidBody A, B;
        public Vector2 ConPoint;
        public float Disp;
        public Vector2 Axis;
        public Contact(RigidBody a, RigidBody b, Vector2 con, Vector2 axis, float disp)
        {
            A = a;
            B = b;
            ConPoint = con;
            Disp = disp;
            Axis = axis;
        }
    }

    public struct ContactP
    {
        public RigidBody A, B;
        public List<Vector2> ConPoints;
        public float Disp;
        public Vector2 Axis;
        public ContactP(RigidBody a, RigidBody b, Vector2 axis, float disp)
        {
            A = a;
            B = b;
            ConPoints = new List<Vector2>();
            Disp = disp;
            Axis = axis;
        }
    }

    public struct ObjCollisionData
    {
        public Vector2 dPos;
        public Vector2 dVel;
        public float dAngVel;

        public static ObjCollisionData operator +(ObjCollisionData a, ObjCollisionData b)
        {
            ObjCollisionData ret = a;
            ret.dPos += b.dPos;
            ret.dVel += b.dVel;
            ret.dAngVel += b.dAngVel;
            return ret;
        }
        public static ObjCollisionData operator /(ObjCollisionData a, float b)
        {
            ObjCollisionData ret = a;
            ret.dPos /= b;
            ret.dVel /= b;
            ret.dAngVel /= b;
            return ret;
        }
    }

    public struct ObjPenetrationData
    {
        public Vector2 dPos;
        public float dRot;
        public ObjPenetrationData(Vector2 DPOS, float DROT)
        {
            dPos = DPOS;
            dRot = DROT;
        }
    }

    public static class CollisionEngine
    {
        public static float SOFTNESS = .8f; // from (0, 1] = represents how much interpenetration is tolerated

        /// <summary>
        /// bounding box test, for polys
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool BBTest(Polygon a, Polygon b)
        {
            throw new NotImplementedException();
        }
        public static bool TestCollisionBody(RigidBody a, RigidBody b, out Vector2 axis, out float disp)
        {
            axis = new Vector2();
            disp = float.MinValue;

            if (a is PolyBody && b is PolyBody)
            {
                return TestCollisionPoly((PolyBody)a, (PolyBody)b, out axis, out disp);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static bool TestCollisionSpiritBody(RigidBody sp, RigidBody bod, out Vector2 axis, out float disp)
        {
            axis = new Vector2();
            disp = float.MinValue;
            if (sp is Particle && bod is PolyBody)
            {
                return TestCollisionPartiPoly((Particle)sp, (PolyBody)bod, out axis, out disp);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static bool TestCollisionPartiPoly(Particle p, PolyBody bod, out Vector2 axis, out float disp)
        {
            axis = new Vector2();
            disp = float.MinValue;

            var norms = bod.GetNormals().ToList();

            Vector2 closeV = Vector2.Zero;
            var verts = bod.GetTransformedVertices();
            float min = float.MaxValue;
            foreach (var v in verts)
            {
                var d = (p.Position - v).LengthSquared();
                if (d < min)
                {
                    d = min;
                    closeV = v;
                }
            }
            if (closeV != Vector2.Zero && p.Position - closeV != Vector2.Zero)
            {
                var pNorm = Vector2.Normalize(p.Position - closeV);
                norms.Add(pNorm);
            }

            //
            //Col test
            //
            foreach (Vector2 ax in norms)
            {
                float d = Projection.IntervalDist(p.Project(ax), bod.Project(ax));

                if (d > 0) //no collision if even 1 separating axis found
                {
                    disp = 0;
                    return false;
                }
                else if (d > disp) //return disp w/ lowset overlap (ie: highest separation)
                {
                    disp = d;
                    axis = ax;
                }

            }

            if (Vector2.Dot(p.Position - bod.Position, axis) < 0) //make sure the normal always points from p->bod. (should this use .GetCentroid() instead of position?)
                axis = -axis;
            return true;
        }
        public static bool TestCollisionPoly(Polygon a, Polygon b)
        {
            var axis = new Vector2();
            var disp = float.MinValue;
            return TestCollisionPoly(a, b, out axis, out disp);
        }
        public static bool TestCollisionPoly(Polygon a, Polygon b, out Vector2 axis, out float disp)
        {
            axis = new Vector2();
            disp = float.MinValue;
            if (TestCollisionPolyBroad(a,b))
            {
                return TestCollisionPolyFull(a, b, out axis, out disp);
            }
            else
            {
                return false;
            }

        }
        public static bool TestPointInPoly(Polygon a, Vector2 pt)
        {
            var norms = a.GetNormals();

            foreach (Vector2 ax in norms)
            {
                var p = a.Project(ax);
                var pt_p = Vector2.Dot(pt, ax);

                if (pt_p > p.Max || pt_p < p.Min) //no collision if even 1 separating axis found
                {
                    return false;
                }
            }

            return true; //must check all SAs before confirming a collision.
        }
        public static bool TestCollisionPolyBroad(Polygon a, Polygon b)
        {
            return true;
            //return BBox.Intersects(a.GetBoundingBoxTransformed(), b.GetBoundingBoxTransformed());
        }
        public static bool TestCollisionPolyFull(Polygon a, Polygon b, out Vector2 axis, out float disp) //axis is the normal of collision, FROM a TO b!. DISP IS ALWYAS NEGATIVE! (for collisions)
        {
            axis = new Vector2();
            disp = float.MinValue;

            //
            //Collision test
            //

            //var norms = a.GetNormals().ToList(); //used to be a union.
            //norms.AddRange(b.GetNormals().ToList());
            var norms = a.GetNormals().Union(b.GetNormals());

            foreach (Vector2 ax in norms)
            {
                float d = Projection.IntervalDist(a.Project(ax), b.Project(ax));

                if (d > 0) //no collision if even 1 separating axis found
                {
                    disp = 0;
                    return false;
                }
                else if (d > disp) //return disp w/ lowset overlap (ie: highest separation)
                {
                    disp = d;
                    axis = ax;
                }

            }

            if (Vector2.Dot(b.Position - a.Position, axis) < 0) //make sure the normal always points from a->b. (should this use .GetCentroid() instead of position?)
                axis = -axis;

            return true; //must check all SAs before confirming a collision. return SA w/ lowest overlap.
        }       

        public static List<Contact> FindSubContactsBody(RigidBody a, RigidBody b, Vector2 axis, float disp) //axis must be from a->b!
        {
            if (a is PolyBody && b is PolyBody)
            {
                return FindSubContactsPoly((PolyBody)a, (PolyBody)b, axis, disp);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static List<Contact> FindSubContactsPoly(PolyBody a, PolyBody b, Vector2 axis, float disp) //axis must be from a->b!
        {
            Vector2[] supA = GetSupports(b, axis); //get supports. instead of iterating over ALL vertices, only iterate over 4 closest, based on sep axis
            Vector2[] supB = GetSupports(a, -axis);

            var cons = new List<Contact>();

            for (int i = 0; i < supA.Length; i++)
                for (int j = 0; j < supB.Length; j++)
                {
                    Vector2 vout;
                    if (TestCollisionLine(supA[i], supA[(i + 1) % supA.Length], supB[j], supB[(j + 1) % supB.Length], out vout))
                    {
                        cons.Add(new Contact(a, b, vout, axis, disp));
                    }
                }

            return cons;

        }

        /// <summary>
        /// finds vertices of each polygon which are within the other polygon
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="axis"></param>
        /// <param name="disp"></param>
        /// <returns></returns>
        public static List<Contact> FindSubContactsPoly_CheapVerts(PolyBody a, PolyBody b, Vector2 axis, float disp) //axis must be from a->b!
        {
            var cons = new List<Contact>();

            foreach (var v in a.GetTransformedVertices())
            {
                if(TestPointInPoly(b, v))
                    cons.Add(new Contact(a, b, v, axis, disp));
            }
            foreach (var v in b.GetTransformedVertices())
            {
                if(TestPointInPoly(a, v))
                    cons.Add(new Contact(a, b, v, axis, disp));
            }

            return cons;
        }


        public static Contact FindFirstContact_ReallyCheap(PolyBody spirit, PolyBody body, Vector2 axis, float disp)
        {
            foreach (var v in spirit.GetTransformedVertices())
            {
                if (TestPointInPoly(body, v))
                    return new Contact(spirit, body, v, axis, disp);
            }
            return new Contact(null, null, Vector2.Zero, Vector2.Zero, 0f);
        }

        public static List<Vector2> FindPenetratingPoints(RigidBody a, RigidBody b, Vector2 axis)
        {
            if (a is PolyBody && b is PolyBody)
            {
                /*var cons = new List<Vector2>();

                foreach (var v in ((PolyBody)a).GetTransformedVertices())
                {
                    if (TestPointInPoly((PolyBody)b, v))
                        cons.Add(v);
                }
                foreach (var v in ((PolyBody)b).GetTransformedVertices())
                {
                    if (TestPointInPoly((PolyBody)a, v))
                        cons.Add(v);
                }

                return cons;*/

                
                Vector2[] supA = GetSupports(((PolyBody)b), axis); //get supports. instead of iterating over ALL vertices, only iterate over 4 closest, based on sep axis
                Vector2[] supB = GetSupports(((PolyBody)a), -axis);

                var cons = new List<Vector2>();

                for (int i = 0; i < supA.Length; i++)
                    for (int j = 0; j < supB.Length; j++)
                    {
                        Vector2 vout;
                        if (TestCollisionLine(supA[i], supA[(i + 1) % supA.Length], supB[j], supB[(j + 1) % supB.Length], out vout))
                        {
                            cons.Add(vout);
                        }
                    }

                return cons;
                
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        

        public static void SolveCollisions(List<RigidBody> bodies)
        {
            //bodies.ForEach(b => b.PenData.Clear());

            Enumerable.Range(0, bodies.Count).PForEach(i =>
                {
                    for (int j = i + 1; j < bodies.Count; j++)
                    {
                        Vector2 axis;
                        float disp;
                        var a = bodies[i];
                        var b = bodies[j];
                        if (!(a.IsFixed && b.IsFixed) && !(a.IsSleeping && b.IsSleeping) && TestCollisionBody(a, b, out axis, out disp))
                        {

                            var scon = FindSubContactsBody(a, b, axis, disp);
                            foreach (var c in scon)
                            {
                                ObjCollisionData dataA, dataB;
                                CalcCollision(c, out dataA, out dataB);

                                ApplyColData(a, dataA);
                                ApplyColData(b, dataB);
                            }

                            //ResolvePenetration_Cheap(a, b, axis, disp);

                            a.OnCollided(b, axis, disp);
                            b.OnCollided(a, -axis, disp);

                        }
                    }
                }

            );

            //bodies.ForEach(b => ApplyPenData(b, AggrPenData(b.PenData, .8f)));

        }

        public static void SolveAndDoPenitSpiritedCollisions(List<RigidBody> spirits, List<RigidBody> bodies)
        {
            try {
                Enumerable.Range(0, bodies.Count).PForEach(i =>
                                                               {
                                                                   for (int j = 0; j < spirits.Count; j++) {
                                                                       Vector2 axis;
                                                                       float disp;
                                                                       var sp = spirits[j];
                                                                       var bod = bodies[i];
                                                                       if (TestCollisionBody((PolyBody) sp,
                                                                                             (PolyBody) bod, out axis,
                                                                                             out disp)) {
                                                                           var con =
                                                                               FindFirstContact_ReallyCheap(
                                                                                   (PolyBody) sp, (PolyBody) bod, axis,
                                                                                   disp);
                                                                               //FindContactPartiPoly((Particle)sp, (PolyBody)bod, axis, disp);

                                                                           if (con.A != null) {
                                                                               ObjCollisionData dataA, dataB;
                                                                               CalcCollision(con, out dataA, out dataB);

                                                                               sp.Position += disp*axis;
                                                                                   //do the penetration stuff, exclusivly for the spirit

                                                                               ApplyColData(sp, dataA);
                                                                               ApplyColData(bod, dataB);

                                                                               sp.OnCollided(bod, axis, disp);
                                                                               bod.OnCollided(sp, -axis, disp);
                                                                           }

                                                                       }
                                                                   }
                                                               });


            }catch {
                
            }

        }

        /// <summary>
        /// generates a contact (and a list of subcontacts) for a pair of rigid bodies
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        private static bool testAndGenCon(RigidBody a, RigidBody b, out ContactP con)
        {
            Vector2 axis;
            float disp;
            con = new ContactP();

            if (!(a.IsFixed && b.IsFixed) && TestCollisionBody(a, b, out axis, out disp))
            {
                var pcons = FindPenetratingPoints(a, b, axis);
                con = new ContactP(a, b, axis, disp);
                con.ConPoints = pcons;
                return true;
            }
            return false;
        }

        private static List<ContactP> calcPens(List<RigidBody> bodies)
        {

            var pens = new List<ContactP>(500);

            Enumerable.Range(0, bodies.Count).PForEach(i =>
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    var a = bodies[i];
                    var b = bodies[j];
                    ContactP con;
                    if (testAndGenCon(a, b, out con))
                    {
                        pens.Add(con);
                    }
                }
            });

            return pens;
        }

        /// <summary>
        /// calculates contacts between all bodies, but skips enumeration of subcontacts
        /// </summary>
        /// <param name="bodies"></param>
        /// <returns></returns>
        private static List<ContactP> calcPens_Lite(List<RigidBody> bodies)
        {
            var pens = new List<ContactP>(500);

            Enumerable.Range(0, bodies.Count).PForEach(i =>
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    var a = bodies[i];
                    var b = bodies[j];
                    ContactP con = new ContactP();
                    Vector2 axis;
                    float disp;
                    if (!(a.IsFixed && b.IsFixed) && !(a.IsVegetated() && b.IsVegetated()) && TestCollisionBody(a, b, out axis, out disp))
                    {
                        con = new ContactP(a, b, axis, disp);
                        pens.Add(con);
                    }
                }
            });

            return pens;
        }

        public static void ResolvePenetrationAll(List<RigidBody> bodies, int iter)
        {
            for (int i = 0; i < iter; i++)
            {
                bodies.ForEach(b => b.PenData.Clear());

                var pens = calcPens(bodies);
                foreach (var pen in pens)
                {
                    ResolvePenetration(pen, 1);
                }

                bodies.ForEach(b =>
                {
                    var penD = AggrPenData(b.PenData, 2.0f);
                    ApplyPenData(b, penD);

                    b.OnPen(penD.dPos, penD.dRot);
                });
            }
        }

        public static void ResolvePenetrationAll_Cheap(List<RigidBody> bodies, int iter)
        {
            for (int i = 0; i < iter; i++)
            {
                bodies.ForEach(b => b.PenData.Clear());

                var pens = calcPens_Lite(bodies);
                foreach (var pen in pens)
                {
                    ResolvePenetration_Cheap(pen);
                }

                bodies.ForEach(b => 
                    {
                        var penD = AggrPenData(b.PenData, .8f);
                        ApplyPenData(b, penD);

                        b.OnPen(penD.dPos, penD.dRot);
                    });
            }
        }


        public static void ResolvePenetration_Cheap(ContactP pcon)
        {
            var a = pcon.A;
            var b = pcon.B;
            var axis = pcon.Axis;
            var disp = pcon.Disp;

            ResolvePenetration_Cheap(a, b, axis, disp);
        }

        public static void ResolvePenetration_Cheap(RigidBody a, RigidBody b, Vector2 axis, float disp)
        {
            ObjPenetrationData pA = new ObjPenetrationData();
            ObjPenetrationData pB = new ObjPenetrationData();

            var totInvMass = a.InvMass + b.InvMass;
            pA.dPos = +disp * axis * a.InvMass / totInvMass;
            pB.dPos = -disp * axis * b.InvMass / totInvMass;

            pA.dRot = 0f;
            pB.dRot = 0f;

            a.PenData.Add(pA);
            b.PenData.Add(pB);
        }

        /// <summary>
        /// should exit NOT MODIFYING the system, but adding a ObjCollisionData to each body.PenData
        /// </summary>
        /// <param name="pcon"></param>
        public static void ResolvePenetration(ContactP pcon, int iter)
        {
            //Queue<Vector2> cons = new Queue<Vector2>(pcon.ConPoints);
            //TODO: Priority Queue

            var psA = pcon.A.GetState(); //previous state of A
            var psB = pcon.B.GetState(); //previous state of B

            //while(pcon.ConPoints.Count > 0) //TODO: put limits on this
            
            /*for (int i = 0; i < iter && pcon.ConPoints.Count > 0; i++)
            {

                resolveSingleCon(pcon.A, pcon.B, pcon.ConPoints[0], pcon.Axis, pcon.Disp);

                var newcons = new ContactP();
                if (testAndGenCon(pcon.A, pcon.B, out newcons))
                {
                    pcon.ConPoints = newcons.ConPoints;
                }
                else
                {
                    break;
                }
            }*/


            foreach (var conp in pcon.ConPoints)
            {
                resolveSingleCon(pcon.A, pcon.B, conp, pcon.Axis, pcon.Disp);
            }

            ObjPenetrationData pdataA = new ObjPenetrationData();
            pdataA.dPos = pcon.A.Position - psA.Position;
            pdataA.dRot = pcon.A.Rotation - psA.Rotation;
            pcon.A.SetState(psA); //revert to pre-penetration simulation state

            ObjPenetrationData pdataB = new ObjPenetrationData();
            pdataB.dPos = pcon.B.Position - psB.Position;
            pdataB.dRot = pcon.B.Rotation - psB.Rotation;
            pcon.B.SetState(psB);


            pcon.A.PenData.Add(pdataA);
            pcon.B.PenData.Add(pdataB);
        }
        private static void resolveSingleCon(RigidBody a, RigidBody b, Vector2 con, Vector2 axis, float disp) //axis points a->b
        {
            /*float thresh = -.001f;
            if (disp > thresh)
            {
                return;
            }
            else
            {
                disp -= thresh;
            }*/

            var Ia = a.GetInertiaMoment();
            var Ib = b.GetInertiaMoment();
            var Ia_inv = a.GetInverseInertiaMoment();
            var Ib_inv = b.GetInverseInertiaMoment();

            var n = -axis; //points from B->A!!
            var n3 = n.ToVector3();



            var relPosA = (con - a.Position).ToVector3();
            var relPosB = (con - b.Position).ToVector3();


            Vector3 velA, velB, tvelA, tvelB;
            tvelA = Vector3.Cross(a.AngularVelocity * Vector3.UnitZ, relPosA); // tangentVel = (contactpt - centroid)x(angVel) // remember angVel is really on the Z axis.
            tvelB = Vector3.Cross(b.AngularVelocity * Vector3.UnitZ, relPosB); //also, remember position is the position of the axis of rotation (centroid, or fixed point). which works our well.

            velA = a.Velocity.ToVector3() + tvelA; //velocity of contact point = (vel of object) + (tangential vel of pt due to ang_vel)
            velB = b.Velocity.ToVector3() + tvelB;

            var torquePerImpulseA = Vector3.Cross(relPosA, n3);
            var torquePerImpulseB = Vector3.Cross(relPosB, n3);

            var rotPerImpA = torquePerImpulseA * Ia_inv;
            var rotPerImpB = torquePerImpulseB * Ib_inv;

            var velPerImpA = Vector3.Cross(rotPerImpA, relPosA); //angularInertiaWorld (pg326)
            var velPerImpB = Vector3.Cross(rotPerImpB, relPosB);


            Matrix localToWorld = CreateOrthonormalBasis(n, n.Perpen());
            Matrix worldToLocal = Matrix.Transpose(localToWorld);

            var angularInertiaA = velPerImpA.TransformToLocal(worldToLocal).X; //"deltaVelocity"
            var angularInertiaB = velPerImpB.TransformToLocal(worldToLocal).X;

            var linearInertiaA = Ia_inv;
            var linearInertiaB = Ib_inv;

            var totalInertia = angularInertiaA + angularInertiaB + linearInertiaA + linearInertiaB;
            var invI = 1 / totalInertia;

            var linearMoveA =  disp * linearInertiaA * invI;
            var linearMoveB = -disp * linearInertiaB * invI;

            var angMoveA =  disp * angularInertiaA * invI;
            var angMoveB = -disp * angularInertiaB * invI;

            var impulsiveTorqueA = Vector3.Cross(relPosA, n3);
            var impulsiveTorqueB = Vector3.Cross(relPosB, n3);

            var impPerMoveA = impulsiveTorqueA * Ia_inv;
            var impPerMoveB = impulsiveTorqueB * Ib_inv;
            
            a.Position += linearMoveA * axis;
            b.Position += linearMoveB * axis;

            a.Rotation += -impPerMoveA.Z * angMoveA * Ia_inv;
            b.Rotation += -impPerMoveB.Z * angMoveB * Ib_inv;
        }

        public static Matrix CreateOrthonormalBasis(Vector2 x, Vector2 ySugg)
        {
            Vector3 z = Vector3.Cross(x.ToVector3(), ySugg.ToVector3());
            if (z.LengthSquared() == 0.0) new Exception("arguments cannot be parrallel.");

            Vector3 y = Vector3.Cross(z, x.ToVector3());
            y.Normalize();
            z.Normalize();

            Matrix m = new Matrix();
            m.Right = x.ToVector3();
            m.Up = y;
            m.Forward = z;
            m.Translation = Vector3.Zero;

            return m;
        }
        public static Matrix CreateSkewSymmetric(Vector3 v)
        {
            Matrix m = new Matrix();
            m.M11 = 0;
            m.M12 = -v.Z;
            m.M13 = v.Y;
            m.M21 = v.Z;
            m.M22 = 0;
            m.M23 = -v.X;
            m.M31 = -v.Y;
            m.M32 = v.X;
            m.M33 = 0;

            m.Translation = Vector3.Zero;

            return m;
        }
        public static void CalcCollisionNoFrict(Contact c, out ObjCollisionData dataA, out ObjCollisionData dataB)
        {
            dataA.dVel = Vector2.Zero;
            dataB.dVel = Vector2.Zero;
            dataA.dAngVel = 0;
            dataB.dAngVel = 0;
            dataA.dPos = Vector2.Zero;
            dataB.dPos = Vector2.Zero;


            var con = c.ConPoint;
            var a = c.A;
            var b = c.B;
            var disp = c.Disp;

            var Ia = a.GetInertiaMoment();
            var Ib = b.GetInertiaMoment();

            var Ia_inv = a.GetInverseInertiaMoment();
            var Ib_inv = b.GetInverseInertiaMoment();

            var axis = c.Axis; //from A->B
            var n = -axis; //points from B->A
            var n3 = n.ToVector3();

            var u = (a.Friction + b.Friction) / 2;
            var res = (a.Restitution + b.Restitution) / 2;
            //ADD: limit the rest. if sep vel is slow

            var totInvMass = a.InvMass + b.InvMass;
            dataA.dPos = +disp * axis * a.InvMass / totInvMass;
            dataB.dPos = -disp * axis * b.InvMass / totInvMass;


            var relPosA = (con - a.Position).ToVector3();
            var relPosB = (con - b.Position).ToVector3();
                                                     
                                                                   
            Vector3 velA, velB, tvelA, tvelB;
            tvelA = Vector3.Cross(a.AngularVelocity * Vector3.UnitZ, relPosA); // tangentVel = (contactpt - centroid)x(angVel) // remember angVel is really on the Z axis.
            tvelB = Vector3.Cross(b.AngularVelocity * Vector3.UnitZ, relPosB); //also, remember position is the position of the axis of rotation (centroid, or fixed point). which works our well.

            velA = a.Velocity.ToVector3() + tvelA; //velocity of contact point = (vel of object) + (tangential vel of pt due to ang_vel)
            velB = b.Velocity.ToVector3() + tvelB;

            var torquePerImpulseA = Vector3.Cross(relPosA, n3);
            var torquePerImpulseB = Vector3.Cross(relPosB, n3);

            var rotPerImpA = torquePerImpulseA * Ia_inv;
            var rotPerImpB = torquePerImpulseB * Ib_inv;

            var velPerImpA = Vector3.Cross(rotPerImpA, relPosA); //"deltaVelWorld"
            var velPerImpB = Vector3.Cross(rotPerImpB, relPosB);


            Matrix localToWorld = CreateOrthonormalBasis(n, n.Perpen());
            Matrix worldToLocal = Matrix.Transpose(localToWorld);

            var velPerImpA_Contact = velPerImpA.TransformToLocal(worldToLocal); //"deltaVelocity"
            var velPerImpB_Contact = velPerImpB.TransformToLocal(worldToLocal);

            var angCompA = velPerImpA_Contact.X;
            var angCompB = velPerImpB_Contact.X;


            var deltaVelocity = angCompA + angCompB + a.InvMass + b.InvMass;


            var sepVel = velA - velB;
            var conVel = sepVel.TransformToLocal(worldToLocal);

            var desiredVel = -conVel.X * (1 + res);

            Vector3 impulseContact;
            impulseContact.X = desiredVel / deltaVelocity;
            impulseContact.Y = 0;
            impulseContact.Z = 0;

            var impulse = impulseContact.TransformToWorld(localToWorld);
            var impulse2 = impulse.ToVector2();

            dataA.dVel = impulse2 * a.InvMass;
            dataB.dVel = -impulse2 * b.InvMass;

            var impulsiveTorqueA = Vector3.Cross(relPosA, impulse);
            var impulsiveTorqueB = Vector3.Cross(relPosB, -impulse);

            dataA.dAngVel = impulsiveTorqueA.Z * a.GetInverseInertiaMoment();
            dataB.dAngVel = impulsiveTorqueB.Z * b.GetInverseInertiaMoment();
        }

        public static void CalcCollision(Contact c, out ObjCollisionData dataA, out ObjCollisionData dataB)
        {
            dataA.dVel = Vector2.Zero;
            dataB.dVel = Vector2.Zero;
            dataA.dAngVel = 0;
            dataB.dAngVel = 0;
            dataA.dPos = Vector2.Zero;
            dataB.dPos = Vector2.Zero;


            var con = c.ConPoint;
            var a = c.A;
            var b = c.B;
            var disp = c.Disp;

            var Ia = a.GetInertiaMoment();
            var Ib = b.GetInertiaMoment();

            var Ia_inv = a.GetInverseInertiaMoment();
            var Ib_inv = b.GetInverseInertiaMoment();

            var axis = c.Axis; //from A->B
            var n = -axis; //points from B->A
            var n3 = n.ToVector3();

            var u = (a.Friction + b.Friction) / 2;
            var res = (a.Restitution + b.Restitution) / 2;
            //TODO: limit the rest. if sep vel is slow

            //var totInvMass = a.InvMass + b.InvMass;
            //dataA.dPos = +disp * axis * a.InvMass / totInvMass;
            //dataB.dPos = -disp * axis * b.InvMass / totInvMass;


            var relPosA = (con - a.Position).ToVector3();
            var relPosB = (con - b.Position).ToVector3();


            Vector3 velA, velB, tvelA, tvelB;
            tvelA = Vector3.Cross(a.AngularVelocity * Vector3.UnitZ, relPosA); // tangentVel = (contactpt - centroid)x(angVel) // remember angVel is really on the Z axis.
            tvelB = Vector3.Cross(b.AngularVelocity * Vector3.UnitZ, relPosB); //also, remember position is the position of the axis of rotation (centroid, or fixed point). which works our well.

            velA = a.Velocity.ToVector3() + tvelA; //velocity of contact point = (vel of object) + (tangential vel of pt due to ang_vel)
            velB = b.Velocity.ToVector3() + tvelB;
            

            var relVel = velB - velA;
            var t = relVel - n3 * Vector3.Dot(relVel, n3);
            if (t != Vector3.Zero)
                t.Normalize();
            else
                t = n.Perpen().ToVector3();
            
            Matrix localToWorld = CreateOrthonormalBasis(n, n.Perpen());
            Matrix worldToLocal = Matrix.Transpose(localToWorld);


            var sepVel = velA - velB;
            var conVel = sepVel.TransformToLocal(worldToLocal);
            var desiredVel = -conVel.X * (1 + res);



            //
            //Friction-collision calcs
            //

            var impulseToTorqueA = CreateSkewSymmetric(relPosA);
            /*var torquePerUnitImpulseA = impulseToTorqueA * localToWorld;
            var angVelPerUnitImpulseA = torquePerUnitImpulseA * a.GetInverseInertiaMoment();
            var velPerUnitImpulseA = -angVelPerUnitImpulseA * impulseToTorqueA;
            var velPerUnitImpulseContactA = worldToLocal * velPerUnitImpulseA; //used to transform an impulse in contact coordinates -> velocity in contact coordinates
            */

            var impulseToTorqueB = CreateSkewSymmetric(relPosB);
            /*var torquePerUnitImpulseB = impulseToTorqueB * localToWorld;
            var angVelPerUnitImpulseB = torquePerUnitImpulseB * b.GetInverseInertiaMoment();
            var velPerUnitImpulseB = -angVelPerUnitImpulseB * impulseToTorqueB;
            var velPerUnitImpulseContactB = worldToLocal * velPerUnitImpulseB;
            */


            var delVelWorldA = -1*(impulseToTorqueA * a.GetInverseInertiaMoment() * impulseToTorqueA); //matrix which transforms impluse -> velocity in world coordinates ie: Velocity Per Unit Impulse
            var delVelWorldB = -1*(impulseToTorqueB * b.GetInverseInertiaMoment() * impulseToTorqueB);

            var delVelWorld = delVelWorldA + delVelWorldB;
            var delVelContact = worldToLocal * delVelWorld * localToWorld;


            var invM = a.InvMass + b.InvMass;
            delVelContact.M11 += invM;
            delVelContact.M22 += invM;
            delVelContact.M33 += invM;


            MathNet.Numerics.LinearAlgebra.Matrix m = delVelContact.ToMathNet();
            var mi = m.Inverse(); //from vel per impulse -> impulse per vel
            Matrix impulsePerVel = mi.ToXNA();


            //var velKill = (desiredVel * Vector3.UnitX); //NO FRICTION
            var velKill = (desiredVel * Vector3.UnitX) - conVel.Y*Vector3.UnitY - conVel.Z*Vector3.UnitZ; //default = super mega-friction (ie: kill all talgential velocities
            var impulseContact = Vector3.TransformNormal(velKill, impulsePerVel);


            var planarImp = (float)Math.Sqrt(impulseContact.Y * impulseContact.Y + impulseContact.Z * impulseContact.Z);

            if (planarImp != 0 && planarImp > impulseContact.X * u)
            {
                //handle dynamic friction
                impulseContact.Y /= planarImp; //basically normalize (preserve direction, set len (y,x)=0
                impulseContact.Z /= planarImp;

                impulseContact.X = desiredVel / (delVelContact.M11 + delVelContact.M12 * u * impulseContact.Y + delVelContact.M13 * u * impulseContact.Z); //shorhand for the matrix transform
                impulseContact.Y *= u * impulseContact.X; //scale up the impluses as friction*normal (since impCon.X = normal)
                impulseContact.Z *= u * impulseContact.X;
            }



            //
            //impulse from ImpulseContact + apply
            //

            var impulse = impulseContact.TransformToWorld(localToWorld);
            var impulse2 = impulse.ToVector2();

            dataA.dVel = impulse2 * a.InvMass;
            dataB.dVel = -impulse2 * b.InvMass;

            var impulsiveTorqueA = Vector3.Cross(relPosA, impulse);
            var impulsiveTorqueB = Vector3.Cross(relPosB, -impulse);

            dataA.dAngVel = impulsiveTorqueA.Z * a.GetInverseInertiaMoment();
            dataB.dAngVel = impulsiveTorqueB.Z * b.GetInverseInertiaMoment();

        }
        public static void CalcCollisionOld(Contact c, out ObjCollisionData dataA, out ObjCollisionData dataB)
        {
            dataA.dVel = Vector2.Zero; //incase the method exits before ang velocities need to be calculated (ie, if the contact point is separating)
            dataB.dVel = Vector2.Zero;
            dataA.dAngVel = 0;
            dataB.dAngVel = 0;
            dataA.dPos = Vector2.Zero;
            dataB.dPos = Vector2.Zero;

            var a = c.A;
            var b = c.B;
            var disp = c.Disp;
            var axis = c.Axis; //axis from A to B
            var con = c.ConPoint;

            var Ia = a.GetInertiaMoment();
            var Ib = b.GetInertiaMoment();


            var totInvMass = a.InvMass + b.InvMass;
            dataA.dPos = +disp * axis * a.InvMass / totInvMass;
            dataB.dPos = -disp * axis * b.InvMass / totInvMass;


            //
            //impulse calculations to set momentum
            //

            var radA = (con - a.Position);
            var radB = (con - b.Position);

            Vector2 velA, velB, tvelA, tvelB;
            tvelA = Vector3.Cross(a.AngularVelocity * Vector3.UnitZ, radA.ToVector3()).ToVector2(); // tangentVel = (contactpt - centroid)x(angVel) // remember angVel is really on the Z axis.
            tvelB = Vector3.Cross(b.AngularVelocity * Vector3.UnitZ, radB.ToVector3()).ToVector2(); //also, remember position is the position of the axis of rotation (centroid, or fixed point). which works our well.

            velA = a.Velocity + tvelA; //velocity of contact point = (vel of object) + (tangential vel of pt due to ang_vel)
            velB = b.Velocity + tvelB;

            var sepVel = Vector2.Dot((velB - velA), axis); //find the separating velocity of the contact points
            if (sepVel > 0) return; //if the objects are already separatingm, then skip the impulse calcs below

            var res =  (a.Restitution + b.Restitution) / 2;
            var n = -axis; //-axis?
            
            
            var tmpA = Vector3.Dot(Vector3.Cross(n.ToVector3(), Vector3.Cross(radA.ToVector3(), n.ToVector3()) / Ia), radA.ToVector3());
            var tmpB = Vector3.Dot(Vector3.Cross(n.ToVector3(), Vector3.Cross(radB.ToVector3(), n.ToVector3()) / Ib), radB.ToVector3());
            var impulse = -sepVel * (res + 1) / (a.InvMass + b.InvMass + tmpA + tmpB); //angular

            var relVel = (velA - velB);
            var t = Vector3.Cross(Vector3.Cross(n.ToVector3(), relVel.ToVector3()), n.ToVector3()).ToVector2();  
            t = relVel - n*Vector2.Dot(relVel, n);
            if (t != Vector2.Zero) t.Normalize();

            var u = (a.Friction + b.Friction) / 2;

            dataA.dVel = (impulse * n + impulse * u * t) * a.InvMass;
            dataB.dVel = (-impulse * n + impulse * u * t) * b.InvMass;

            dataA.dAngVel = (Vector3.Cross(radA.ToVector3(), (impulse * n.ToVector3() + impulse * u * t.ToVector3()))).Z * a.GetInverseInertiaMoment();
            dataB.dAngVel = (Vector3.Cross(radB.ToVector3(), (-impulse * n.ToVector3() + impulse * u * t.ToVector3()))).Z * b.GetInverseInertiaMoment();
            
        }
        public static void ResolveContact(Contact c)
        {

            var a = c.A;
            var b = c.B;
            var disp = c.Disp;
            var axis = c.Axis; //axis from A to B
            var con = c.ConPoint;

            var Ia = a.GetInertiaMoment();
            var Ib = b.GetInertiaMoment();


            var totInvMass = a.InvMass + b.InvMass;
            a.Position += +disp * axis * a.InvMass / totInvMass;
            b.Position += -disp * axis * b.InvMass / totInvMass;


            //
            //impulse calculations to set momentum
            //

            var radA = (con - a.Position);
            var radB = (con - b.Position);

            Vector2 velA, velB, tvelA, tvelB;
            tvelA = Vector3.Cross(a.AngularVelocity * Vector3.UnitZ, radA.ToVector3()).ToVector2(); // tangentVel = (contactpt - centroid)x(angVel) // remember angVel is really on the Z axis.
            tvelB = Vector3.Cross(b.AngularVelocity * Vector3.UnitZ, radB.ToVector3()).ToVector2(); //also, remember position is the position of the axis of rotation (centroid, or fixed point). which works our well.

            velA = a.Velocity + tvelA; //velocity of contact point = (vel of object) + (tangential vel of pt due to ang_vel)
            velB = b.Velocity + tvelB;

            var sepVel = Vector2.Dot((velB - velA), axis); //find the separating velocity of the contact points
            if (sepVel > 0) return; //if the objects are already separatingm, then skip the impulse calcs below

            var res = (a.Restitution + b.Restitution) / 2;
            var n = -axis; //-axis?

            var tmpA = Vector3.Dot(n.ToVector3(), (Vector3.Cross(radA.ToVector3(), n.ToVector3()) / Ia));
            var tmpB = Vector3.Dot(n.ToVector3(), (Vector3.Cross(radB.ToVector3(), n.ToVector3()) / Ib));


            var impulse = -sepVel * (res + 1) / (a.InvMass + b.InvMass + tmpA + tmpB); //angular

            //var t = -Vector2.Normalize((velB - velA));
            var t = Vector3.Cross(Vector3.Cross(n.ToVector3(), (velA - velB).ToVector3()), n.ToVector3()).ToVector2();

            var u = (a.Friction + b.Friction) / 2;

            a.Velocity += (impulse * n + impulse * u * t) * a.InvMass;
            b.Velocity += (-impulse * n + impulse * u * t) * b.InvMass;

            a.AngularVelocity += (Vector3.Cross(radA.ToVector3(), (impulse * n.ToVector3() + impulse * u * t.ToVector3()))).Z * a.InvMass;
            b.AngularVelocity += (Vector3.Cross(radB.ToVector3(), (-impulse * n.ToVector3() + impulse * u * t.ToVector3()))).Z * b.InvMass;


        }

        static Vector2 maxDisp(List<ObjCollisionData> vects)
        {
            int i = 0;
            var max = vects.FirstOrDefault().dPos;
            float maxl = 0.0f;
            for (int a = 0; a < vects.Count; a++)
            {
                if (vects[a].dPos.Length() > maxl)
                {
                    i = a;
                    maxl = vects[a].dPos.Length();
                }
            }
            return max;
        }
        public static ObjCollisionData AggrColData(List<ObjCollisionData> cols)
        {
            ObjCollisionData dat = new ObjCollisionData();
            float totWeight = 0f;
            foreach (var c in cols)
            {
                var weight = 1; // c.dPos.Length();

                dat.dPos += weight * c.dPos;
                dat.dVel += weight * c.dVel;
                dat.dAngVel += weight * c.dAngVel;
                totWeight += weight;
            }
            if (totWeight == 0f) totWeight = 1;
            dat.dPos /= totWeight;
            dat.dVel /= totWeight;
            dat.dAngVel /= totWeight;

            return dat;
        }
        public static ObjPenetrationData AggrPenData(List<ObjPenetrationData> cols, float soft)
        {
            ObjPenetrationData dat = new ObjPenetrationData();
            float totWeight = 0f;
            foreach (var c in cols)
            {
                var weight = 1; // c.dPos.Length();

                dat.dPos += weight * c.dPos;
                dat.dRot += weight * c.dRot;
                totWeight += weight;
            }
            if (totWeight == 0f) totWeight = 1;
            dat.dPos /= totWeight * soft; //TODO: make this a const
            dat.dRot /= totWeight * soft;

            return dat;
        }
        public static void ApplyColData(RigidBody body, ObjCollisionData colD)
        {
            body.SetMasterPosition(body.Position + colD.dPos);
            body.Velocity += colD.dVel;
            body.AngularVelocity += colD.dAngVel;
        }
        public static void ApplyPenData(RigidBody body, ObjPenetrationData penD)
        {
            body.SetMasterPosition(body.Position + penD.dPos);
            body.SetMasterRotation(body.Rotation + penD.dRot);
        }
        public static void SolveAllBodies(List<RigidBody> bodies)
        {
            SolveCollisions(bodies);
        }
        public static void SolveAllDoPenitSpirits(List<RigidBody> spirits, List<RigidBody> bodies)
        {
            SolveAndDoPenitSpiritedCollisions(spirits, bodies);
        }
        public static Vector2 GetClosestPoint(Vector2 pt, Vector2 a, Vector2 b) //gets point on ab closest to pt (projection on pt onto ab, really)
        {
            /*var norm = Vector2.Normalize(b - a);
            var v = Vector2.Dot(norm, pt)*norm;
            return (v-Vector2.Dot(a,norm)*norm)+a;*/

            // THIS CODE MAY BE FASTER:
            Vector2 AB = b - a;
            Vector2 AP = pt - a;
            float ab_ab = AB.LengthSquared();
            float ab_ap = Vector2.Dot(AP, AB);
            float t = ab_ap / ab_ab;
            //t = (t < 0.0f)? 0.0f : (t > 1.0f)? 1.0f : t;

            return a + t * AB;

        }
        public static bool TestCollisionLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            float THRESH = 0.0001f;
            intersection = Vector2.Zero;

            float ua = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
            float ub = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
            float denom = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);

            if (Math.Abs(denom) <= THRESH)
            {

                //lines are coincident-ish

                var ax1 = Vector2.Normalize((a2 - a1).Perpen());
                var ax2 = Vector2.Normalize((b2 - b1).Perpen());

                if (Math.Abs(Vector2.Dot(a1, ax1) - Vector2.Dot(b1, ax1)) <= THRESH || Math.Abs(Vector2.Dot(a1, ax2) - Vector2.Dot(b1, ax2)) <= THRESH)
                {
                    intersection = (a1 + a2 + b1 + b2) / 4;
                    return true;
                }
            }
            else
            {
                ua /= denom;
                ub /= denom;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    intersection.X = a1.X + ua * (a2.X - a1.X);
                    intersection.Y = a1.Y + ua * (a2.Y - a1.Y);
                    return true;
                }
            }
            return false;
        }
        public static Vector2[] GetSupports(Polygon p, Vector2 dir) // where dir is the collision normal, pointing from (object)->(p)
        {
            var verts = p.GetTransformedVertices();
            var sVerts = verts.Select((v, i) => new { Index = i, Vertex = v }).OrderBy(a => Vector2.Dot(a.Vertex, dir)).Take(4).OrderBy(a => a.Index).Select(a => a.Vertex).ToArray();

            return sVerts;
        }

    }
}
