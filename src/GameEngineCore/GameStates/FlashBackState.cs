using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.UICore;
using HuntTheWumpus.Utilities;
using HuntTheWumpus.UICore.Controls;
using HuntTheWumpus.SoundCore;

namespace HuntTheWumpus.GameEngineCore.GameStates
{
    public class FlashBackState : GameState
    {
        Sound FlashBackSound;
        Sprite FlashBackBackground;
        UIEngine UIManager;
        
        public FlashBackState(string ID)
        {
            UIManager = new UIEngine();
            UIManager.AddAndLoad(new Frame());
            UIManager.ActiveFrame.KeyUp+=new KeyEventHandler(ActiveFrame_KeyUp);
            FlashBackSound = new Sound();
            FlashBackSound.CreateSoundInstance(ID);
            FlashBackSound.PlaySound();
            FlashBackBackground = new Sprite(ID, "", 1f, (((Vector2)(Size)DefaultSettings.Settings["WindowSize"])/2f).C2P(), 0f);

        }
        public override void Load()
        {

        }
        public override void Update(Microsoft.Xna.Framework.GameTime time)
        {
            UIManager.Update(time);
            if (!FlashBackSound.SoundPlaying())
            {
                FlashBackSound.StopSound();
                this.Exit();
            }

        }

        void ActiveFrame_KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Length > 0) {
                this.Exit();
            }
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            GraphicsEngine.AddSprite(FlashBackBackground);
            GraphicsEngine.Draw(time);
           
        }

    }
}
