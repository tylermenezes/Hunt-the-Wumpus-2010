using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HuntTheWumpus.Utilities;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HuntTheWumpus.PhysicsCore;

namespace HuntTheWumpus.GraphicsCore
{
    public class GraphicsEngine
    {
        static SpriteBatch _spriteBatch = (SpriteBatch)Globals.Variables["SpriteBatch"];
        private static Texture2D pixel = (Texture2D)ContentRepository.Content["PhysicsTestPixel"];
        public static Sprite[] Sprites { get; private set; }

        static float PixelsPerMeter;

        public static void Load()  //Loads Everything and Creates Sprites
        {
            AnimationRepository.LoadContent();
            Sprites = new Sprite[0];
            PixelsPerMeter = MathUtil.PixelsPerMeter;
        }

        public static void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

            double TotalTime = gameTime.ElapsedGameTime.TotalSeconds;
            foreach (Sprite sprite in Sprites) //Draws Each Sprite Created
            {
                sprite.currentAnimation().Update(TotalTime);
                _spriteBatch.Draw(sprite.currentAnimation().Filmstrip,
                    sprite.Position.P2C(),
                    sprite.currentAnimation().SourceRectangle,
                    Color.White,
                    -sprite.Roation,
                    new Vector2(sprite.currentAnimation().SourceRectangle.Width / 2, sprite.currentAnimation().SourceRectangle.Height / 2),
                    (PixelsPerMeter*sprite.Scale)/60f,
                    sprite.spriteEffects,
                    1);
                
                
            }
            Sprites = new Sprite[0]; // Clears the Graphics for the next step  
        }
        public static void AddSprite(Sprite sprite) //Add a sprite to the list to be drawn
        {
            Sprite[] temporaryArray = Sprites;
            Array.Resize(ref temporaryArray, temporaryArray.Length + 1);
            temporaryArray[temporaryArray.Length - 1] = sprite;
            Sprites = temporaryArray;
        }
        public static void ResetAnimations()
        {
            foreach (Sprite sprite in Sprites)
            {
                sprite.currentAnimation().CurrentFrame = 0;
            }

        }

        public static void DrawRectangle(Vector2 cent, float w, float h, float rot, Color c)
        {
            Rectangle rect = new Rectangle((int)(cent.X), (int)(cent.Y), (int)w, (int)h);
            _spriteBatch.Draw(pixel, rect, null, c, rot, new Vector2(.5f, .5f), SpriteEffects.None, 0f);
        }
        public static void DrawRectangeNoFill(Vector2 cent, float w, float h, Color c, float thick)
        {
            Vector2[] verts = {new Vector2(cent.X - (w/2), cent.Y + (w/2)), new Vector2(cent.X + (w/2), cent.Y + (w/2)), new Vector2(cent.X + (w/2), cent.Y - (w/2)), new Vector2(cent.X - (w/2), cent.Y - (w/2))} ;
            for (int i = 0; i < verts.Length; i++)
            {
                Vector2 p1 = verts[i].P2C();
                Vector2 p2 = verts[(i + 1) % verts.Length].P2C();
                DrawLine(p1, p2, c, thick);
            }
        }
        public static void DrawLine(Vector2 p1, Vector2 p2, Color c, float thick)
        {
            DrawRectangle(((p1 + p2) / 2), thick, (p1 - p2).Length(), -(float)Math.Atan((p2 - p1).X / (p2 - p1).Y), c);
        }
        public static void DrawArrow(Vector2 pos, Vector2 targ, Color c, float thick)
        {
            float LERP = .7f;
            float WID = (targ - pos).Length() / 4;

            DrawLine(pos, targ, c, thick);
            var p = Vector2.Lerp(pos, targ, LERP);
            var t = Vector2.Normalize(targ - pos).Transpose();
            var v1 = Vector2.Normalize(Vector2.Reflect(t, Vector2.UnitX));
            var v2 = Vector2.Normalize(Vector2.Reflect(t, Vector2.UnitY));
            var p1 = p + v1 * WID;
            var p2 = p + v2 * WID;

            DrawLine(pos, targ, c, thick);
            DrawLine(p1, targ, c, thick);
            DrawLine(p2, targ, c, thick);
        }
        public static void DrawPolygon(Polygon p, Color c, float thick)
        {
            var verts = p.GetTransformedVertices();
            for (int i = 0; i < verts.Length; i++)
            {
                Vector2 p1 = verts[i].P2C();
                Vector2 p2 = verts[(i + 1) % verts.Length].P2C();
                DrawLine(p1, p2, c, thick);
            }
        }
        public static void FillPolygon(Polygon p, Texture2D tex, Color c)
        {
            var ppm = MathUtil.PixelsPerMeter;
            var bb = p.GetBoundingBoxRaw();
            var orgin = new Vector2(-(float)bb.xmin * ppm, (float)bb.ymax * ppm);
            _spriteBatch.Draw(tex, p.Position.P2C(), null, c, -p.Rotation, orgin, 1f, SpriteEffects.None, 0f);
           
        }
        public static void DrawForces(Vector2 pos, List<Force> currForces, float thick, float scale) //scale = 1 will draw to scale. WARNING: looks really large
        {
            foreach (var f in currForces)
            {
                Color c;
                switch (f.t)
                {
                    case ForceType.Gravity:
                        c = Color.Green;
                        break;
                    case ForceType.Spring:
                        c = Color.Red;
                        break;
                    case ForceType.Drag:
                        c = Color.SaddleBrown;
                        break;
                    default:
                        throw new NotImplementedException("add a force color, yo!");
                }
                DrawArrow(pos.P2C(), (pos + scale * f.f).P2C(), c, thick);
            }
        }
    }
}
