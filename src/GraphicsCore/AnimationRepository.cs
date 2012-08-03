using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.GraphicsCore
{
    public static class AnimationRepository
    {
        public static Dictionary<string, Object> Animations { get; set; }
        static AnimationRepository()
        {
            Animations = new Dictionary<string, object>();
        }
        public static void LoadContent()
        {
            //ActorName-ActorState
            Animations.Clear();
            Animations.Add("none",
               addAnimation((Texture2D)ContentRepository.Content["none"], 1, .2, false));
            Animations.Add("Player-Static",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayer"], 1, .1, false));
            Animations.Add("Player-Walking",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayerRunning"], 6, .04));
            Animations.Add("Player-Jumping",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayerJumping"], 5, .05, true, false));
            Animations.Add("Player-Standing",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayer"], 1, .2, false));


            Animations.Add("Player-Static-",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayer"], 1, .1, false));
            Animations.Add("Player-Walking-",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayerRunning"], 6, .04));
            Animations.Add("Player-Jumping-",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayerJumping"], 5, .05, true, false));
            Animations.Add("Player-Standing-",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayer"], 1, .2, false));

            Animations.Add("Player-Static-GravityGrenade",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayer"], 1, .1, false));
            Animations.Add("Player-Walking-GravityGrenade",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayerRunning"], 6, .04));
            Animations.Add("Player-Jumping-GravityGrenade",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayerJumping"], 5, .05, true, false));
            Animations.Add("Player-Standing-GravityGrenade",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayer"], 1, .2, false));

            Animations.Add("Player-Static-FrictionFlamethrower",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayer"], 1, .1, false));
            Animations.Add("Player-Walking-FrictionFlamethrower",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayerFireRun"], 6, .04));
            Animations.Add("Player-Jumping-FrictionFlamethrower",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayerFireJump"], 5, .05, true, false));
            Animations.Add("Player-Standing-FrictionFlamethrower",
                addAnimation((Texture2D)ContentRepository.Content["MainPlayer"], 1, .2, false));



            Animations.Add("Block-Static",
                addAnimation((Texture2D)ContentRepository.Content["Crate"], 1, .2, false));
            Animations.Add("Block-",
                addAnimation((Texture2D)ContentRepository.Content["Crate"], 1, .2, false));
            Animations.Add("Bouncer-Static",
                addAnimation((Texture2D)ContentRepository.Content["Bouncer"], 1, .2, false));
            Animations.Add("Portal-Static",
                addAnimation((Texture2D)ContentRepository.Content["Block"], 1, .2, false));
            Animations.Add("FromArtPlatform-Static",
                addAnimation((Texture2D)ContentRepository.Content["Platform"], 1, .2, false));
            Animations.Add("FromArtPlatform-",
                addAnimation((Texture2D)ContentRepository.Content["Platform"], 1, .2, false));

            Animations.Add("IntraMapPortal-Static",
                addAnimation((Texture2D)ContentRepository.Content["Portal"], 1, .2, false));
            Animations.Add("InterMapPortal-Static",
                addAnimation((Texture2D)ContentRepository.Content["Portalexit"], 1, .2, false));
            Animations.Add("PhysicsHat-Static",
                addAnimation((Texture2D)ContentRepository.Content["Energy"], 1, .2, false));
            Animations.Add("GravityGrenade-Static",
                addAnimation((Texture2D)ContentRepository.Content["GravityGrenade"], 4, .2));


            Animations.Add("FlameBall-Static", addAnimation((Texture2D)ContentRepository.Content["FlameBall"], 2, .02));
            #region Flashbacks
            Animations.Add("finalMap2-",
                addAnimation((Texture2D)ContentRepository.Content["Flashback1"], 1, .2, false));
            Animations.Add("2-",
                addAnimation((Texture2D)ContentRepository.Content["Flashback2"], 1, .2, false));
            Animations.Add("3-",
                addAnimation((Texture2D)ContentRepository.Content["Flashback3"], 1, .2, false));
            Animations.Add("4-",
                addAnimation((Texture2D)ContentRepository.Content["Flashback4"], 1, .2, false));
            Animations.Add("5-",
                addAnimation((Texture2D)ContentRepository.Content["Flashback5"], 1, .2, false));
            Animations.Add("6-",
                addAnimation((Texture2D)ContentRepository.Content["Flashback6"], 1, .2, false));
            #endregion
            Animations.Add("Background-Background",
                addAnimation((Texture2D)ContentRepository.Content["BackGround"], 1, .2, false));
             
            
        }

        private static object addAnimation(Texture2D texture2D, int p, double p2)
        {
            Animation animation = new Animation(texture2D, p, p2);
            return animation;
        }
        private static object addAnimation(Texture2D texture2D, int p, double p2, Boolean loop)
        {
            Animation animation = new Animation(texture2D, p, p2, loop);
            return animation;
        }
        private static object addAnimation(Texture2D texture2D, int p, double p2, Boolean loop, Boolean repeat)
        {
            Animation animation = new Animation(texture2D, p, p2, loop, repeat);
            return animation;
        }



    }
}
