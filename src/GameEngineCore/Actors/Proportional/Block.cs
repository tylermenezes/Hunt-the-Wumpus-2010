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
    class Block : Actor
    {

        public const float DefaultScale = 1;

        public Block() : this(DefaultScale) { }
        public Block(float scale)
        {
            Scale = scale;
            PolyBody = new PolyBody(1, 4);
            this.PolyBody.Friction = .3f;
            Load();
        }

        public Block(float scale, float x, float y)
            : this(scale)
        {
            PolyBody.Position = new Vector2(x, y);
        }

        public Block(float mass, float scale, float x, float y)
            : this(scale)
        {
            PolyBody.Position = new Vector2(x, y);
            PolyBody.Mass = mass;
            PolyBody.GoToSleep();
        }

        public Block(float mass, float scale, float x, float y, float rot)
            : this(scale)
        {
            PolyBody.Position = new Vector2(x, y);
            PolyBody.Mass = mass;
            PolyBody.Rotation = rot;
        }

        public Block(float mass, float scale, float x, float y, float rot, float fric)
            : this(scale)
        {
            PolyBody.Position = new Vector2(x, y);
            PolyBody.Mass = mass;
            PolyBody.Rotation = rot;
            PolyBody.Friction = fric;
        }

        public override void setupGraphics()
        {
            Sprite = new Sprite("Block", State.Name, Scale);
        }

        public override void setupPhysics()
        {
            PolyBody.MakeRect(Sprite.BoundingBox().X, Sprite.BoundingBox().Y);
        }


        protected override string ParseStandardConstructor()
        {
            // float mass, float scale, float x, float y
            return String.Format(
                @"<float>{0}</float>
                 <float>{1}</float>
                 <float>{2}</float>
                 <float>{3}</float>",
                PolyBody.Mass, Scale, PolyBody.Position.X, PolyBody.Position.Y);
        }
    }
}
