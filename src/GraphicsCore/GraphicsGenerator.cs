using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HuntTheWumpus.PhysicsCore;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.Utilities;
using HuntTheWumpus.GameEngineCore;
using System.IO;

namespace HuntTheWumpus.GraphicsCore
{
    public static class GraphicsGenerator
    {
        public static Texture2D GenerateTexFromPolygon(Polygon p)
        {
            var ppm = MathUtil.PixelsPerMeter;

            var np = new PolyBody(1, p.RawVertices.Length);
            //np.RawVertices = p.GetRotatedVertices();
            np.RawVertices = p.RawVertices;
            np.Position = Vector2.Zero;
            np.Rotation = 0;

            var bb = np.GetBoundingBoxRaw();

            int w = (int)(bb.Width * ppm);
            int h = (int)(bb.Height * ppm);
            Color[] col = new Color[w * h];

            Debug.Trace("Generating textures...");

            /*for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    var xt = bb.xmin + x/ppm; //transform into polygon-space
                    var yt = bb.ymax - y/ppm;
                    var vt = new Vector2((float)xt, (float)yt);
                    if (CollisionEngine.TestPointInPoly(np, vt))
                    {
                        col[x + y * w] = Color.White;
                    }
                    else
                    {
                        col[x + y * w] = Color.TransparentBlack;
                    }
                }*/
            Enumerable.Range(0, w).PForEach(x =>
                {
                    for (int y = 0; y < h; y++)
                    {
                        var xt = bb.xmin + x / ppm; //transform into polygon-space
                        var yt = bb.ymax - y / ppm;
                        var vt = new Vector2((float)xt, (float)yt);
                        if (CollisionEngine.TestPointInPoly(np, vt))
                        {
                            col[x + y * w] = Color.White;
                        }
                        else
                        {
                            col[x + y * w] = Color.TransparentBlack;
                        }
                    }
                }
            );

            var sb = (SpriteBatch)(Globals.Variables["SpriteBatch"]);
            var dev = (GraphicsDevice)(Globals.Variables["GraphicsDevice"]);
            Texture2D tex = new Texture2D(dev, w, h, 1, TextureUsage.None, SurfaceFormat.Color);
            tex.SetData<Color>(col);
            
            /*MemoryStream texStream = new MemoryStream();
            texStream.BeginWrite();
            col.

            Texture2D tex = Texture2D.FromFile((GraphicsDevice)(Globals.Variables["GraphicsDevice"]), texStream);*/


            Debug.Trace("Done generating!");
            return tex;
        }
    }
}
