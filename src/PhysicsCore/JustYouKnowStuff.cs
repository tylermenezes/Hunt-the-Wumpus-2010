using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HuntTheWumpus.PhysicsCore
{
    public class ParticleEmmiter
    {
        PhysicsEngine pe;
        float ttl;

        public bool IsOn = false;

        public Vector2 Position;

        Vector2 tar;
        public Vector2 Target
        {
            get { return tar; }
            set
            {
                tar = Vector2.Normalize(value);
            }
        }

        public float Velocity;

        float n_acc = 0f;

        /// <summary>
        /// number emmitted per second
        /// </summary>
        public float Rate = 1f;


        public ParticleEmmiter(PhysicsEngine pe, Vector2 pos, Vector2 tar, float vel, float rate, float maxAge)
        {
            this.pe = pe;

            this.Position = pos;
            this.Target = tar;
            this.Rate = rate;
            this.ttl = maxAge;
            this.Velocity = vel;
        }
        public void TurnOn()
        {
            this.IsOn
                 = true;
        }
        public void TurnOff()
        {
            this.IsOn = false;
        }
        public void Update(float dt)
        {
            if (this.IsOn)
            {
                n_acc += dt * Rate;

                while (n_acc >= 1)
                {
                    n_acc -= 1;
                    Emit();
                }
            }
        }
        public void Emit()
        {
            var p = new Particle(.8f, .5f, ttl);
            p.Position = this.Position;
            p.Velocity = this.Target * this.Velocity;
            pe.AddSpirit(p);
        }
    }
}
