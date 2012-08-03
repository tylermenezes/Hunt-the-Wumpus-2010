using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.GameEngineCore
{
    /// <summary>
    /// Defines the state of the game, what its functionality is.
    /// </summary>
    public abstract class GameState
    {
        public bool Transparent { get; protected set; }
        public bool PleaseDestroy { get; set; }

        /// <summary>
        /// Setup GameState Here
        /// </summary>
        public abstract void Load();
        public abstract void Update(GameTime time);
        public abstract void Draw(GameTime time, SpriteBatch spriteBatch);
        public virtual void Destroy() { }

        public virtual void Exit()
        {
            PleaseDestroy = true;
        }
    }
}
