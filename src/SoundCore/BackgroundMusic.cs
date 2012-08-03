using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuntTheWumpus.Utilities;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;


namespace HuntTheWumpus.SoundCore
{
    public class BackgroundMusic
    {
        float Volume;
        bool Fadeout;
        bool Fadein;
        SoundEffect balls;
        SoundEffectInstance doubleballs;
        //METHOD
        public void Update()
        {
            if (Fadeout)
            {
                    doubleballs.Volume -= .01f;
                if (doubleballs.Volume < 0.02f)
                {
                    doubleballs.Dispose();
                    Fadeout = false;
                }
            }
            if (Fadein)
            {
                doubleballs.Volume += .01f;
                if (doubleballs.Volume > .98f)
                {
                    
                    Fadein = false;
                }
            }
        }
        public SoundEffectInstance playSound(SoundEffect sound) //returns when sound is finished
        {
            SoundEffectInstance SoundPlayer = sound.CreateInstance();
            return SoundPlayer;

        }
        public void pickBackgroundMusic(int mapID)
        {
            switch (mapID)
            {
                    
                case 1:
                    balls = (SoundEffect)ContentRepository.Content["LevelOneMusic"];
                    
                    break;
                case 2:

                    break;

                default:
                    throw new Exception("No Sound for this map Id");
                    break;
            }
            
            doubleballs = balls.CreateInstance();
            doubleballs.IsLooped = true;
            
            fadeInCurrentBackgroundMusic();
            
        }
        public void endCurrentBackgroundMusic()
        {  
             doubleballs.Dispose();
        }
        public void fadeOutCurrentBackgroundMusic()
        {
            Fadeout = true;
        }
        public void fadeInCurrentBackgroundMusic()
        {
            
            
            doubleballs.Play();
            doubleballs.Volume = 0f;
            Fadein = true;
        }
        public void playCurrentBackgroundMusic()
        {
            doubleballs.Play();
        }

        
    }
}
