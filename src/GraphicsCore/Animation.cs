using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace HuntTheWumpus.GraphicsCore
{
    
    public class Animation
    {

        public double FrameRate { get; set; }
        public bool AnimationRunning { get; private set; }
        public bool IsLooping { get; private set; }
        public bool ReverseLooping { get; private set; }
        public bool IsRepeating { get; private set; } 
        public int FrameNumber { get; private set; }
        public int CurrentFrame { get;  set; }
        public float Scaler { get; private set; }
        private Rectangle _sourceRectangle;
        private Vector2 _sourceBox;
        double animFrameElapsed;
        public Texture2D Filmstrip { get; private set; }
        public Rectangle SourceRectangle { get { return _sourceRectangle; } }
        public Vector2 SourceBox { get { return _sourceBox; } }


        public bool IsRunning;
      

        public Animation(Texture2D filmstrip, int framenumber, double framerate)
        {
            Filmstrip = filmstrip;
            FrameNumber = framenumber;
            FrameRate = framerate;

            _sourceRectangle = new Rectangle(0, 0, filmstrip.Width / framenumber, filmstrip.Height);
            _sourceBox = new Vector2(filmstrip.Width / framenumber, filmstrip.Height);
            animFrameElapsed = 0;
            IsLooping = true;
            IsRepeating = true;


        }

        public Animation(Texture2D filmstrip, int framenumber, double framerate, bool islooping)
            : this(filmstrip, framenumber, framerate)
        {
            IsLooping = islooping;
        }
        public Animation(Texture2D filmstrip, int framenumber, double framerate, bool islooping, bool isrepeating)
            : this(filmstrip, framenumber, framerate, islooping)
        {
            IsRepeating = isrepeating;
        }

        public void Update(Double gameTime)
        {

            if (IsLooping)
            {


                animFrameElapsed += gameTime; //Says How Long the frame has bee going
                if (IsRepeating)
                {
                    if (animFrameElapsed >= FrameRate)
                    {

                        CurrentFrame = (CurrentFrame + 1) % FrameNumber;
                        //Changes the Frame
                        animFrameElapsed = 0;

                    }
                }
                if (!IsRepeating && (CurrentFrame != FrameNumber-1))
                {
                    if (animFrameElapsed >= FrameRate)
                    {

                        CurrentFrame = (CurrentFrame + 1);
                        animFrameElapsed = 0;

                    }


                }
               
                //Creates the current view of the spritesheet based on the crrent frame.    
                _sourceRectangle.X = CurrentFrame * _sourceRectangle.Width;

                if (!IsLooping && _sourceRectangle.X ==
                    Filmstrip.Width - _sourceRectangle.Width)
                {
                    IsRunning = false;
                }
            }

        }


        public void Reset()
        {
            _sourceRectangle.X = 0;
            animFrameElapsed = 0;
        }

    }
}
