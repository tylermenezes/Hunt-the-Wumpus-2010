using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HuntTheWumpus.UICore;
using HuntTheWumpus.Utilities;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace HuntTheWumpus.GameEngineCore.GameStates
{
    class MainMenuState: GameState
    {
        UIEngine guiManager = new UIEngine(); 

        public override void Load()
        {
            var mainMenuFrame = new Frames.MainMenuFrame();
            guiManager.AddAndLoad(mainMenuFrame);
            
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            guiManager.Draw(time, spriteBatch); 
        }

        public override void Update(GameTime time)
        {
            if (!guiManager.Update(time))
                this.Exit();
        }
    }
}