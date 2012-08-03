using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.GameEngineCore.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HuntTheWumpus.GameEngineCore.Editor
{
    class StandardObjects
    {
        public List<String> objectCodes = new List<String>();

        public void Load()
        {
            MethodInfo[] methods = this.GetType().GetMethods();
            foreach (MethodInfo method in methods)
            {
                if ((method.IsStatic) && (method.IsPublic))
                    objectCodes.Add(method.Name);
            }
        }

        public Actor create(int objectID)
        {
            MethodInfo method = this.GetType().GetMethod(objectCodes.ElementAt(objectID));
            Actor actor = (Actor)method.Invoke(null, null);
            actor.objectType = objectCodes.IndexOf(method.Name);
            return actor;
        }
        public Actor create(int objectID, string linkID, string address)
        {
            MethodInfo method = this.GetType().GetMethod(objectCodes.ElementAt(objectID));
            Actor actor = (Actor)method.Invoke(null, new object[]{linkID, address});
            actor.objectType = objectCodes.IndexOf(method.Name);
            return actor;
        }

        //   _ _ _ _
        //  |       |
        //  |       |
        //  |       |
        //  |_ _ _ _|
        private static Actor newBlock(float Width, float Height, Vector2 position)
        {
            Actor block = new FixedBlock(1);
            block.PolyBody.MakeRect(Width, Height);
            block.PolyBody.SetMasterPosition(position);
            block.PolyBody.tex = GraphicsGenerator.GenerateTexFromPolygon(block.PolyBody);
            block.color = Color.Gray;
            return block;
        }
        private static Actor newBlock(Vector2[] Vertices)
        {
            Actor block = new FixedBlock(1);
            block.PolyBody.ConstructFromVertices(Vertices);
            block.PolyBody.SetMasterPosition(block.PolyBody.Position);
            block.PolyBody.tex = GraphicsGenerator.GenerateTexFromPolygon(block.PolyBody);
            block.color = Color.Gray;
            return block;
        }

        #region Tiles

        //   _ _ _ _
        //  |# # # #|
        //  |# # # #|
        //  |# # # #|
        //  |#_# #_#|
        public static Actor RegularBlock()
        {
            Actor block = newBlock(Tile.size.Width, Tile.size.Height,(Vector2)Tile.size / 2f);
            return block;
        }

        //   _ _ _ _
        //  |# #    |
        //  |# #    |
        //  |# #    |
        //  |#_#_ _ |
        public static Actor HalfBlockLeft()
        {
            Actor block = newBlock(Tile.size.Width / 2, Tile.size.Height, new Vector2(Tile.size.X / 4, Tile.size.Y / 2));
            return block;
        }

        //   _ _ _ _
        //  |    # #|
        //  |    # #|
        //  |    # #|
        //  |_ _ #_#|
        public static Actor HalfBlockRight()
        {
            Actor block = newBlock(Tile.size.Width / 2, Tile.size.Height, new Vector2(Tile.size.X * 3 / 4, Tile.size.Y / 2));
            return block;
        }

        //   _ _ _ _
        //  |# # # #|
        //  |# # # #|
        //  |       |
        //  |_ _ _ _|
        public static Actor HalfBlockUp()
        {
            Actor block = newBlock(Tile.size.Width, Tile.size.Height / 2, new Vector2(Tile.size.X / 2, Tile.size.Y * 3 / 4));
            return block;
        }

        //   _ _ _ _
        //  |       |
        //  |       |
        //  |# # # #|
        //  |#_# #_#|
        public static Actor HalfBlockDown()
        {
            Actor block = newBlock(Tile.size.Width, Tile.size.Height / 2, new Vector2(Tile.size.X / 2, Tile.size.Y / 4));
            return block;
        }
        
        //   _ _ _ _
        //  |#      |
        //  |# #    |
        //  |# # #  |
        //  |#_#_#_#|
        public static Actor TriangleBlockLeft()
        {
            Vector2[] Vertices = { new Vector2(0, 0), new Vector2(0, Tile.size.Y), new Vector2(Tile.size.X, 0) };
            Actor block = newBlock(Vertices);
            return block;
        }

        //   _ _ _ _
        //  |# # # #|
        //  |  # # #|
        //  |    # #|
        //  |_ _ _ #|
        public static Actor TriangleBlockRight()
        {
            Vector2[] Vertices = { new Vector2(Tile.size.X, 0), new Vector2(Tile.size.X, Tile.size.Y), new Vector2(0, Tile.size.Y) };
            Actor block = newBlock(Vertices);
            return block;

        }

        //   _ _ _ _
        //  |# # # #|
        //  |# # #  |
        //  |# #    |
        //  |#_ _ _ |
        public static Actor TriangleBlockUp()
        {
            Vector2[] Vertices = { new Vector2(0, 0), new Vector2(Tile.size.X, Tile.size.Y), new Vector2(0, Tile.size.Y) };
            Actor block = newBlock(Vertices);
            return block;
        }

        //   _ _ _ _
        //  |      #|
        //  |    # #|
        //  |  # # #|
        //  |#_#_#_#|
        public static Actor TriangleBlockDown()
        {
            Vector2[] Vertices = { new Vector2(0, 0), new Vector2(Tile.size.X, Tile.size.Y), new Vector2(Tile.size.X, 0) };
            Actor block = newBlock(Vertices);
            return block;
        }

        //   _ _ _ _
        //  |       |
        //  |       |
        //  |# #    |
        //  |#_#_ _ |
        public static Actor QuarterBlockLeft()
        {
            Actor block = newBlock(Tile.size.Width / 4, Tile.size.Height / 4, new Vector2(Tile.size.X / 4, Tile.size.Y / 4));
            return block;
        }

        #endregion

        #region Objects

        public static Actor Block()
        {
            Actor block = new Block(1, .2f, Tile.size.X/2, Tile.size.Y/2);
            return block;
        }

        public static Actor PhysicsHatGravity()
        {
            Actor hat = new PhysicsHat("GravityGrenade", Tile.size.X / 2, Tile.size.Y / 2);
            return hat;
        }

        public static Actor PhysicsHatFriction()
        {
            Actor hat = new PhysicsHat("FrictionFlamethrower", Tile.size.X / 2, Tile.size.Y / 2);
            return hat;
        }

        public static Actor PhysicsHatTime()
        {
            Actor hat = new PhysicsHat("TimeTrident", Tile.size.X / 2, Tile.size.Y / 2);
            return hat;
        }

        public static Actor PhysicsHatLight()
        {
            Actor hat = new PhysicsHat("LightLance", Tile.size.X / 2, Tile.size.Y / 2);
            return hat;
        }
        public static Actor PhysicsHatElectromagnetism()
        {
            Actor hat = new PhysicsHat("ElectromagneticWhatever", Tile.size.X / 2, Tile.size.Y / 2);
            return hat;
        }

        public static Actor InterPortal(string portalID, string address)
        {
            Actor portal = new InterMapPortal(portalID, address, Tile.size.X / 2, Tile.size.Y / 2);
            return portal;
        }

        public static Actor IntraPortal(string portalID, string address)
        {
            Actor portal = new IntraMapPortal(portalID, address, Tile.size.X / 2, Tile.size.Y / 2);
            return portal;
        }

        #endregion

    }
}
