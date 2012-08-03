using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    class MovablePlatform : Actor
    {
        public float Radius { get; private set; }

        public const float DefaultRadius = 1;

        public MovablePlatform()
            : this(DefaultRadius)
        { }

        public MovablePlatform(float radius)
        {
            Radius = radius;
            PolyBody = new PolyBody(1, 4);
            color = Color.Gray;
            Load();
        }
        public MovablePlatform(float radius, Color _color)
            : this(radius)
        {
            color = _color;
        }

        public MovablePlatform(float radius, Color _color, float x, float y)
            : this(radius, _color)
        {
            PolyBody.Position = new Vector2(x, y);
        }

        public MovablePlatform(float mass, float radius, Color _color, float x, float y)
            : this(radius, _color)
        {
            PolyBody.Position = new Vector2(x, y);
            PolyBody.Mass = mass;
        }

        public MovablePlatform(float mass, float width, Color _color, float hieght, float x, float y)
            : this(mass, 1, _color, x, y)
        {
            (PolyBody).ChangeHieght(hieght);
            (PolyBody).ChangeWidth(width);
        }

        public override void setupGraphics()
        {
            NoTexture = true;
        }


        public override void setupPhysics()
        {
            var body = PolyBody;
            body.MakeRegularFromRad(Radius);
        }


        protected override string ParseStandardConstructor()
        {
            return String.Format(
                @"<float>{0}</float>
                  <float>{1}</float>
                  <Color>{2}</Color>
                  <float>{3}</float>
                  <float>{4}</float>
                  <float>{5}</float>",
                PolyBody.Mass, PolyBody.GetWidth(), color.ToVector3().ToPrettyString(), 
                PolyBody.GetHeight(), 
                PolyBody.Position.X, PolyBody.Position.Y);
        }
    }
}
