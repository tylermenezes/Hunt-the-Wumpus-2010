using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace HuntTheWumpus.Utilities
{
    public static class ContentRepository
    {
        public static Dictionary<string, Object> Content { get; private set;}
        static ContentRepository()
        {
            Content = new Dictionary<string,object>();
        }

        public static void LoadContent(ContentManager manager)
        {
            Content.Add("TextButton",
                manager.Load<SpriteFont>("Ingelby"));
            Content.Add("Kootenay",
                manager.Load<SpriteFont>("Kootenay"));
            Content.Add("Title",
                manager.Load<SpriteFont>("Title"));
            Content.Add("Text",
                manager.Load<SpriteFont>("IngelbySmall"));
            Content.Add("ConsoleText",
                manager.Load<SpriteFont>("ProggyClean"));
            Content.Add("ExitButton",
                manager.Load<Texture2D>("Art/UI/ExitButton"));
            Content.Add("NewGameButton",
                manager.Load<Texture2D>("Art/UI/NewGameButton"));
            Content.Add("CloseButton",
                manager.Load<Texture2D>("Art/UI/xbutton2"));
            Content.Add("TitleScreen",
                manager.Load<Texture2D>("Art/UI/TitleScreen"));
            Content.Add("PhysicsTestPixel",
                manager.Load<Texture2D>("Art/PhysicsTestPixel"));
            Content.Add("GraphicsTest",
                manager.Load<Texture2D>("Art/Actors/MainPlayer/GraphicsTest"));
            Content.Add("EndGame",
                manager.Load<Texture2D>("Art/UI/button_endgame"));
            Content.Add("NewGame",
                manager.Load<Texture2D>("Art/UI/button_newgame"));
            Content.Add("MainPlayer",
                manager.Load<Texture2D>("Art/Actors/MainPlayer/protagonist_idle"));
            Content.Add("MainPlayerRunning",
                manager.Load<Texture2D>("Art/Actors/MainPlayer/protagonist_run"));
            Content.Add("MainPlayerJumping",
                manager.Load<Texture2D>("Art/Actors/MainPlayer/protagonist_jump"));
            Content.Add("GravityGrenade",
                manager.Load<Texture2D>("Art/Actors/MainPlayer/tool_glauncher_grenade"));
            Content.Add("FlameBall",
                manager.Load<Texture2D>("Art/Actors/MainPlayer/tool_flamethrower_fire"));

            Content.Add("MainPlayerFireRun",
    manager.Load<Texture2D>("Art/Actors/MainPlayer/protagonist_run_flamethrower"));
            Content.Add("MainPlayerFireJump",
manager.Load<Texture2D>("Art/Actors/MainPlayer/protagonist_jump_flamethrower"));


            Content.Add("EnterButton", 
                manager.Load<Texture2D>("Art/UI/EnterButton"));
            Content.Add("Energy",
                manager.Load<Texture2D>("Art/Actors/Static/energy"));
            Content.Add("Brick",
                manager.Load<Texture2D>("Art/Actors/Static/brick"));
            Content.Add("Block",
                manager.Load<Texture2D>("Art/Actors/Static/block"));
            Content.Add("Crate",
                 manager.Load<Texture2D>("Art/Actors/Static/crate_big"));
            Content.Add("Dimondblock",
                manager.Load<Texture2D>("Art/Actors/Static/Dimondblock"));
            Content.Add("Platform",
                manager.Load<Texture2D>("Art/Actors/Static/platform"));
            Content.Add("Bouncer",
                manager.Load<Texture2D>("Art/Actors/Static/bouncer"));
            Content.Add("Portal",
                manager.Load<Texture2D>("Art/Actors/Static/Portal"));
            Content.Add("Portalexit",
                manager.Load<Texture2D>("Art/Actors/Static/Portalexit"));
            Content.Add("none",
                manager.Load<Texture2D>("Art/Actors/Static/null"));
            Content.Add("TrueButton", 
                manager.Load<Texture2D>("Art/UI/TrueButton")); 
            Content.Add("FalseButton", 
                manager.Load<Texture2D>("Art/UI/FalseButton"));
            Content.Add("EnergyBar",
                manager.Load<Texture2D>("Art/UI/EnergyBar"));
            Content.Add("PowerBar",
                manager.Load<Texture2D>("Art/UI/PowerBar"));
            Content.Add("LevelOneMusic",
                manager.Load<SoundEffect>("Sound/Music/1"));
            Content.Add("TEST",
                 manager.Load<SoundEffect>("tada"));

            #region voice
            Content.Add("1-",
                manager.Load<SoundEffect>("Sound/Voice/1"));
            
            Content.Add("2-",
                manager.Load<SoundEffect>("Sound/Voice/2"));

            Content.Add("3-",
                manager.Load<SoundEffect>("Sound/Voice/3"));

            Content.Add("4-",
                manager.Load<SoundEffect>("Sound/Voice/4"));

            Content.Add("5-",
                manager.Load<SoundEffect>("Sound/Voice/5"));

            Content.Add("6-",
                manager.Load<SoundEffect>("Sound/Voice/6"));

            #endregion
            #region flashbackimage
            Content.Add("Flashback1",
                manager.Load<Texture2D>("Art/Flashbacks/FB1Clean"));
            Content.Add("Flashback2",
                manager.Load<Texture2D>("Art/Flashbacks/FB1Clean"));
            Content.Add("Flashback3",
                manager.Load<Texture2D>("Art/Flashbacks/flashbackhospital"));
            Content.Add("Flashback4",
                manager.Load<Texture2D>("Art/Flashbacks/flashbackhospital"));
            Content.Add("Flashback5",
                manager.Load<Texture2D>("Art/Flashbacks/flashbackhospital"));
            Content.Add("Flashback6",
                manager.Load<Texture2D>("Art/Flashbacks/flashbackhospital"));
            Content.Add("BackGround",
                manager.Load<Texture2D>("Art/background"));
            #endregion
            //Content.Add("Tada",
            //    manager.Load<SoundEffect>("tada"));
        }


    }
}
