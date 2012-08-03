using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HuntTheWumpus.GameEngineCore;
using Microsoft.Xna.Framework;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.GraphicsCore;


namespace HuntTheWumpus.GameEngineCore.PhysicsWeapons
{
    class FlameBall : Actor, IPhysicsWeapon
    {
        public float WeaponPower { get; private set; }
        public FlameBall(Vector2 pos, Vector2 vel, float maxAge)
        {
            PolyBody = new Particle(.8f, .5f, maxAge);
            PolyBody.Position = pos;
            PolyBody.Velocity = vel;
            PolyBody.Deleted += new PhysicsEventHandler(p_Deleted);
            Scale = 1f;

            PolyBody.Rotation = ((float)(new Random()).NextDouble()) * 2f * (float)Math.PI;

            Load();
        }

        void p_Deleted(RigidBody sender, object delta)
        {
            this.Kill();
        }

        public override void setupGraphics()
        {
            Sprite = new Sprite("FlameBall", State.Name, Scale);
        }
        public override void setupPhysics()
        {
            //lol
        }
        protected override string ParseStandardConstructor()
        {
            return "";
            
            //throw new Exception("Nooooooooo");
        }
    }
}
