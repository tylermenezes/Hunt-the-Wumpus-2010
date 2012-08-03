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
    public class Sound
    {
        SoundEffect SoundFromFile;
        SoundEffectInstance InstanceOfSound;

        public void CreateSoundInstance(string SoundID)
        {
            switch (SoundID)
            {
                    
                case "finalMap1":
                    SoundFromFile = (SoundEffect)ContentRepository.Content["1-"];
                    InstanceOfSound = SoundFromFile.CreateInstance();
                    break;
                     
                case "finalMap2":
                    SoundFromFile = (SoundEffect)ContentRepository.Content["2-"];
                    InstanceOfSound = SoundFromFile.CreateInstance();
                    break;
                case "finalMap3":
                    SoundFromFile = (SoundEffect)ContentRepository.Content["3-"];
                    InstanceOfSound = SoundFromFile.CreateInstance();
                    break;
                case "4":
                    SoundFromFile = (SoundEffect)ContentRepository.Content["4-"];
                    InstanceOfSound = SoundFromFile.CreateInstance();
                    break;
                case "5":
                    SoundFromFile = (SoundEffect)ContentRepository.Content["5-"];
                    InstanceOfSound = SoundFromFile.CreateInstance();
                    break;
                case "6":
                    SoundFromFile = (SoundEffect)ContentRepository.Content["6-"];
                    InstanceOfSound = SoundFromFile.CreateInstance();
                    break;
                default:
                    //throw new Exception("No Sound for this ID");
                    break;
            }
        }
        public bool SoundPlaying()
        {
            if (InstanceOfSound.State == SoundState.Stopped)
                return false;
            else
                return true;
        }
        public void PlaySound()
        {
           
            InstanceOfSound.Play();
        }
        public void StopSound() {
            InstanceOfSound.Stop();
            

        }
    }
}
