using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.Utilities;
using HuntTheWumpus.GraphicsCore;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    class Bouncer : Actor
    {

        public float Radius { get; private set; }
        public const float DefaultRadius = 1;
        public Bouncer()
             : this(DefaultRadius)
        {
        }

        public Bouncer(float radius)
        {
            Radius = radius;
            PolyBody = new PolyBody(1, 5);
            PolyBody.Restitution = 0.0f;
            color = Color.Gray;
            Load();
        }
        public Bouncer(float radius, Color _color)
            :this(radius)
        {
            color = _color;
        }

        public Bouncer(float radius, Color _color, float x, float y)
            : this(radius, _color) 
        {
            PolyBody.Position = new Vector2(x, y);
        }

        public override void setupGraphics()
        {
            NoTexture = true;
        }

        public override void setupPhysics()
        {
            PolyBody.MakeRegularFromRad(Radius);
        }

        public override void actUpon()
        {
            PolyBody.Velocity = 10 * Vector2.UnitY;
        }
        

        protected override string ParseStandardConstructor()
        {
            // float radius, Color _color, float x, float y
            return String.Format(
                @"<float>{0}</float>
                 <Color>{1}</Color>
                 <float>{2}</float>
                 <float>{3}</float>",
                Radius, color.ToVector3().ToPrettyString(), PolyBody.Position.X, PolyBody.Position.Y);
        }
    }
}
