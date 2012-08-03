using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HuntTheWumpus.Utilities;
using HuntTheWumpus.UICore.Controls;
using HuntTheWumpus.UICore;


namespace HuntTheWumpus.UICore
{
    public class PopUpMenuFrame : Frame
    {
        public string Title { get; private set; }

        private float _outsideSpacing;
        public float OutsideSpacing
        {
            get
            {
                return _outsideSpacing;
            }
            set
            {
                this.ElementSize = this.ElementSize + (value - _outsideSpacing) * Vector2.One;
                _outsideSpacing = value;
            }
        }
        protected Label titleTextLabel;
        protected ImageButton exitButton; 
        protected const float CenterTextValue = 250;

        public PopUpMenuFrame(string title) :
            base(Vector2.Zero, Vector2.Zero, true)
        {
            Title = title;
        }

        public PopUpMenuFrame(string title, float outsideSpacing) :
            base(Vector2.Zero, Vector2.Zero, true)
        {
            _outsideSpacing = outsideSpacing;
            Title = title;
        }

        /// <summary>
        /// Place at end of Child's Load Method
        /// </summary>
        public override void Load()
        {
            exitButton = new ImageButton(Vector2.Zero);
            exitButton.Initialize((Texture2D)ContentRepository.Content["CloseButton"]);
            exitButton.MouseClick += new MouseEventHandler(exitButton_MouseClick);
            
            titleTextLabel = new Label(exitButton.ElementSize * Vector2.UnitX, Title);
            titleTextLabel.Initialize();

            MoveAll(this.Location + Vector2.UnitY * (
                (exitButton.ElementSize.Height > titleTextLabel.ElementSize.Height) ?
                exitButton.ElementSize.Height : titleTextLabel.ElementSize.Height));

            this.AddControl(exitButton);
            this.AddControl(titleTextLabel);

            this.BackgroundImage = (Texture2D)ContentRepository.Content["PhysicsTestPixel"];

            FixMenuSize();
            FixMenuLocation();
        }

        protected void FixMenuSize()
        {
            float ySize = 0;
            float xSize = 0;
            float yLocation = float.PositiveInfinity;
            float xLocation = float.PositiveInfinity;

            float outsideSpacing = _outsideSpacing;

            _outsideSpacing = 0;
            foreach (Control c in Controls)
            {
                if (c.Location.X < xLocation)
                    xLocation = c.Location.X;
                if (c.Location.Y < yLocation)
                    yLocation = c.Location.Y;
            }
            this.Location = new Vector2(xLocation, yLocation);
            foreach (Control c in Controls)
            {
                if (c.Location.Y + c.ElementSize.Y - this.Location.Y > ySize)
                    ySize = c.Location.Y + c.ElementSize.Y - this.Location.Y;
            }
           foreach (Control c in Controls)
           {
               if (c.Location.X < xLocation)
                   xLocation = c.Location.X;
               if (c.Location.Y < yLocation)
                   yLocation = c.Location.Y;
           }
           this.Location = new Vector2(xLocation, yLocation); 
           foreach (Control c in Controls)
           {
               if (c.Location.Y + c.ElementSize.Y - this.Location.Y > ySize)
                   ySize = c.Location.Y + c.ElementSize.Y - this.Location.Y; 
                if (c.Location.X + c.ElementSize.X - this.Location.X > xSize)
                    xSize = c.Location.X + c.ElementSize.X - this.Location.X;

               if (c.Location.X + c.ElementSize.X - this.Location.X > xSize)
                   xSize = c.Location.X + c.ElementSize.X - this.Location.X;
           }


           this.ElementSize = new Size(xSize, ySize);
           this.ElementSize = new Size(xSize, ySize);
           OutsideSpacing = outsideSpacing;
        }

        protected void FixMenuLocation()
        {
            this.MoveAll((Vector2)(Size)DefaultSettings.Settings["WindowSize"] / 2);
            this.CenterAboutLocation(true);
        }
        void exitButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            this.Exit();
        }

    }
}