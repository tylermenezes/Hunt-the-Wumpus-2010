using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HuntTheWumpus.Utilities;
using HuntTheWumpus.GameEngineCore;
using HuntTheWumpus.GraphicsCore;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.PhysicsCore;


namespace HuntTheWumpus.GameEngineCore.Editor
{
    class EditorEngine
    {
        Tile[,] Tiles;

        private float PixelsPerMeter = 0;
        private Size windowSizePixels = Vector2.Zero;
        private Vector2 TilesPerScreen;


        public bool showGrid = true;
        private Tile highlighted = null;

        public EditorEngine()
        {
        }

        public void Load()
        {
            PixelsPerMeter = (float)DefaultSettings.Settings["PixelsPerMeter"];
            windowSizePixels = (Size)DefaultSettings.Settings["WindowSize"];



            TilesPerScreen = windowSizePixels / (PixelsPerMeter * (Vector2)Tile.size);

            Tiles = new Tile[(int)TilesPerScreen.X + 1, (int)TilesPerScreen.Y + 1];

            for (int i = 0; i <= (int)TilesPerScreen.X; i++)
            {
                for (int j = 0; j <= (int)TilesPerScreen.Y; j++)
                {
                    Tiles[i, j] = new Tile(new Vector2(i, j));
                }
            }
        }

        public void ClearAllTiles()
        {
            foreach (Tile tile in Tiles)
            {
                tile.clearAllContents();
            }
        }

        public Tile MetersToTile(Vector2 meterPosition)
        {
            Vector2 rawLocation = meterPosition / Tile.size;
            return Tiles[(int)(rawLocation.X), (int)(rawLocation.Y)];
        }

        public Tile PixelsToTile(Vector2 pixelPosition)
        {
            return MetersToTile(pixelPosition.C2P());
        }
        public void highlightTile(Tile toHighlight)
        {
            highlighted = toHighlight;
        }


        private void CreateSprite(String ActorName, String ActorState, float scale, Vector2 position, float roation)
        {

            Sprite Actor = new Sprite(ActorName, ActorState, scale, position, roation);
            GraphicsEngine.AddSprite(Actor);
        }

        public void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            foreach (Tile tile in Tiles)
            {
                if (tile.BlockContents != null)
                    DrawActor(tile.BlockContents);
                if (tile.getTopObjectContents() != null)
                {
                    foreach (Actor actor in tile.getAllObjectsContents())
                    {
                        DrawActor(actor);
                    }
                }
            }
            if (showGrid)
            {
                for (int i = 0; i < TilesPerScreen.Y; i++)
                {
                    float height = i * Tile.size.Y * PixelsPerMeter;
                    GraphicsEngine.DrawLine(new Vector2(0, height), new Vector2(windowSizePixels.X, height), Color.SteelBlue, 2);
                }
                for (int i = 0; i < TilesPerScreen.X; i++)
                {
                    float width = i * Tile.size.X * PixelsPerMeter;
                    GraphicsEngine.DrawLine(new Vector2(width, 0), new Vector2(width, windowSizePixels.Y), Color.SteelBlue, 2);

                }
            }
            if (highlighted != null)
            {
                GraphicsEngine.DrawRectangeNoFill(highlighted.getCenter(), Tile.size.Width, Tile.size.Height, Color.Crimson, 2);
            }

            GraphicsEngine.Draw(time);
        }
        private void DrawPolygon(Polygon body, Actor actor)
        {
            actor.color = Color.Gray;
            GraphicsEngine.FillPolygon(body, ((PolyBody)body).tex, actor.color);
        }
        private void DrawActor(Actor actor)
        {
            if (actor.PolyBody.tex != null)
                DrawPolygon((PolyBody)actor.PolyBody, actor);
            else
                CreateSprite(actor.GetType().ToString(), 
                    actor.State.Name, 
                    actor.Scale, 
                    actor.PolyBody.Position, 
                    actor.PolyBody.Rotation); 
        }

        public List<Actor> ToList()
        {
            List<Actor> actors = new List<Actor>();
            foreach (Tile tile in Tiles)
            {
                if (tile.BlockContents != null)
                    actors.Add(tile.BlockContents);
            }
            actors.Add(null);
            foreach (Tile tile in Tiles)
            {
                foreach (Actor actor in tile.getAllObjectsContents())
                {
                    if (actor != null)
                        actors.Add(actor);
                }
            }
            return actors;
        }
    }
}
