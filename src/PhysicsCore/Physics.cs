using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;
using HuntTheWumpus.Utilities;


namespace HuntTheWumpus.PhysicsCore
{

    public class PhysicsEngine
    {

        List<RigidBody> bodies = new List<RigidBody>();
        List<RigidBody> spirits = new List<RigidBody>();
        List<BinaryForceComponent> bfcs = new List<BinaryForceComponent>(); //not really used for anything, besides storage

        public List<RigidBody> PolyBodies { get { return bodies; } }
        public List<RigidBody> Spirits { get { return spirits; } }
        public List<BinaryForceComponent> BinaryForceComponents { get { return bfcs; } }


        public PhysicsEngine()
        {
            
        }
        private void addDefaults(RigidBody b)
        {

            FieldInfo[] fis = typeof(DefaultGenerators).GetFields();
            foreach (var fi in fis)
            {
                if (fi.FieldType == typeof(ForceGenerator))
                {
                    b.FGens.Add((ForceGenerator)fi.GetValue(null));
                }
                else if (fi.FieldType == typeof(TorqueGenerator))
                {
                    b.TGens.Add((TorqueGenerator)fi.GetValue(null));
                }

            }

        }
        public void AddRigidBody(RigidBody b, bool addDefaultGenerators)
        {
            bodies.Add(b);
            if (addDefaultGenerators) addDefaults(b);
        }
        public void AddRigidBody(RigidBody b)
        {
            AddRigidBody(b, true);
        }
        public void AddSpirit(RigidBody b)
        {
            spirits.Add(b);
            addDefaults(b);
        }
        public void RemoveRigidBody(RigidBody b)
        {
            b.OnDeleted();
            bodies.Remove(b);
        }
        public void RemoveSpirit(RigidBody s)
        {
            s.OnDeleted();
            spirits.Remove(s);
        }
        public void AddBinaryForceComponent(BinaryForceComponent bfc)
        {
            bfc.BodyA.FGens.Add(bfc.ForceGenA);
            bfc.BodyB.FGens.Add(bfc.ForceGenB);
            bfcs.Add(bfc);
        }
        public void RemoveBinaryForceComponent(BinaryForceComponent bfc)
        {
            bfc.BodyA.FGens.Remove(bfc.ForceGenA);
            bfc.BodyB.FGens.Remove(bfc.ForceGenB);
            bfcs.Remove(bfc);
        }
        public void Update(float dt)
        {
            int num = 2;
            float dtIter = dt/ num;

            for (int i = 0; i < num; i++)
                UpdateIter(dtIter);
        }
        private void UpdateIter(float dt)
        {


            //var all = new List<RigidBody>();
           // all.AddRange(bodies);
            //all.AddRange(spirits);

            RigidBody[] bTemp = new RigidBody[bodies.Count];
            bodies.CopyTo(bTemp);
            foreach (var b in bTemp)//add forces from force generators
            {
                if (!b.IsFixed) //don't update/check collisions w/ a fixed obj. as base.
                {
                    if (!b.IsSleeping)
                    {
                        //
                        //advance the state
                        //


                        b.FGens.ForEach(x => b.AddForce(x(b))); //step 1: sum forces
                        Vector2 a = b.FNET * b.InvMass;         //step 2: calc acceleration
                        b.Velocity += a * dt;                   //step 3: ???
                        b.Position += b.Velocity * dt;          //step 4: PROFIT!!

                        b.TGens.ForEach(g => b.AddTorque(g(b)));
                        float ang_a = b.TNET * b.GetInverseInertiaMoment();
                        b.AngularVelocity += ang_a * dt;
                        b.Rotation += b.AngularVelocity * dt;

                        b.ResetForces();
                    }


                    b.UpdateActivity(dt);
                    if (b.IsSleepy())
                        b.GoToSleep();

                }
                
                
            }


            RigidBody[] sTemp = new RigidBody[spirits.Count];
            spirits.CopyTo(sTemp);
            foreach (var s in sTemp)//add forces from force generators
            {

                //
                //advance the state
                //
                var b = (Particle)s;

                b.FGens.ForEach(x => b.AddForce(x(b))); //step 1: sum forces
                Vector2 a = b.FNET * b.InvMass;         //step 2: calc acceleration
                b.Velocity += a * dt;                   //step 3: ???
                b.Position += b.Velocity * dt;          //step 4: PROFIT!!

                b.TGens.ForEach(g => b.AddTorque(g(b)));
                float ang_a = b.TNET * b.GetInverseInertiaMoment();
                b.AngularVelocity += ang_a * dt;
                b.Rotation += b.AngularVelocity * dt;

                b.ResetForces();

                b.UpdateActivity(dt);
                if (b.IsOld())
                {
                    this.RemoveSpirit(s);
                }

            }



            CollisionEngine.SolveAllBodies(bodies);
            CollisionEngine.SolveAllDoPenitSpirits(spirits, bodies);
            CollisionEngine.ResolvePenetrationAll_Cheap(bodies, 2);
        }

    }
}
