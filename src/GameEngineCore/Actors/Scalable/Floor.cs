using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuntTheWumpus.PhysicsCore;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.Utilities;
namespace HuntTheWumpus.GameEngineCore.Actors
{
    class Floor : Actor
    {
        public float Width { get; private set; }
        public float Height { get; private set; }

        public Floor(float width)
        {
            Width = width;
            Height = .5f; // Constant
            PolyBody = new PolyBody(1, 4);
            color = Color.Gray;
            Load();
        }
        public Floor(float width, Color _color)
            :this(width)
        {
            color = _color;
        }
        public Floor(float width, Color _color, float x, float y)
            : this(width, _color)
        {
            PolyBody.SetMasterPosition(new Vector2(x, y));
        }

        public override void setupGraphics()
        {
            NoTexture = true;
        }

        public override void setupPhysics()
        {
            PolyBody.MakeRect(Width, Height, new Vector2 (Width /2, Height / 2));
            PolyBody.IsFixed = true;
            
        }


        protected override string ParseStandardConstructor()
        {
            //float width, Color _color, float x, float y
            return String.Format(
                @"<float>{0}</float>
                  <Color>{1}</Color>
                  <float>{2}</float>
                  <float>{3}</float>", 
                                     Width, color.ToVector3().ToPrettyString(), PolyBody.Position.X, PolyBody.Position.Y);
        }
    }
}
