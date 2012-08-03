using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    class FixedBlock : Actor
    {
        public float Radius { get; private set; }

        public FixedBlock(float radius)
        {
            Radius = radius;
            PolyBody = new PolyBody(1, 4);
            PolyBody.IsFixed = true;
            Load();
        }

        public FixedBlock(float radius, float x, float y)
            : this(radius)
        {
            PolyBody.Position = new Vector2(x, y);
        }

        public FixedBlock(float mass, float radius, float x, float y)
            : this(radius)
        {
            PolyBody.Position = new Vector2(x, y);
            PolyBody.Mass = mass;
        }

        public FixedBlock(float mass, float width, float hieght, float x, float y)
            : this(mass, 1, x, y)
        {
            (PolyBody).ChangeHieght(hieght);
            (PolyBody).ChangeWidth(width);
        }

        public override void setupGraphics()
        {
            Sprite = new Sprite("Block", State.Name, Scale);
            NoTexture = true;
        }

        public override void setupPhysics()
        {
            PolyBody.MakeRect(Sprite.BoundingBox().X, Sprite.BoundingBox().Y);

        }

        protected override string ParseStandardConstructor()
        {
            return String.Format(
                @"<float>{0}</float>
                 <float>{1}</float>
                 <float>{2}</float>
                 <float>{3}</float>
                 <float>{4}</float>",
                PolyBody.Mass, 
                PolyBody.GetWidth(), PolyBody.GetHeight(), 
                PolyBody.Position.X, PolyBody.Position.Y);
        }
    }
}
