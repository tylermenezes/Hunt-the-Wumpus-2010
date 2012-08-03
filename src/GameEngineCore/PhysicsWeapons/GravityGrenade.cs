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
    class GravityGrenade : Actor, IPhysicsWeapon
    {
        public float WeaponPower { get; private set; }
        public const float LaunchSpeed = 20;
        public const float BlastRange = 3;

        public Actor Launcher { get; private set; }
        public GravityGrenade(Actor launcher, float weaponPower, Vector2 destination)
        {
            Scale = .7f;
            this.doNotSave = true;
            var player = (GameEngine.Singleton.GetPlayState())
                .ActiveMap.MainPlayer;
            Launcher = launcher;
            WeaponPower = weaponPower;
            PolyBody = new PolyBody(.1f, 3);
            PolyBody.Gravity = -9.8f * Vector2.UnitY;
            Load();
            PolyBody.Position = launcher.PolyBody.Position +
                (launcher.PolyBody.GetWidth() + (Sprite.BoundingBox().X/2)) * Math.Sign(destination.X - launcher.PolyBody.Position.X) *
                Vector2.UnitX;
            PolyBody.Velocity = computeTargetVelocity(destination);

            PolyBody.Collided += new PhysicsEventHandler(Detonate);
        }

        public override void setupGraphics()
        {
            Sprite = new Sprite("GravityGrenade", State.Name, Scale);
        }

        public override void setupPhysics()
        {
            PolyBody.MakeRect(Sprite.BoundingBox().X, Sprite.BoundingBox().Y);
        }
        private Vector2 computeTargetVelocity(Vector2 destination)
        {            
            // I really hope this works! //
            var g = PolyBody.Gravity.Y;
            var v = LaunchSpeed;
            var v2 = Math.Pow(LaunchSpeed, 2);
            var x = destination.X - PolyBody.Position.X;
            var y = destination.Y - PolyBody.Position.Y;
            var direction = Math.Sign(destination.X - PolyBody.Position.X);

            var u = (v2 - Math.Sqrt(Math.Pow(v2, 2) - g * (g * Math.Pow(x, 2) + 2 * y * v2))) / (g * x);
            var U = (direction == 1) ?
                Math.Sqrt(1 + Math.Pow(u, 2)) :
                -Math.Sqrt(1 + Math.Pow(u, 2));

            var velocity = new Vector2((float)(v / U),
                (float)(v * u / U));

            return float.IsNaN(velocity.Y) ? Vector2.UnitY * v : velocity;
        }

        void Detonate(RigidBody sender, object ignoreParamater)
        {
            this.Kill();
            PolyBody.ChangeWidth(BlastRange);
            PolyBody.ChangeHieght(BlastRange);
            
            var game = GameEngine.Singleton.GetPlayState();
            foreach (Actor actor in game.ActiveMap.GameObjects)
            {
                if (actor != Launcher &&
                    CollisionEngine.TestCollisionPoly(this.PolyBody, actor.PolyBody))
                    actor.PolyBody.Gravity = WeaponPower*Vector2.UnitY;
            }
        }

        public override void actUpon()
        {
            Detonate(null, Vector2.Zero); 
        }

        protected override string ParseStandardConstructor()
        {
            throw new Exception("Nooooooooo");
        }
    }
}
