using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HuntTheWumpus.GameEngineCore;
using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.Utilities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HuntTheWumpus.GameEngineCore.Editor
{
    class Tile
    {
        public Vector2 Location { get; private set; }
        public Actor BlockContents
        {
            get
            {
                return _blockContents;
            }
            set
            {
                if (value == null)
                    _blockContents = null;
                else
                {
                    value.PolyBody.SetMasterPosition(Vector2.Add(Location * Tile.size, value.PolyBody.Position));
                    _blockContents = value;
                }
            }
        }
        //public Stack<Actor> ObjectContents
        //{ 
        //}

        private Actor _blockContents;
        private Stack<Actor> _objectContents = new Stack<Actor>();

        public static Size size = new Size(1,1);

        public Tile(Vector2 location)
        {
            Location = location;
            _objectContents.DefaultIfEmpty(null);
        }

        public void addObjectContent(Actor contents)
        {
            if (contents != null)
            {
                if (contents.IsFixed)
                    contents.PolyBody.SetMasterPosition(Vector2.Add(Location * Tile.size, contents.PolyBody.Position));
                else
                    contents.PolyBody.Position = Vector2.Add(Location * Tile.size, contents.PolyBody.Position);
                _objectContents.Push(contents);
            }
        }
        public Actor getTopObjectContents()
        {
            if (_objectContents.Count != 0)
                return _objectContents.Peek();
            else return null;
        }
        public List<Actor> getAllObjectsContents()
        {
            return _objectContents.ToList();
        }
        public void clearBlockContents()
        {
            BlockContents = null;
        }
        public void clearTopObjectContents()
        {
            if (_objectContents.Count != 0)
                _objectContents.Pop();
        }
        public void clearAllObjectContents()
        {
            _objectContents.Clear();
        }
        public void clearAllContents()
        {
            clearBlockContents();
            clearAllObjectContents();
        }

        public Vector2 getCenter()
        {
            return Vector2.Add(Location, (Vector2)Tile.size / 2f);
        }
    }
}
