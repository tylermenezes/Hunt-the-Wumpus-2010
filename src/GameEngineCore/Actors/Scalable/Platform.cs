using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    /// <summary>
    /// Basically a Fixed Block
    /// </summary>
    class Platform : Actor
    {
        public float Length { get; private set; }
        public float Width { get; private set; }

        public static float DefaultThickness = .5f;

        public Platform(float length)
        {
            Length = length;
            Width = DefaultThickness;
            PolyBody = new PolyBody(1, 4);
            PolyBody.IsFixed = true;
            Load();
        }

        public Platform(float length, float width)
        {
            Length = length;
            Width = width;
            Width = DefaultThickness;
            PolyBody = new PolyBody(1, 4);
            PolyBody.IsFixed = true;
            Load();
        }

        public Platform(float length, Color _color)
            : this(length)
        {
            color = _color;
        }

        public Platform(float length, Color _color, float x, float y)
            : this(length, _color)
        {
            PolyBody.SetMasterPosition(new Vector2(x, y));
        }
        public Platform(float length, Color _color, float x, float y, float rot)
            : this(length, _color)
        {
            PolyBody.SetMasterPosition(new Vector2(x, y));
            PolyBody.SetMasterRotation(rot);
        }
        public Platform(float length, float width, Color _color, float x, float y, float rot)
            : this(length, width)
        {
            PolyBody.SetMasterPosition(new Vector2(x, y));
            PolyBody.SetMasterRotation(rot);
            this.color = _color;
        }
        public Platform(float length, float width, Color _color, float x, float y, float rot, float friction)
            : this(length, width)
        {
            PolyBody.SetMasterPosition(new Vector2(x, y));
            PolyBody.SetMasterRotation(rot);
            PolyBody.Friction = friction;
            this.color = _color;
        }
        public override void setupGraphics()
        {
            NoTexture = true;
        }

        public override void setupPhysics()
        {
            PolyBody.MakeRect(Length, Width, this.PolyBody.Position);
        }


        protected override string ParseStandardConstructor()
        {
            return String.Format(
                @"<float>{0}</float>
                  <Color>{1}</Color>
                  <float>{2}</float>
                  <float>{3}</float>",
                  Length, color.ToVector3().ToPrettyString(), PolyBody.Position.X, PolyBody.Position.Y);
        }

        public static Platform GeneratePlatform(Vector2 p1, Vector2 p2, float thick, Color c, float friction)
        {
            var len = (p1 - p2).Length();
            var cent = (p1 + p2) / 2;
            Platform p = new Platform(len, thick, c, cent.X, cent.Y, (float)Math.Atan((p2 - p1).Y / (p2 - p1).X), friction);
            return p;
        }
    }
}
