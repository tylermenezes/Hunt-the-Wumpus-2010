using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using HuntTheWumpus.Utilities;
using HuntTheWumpus.UICore.Controls;
using HuntTheWumpus.UICore;


namespace HuntTheWumpus.UICore
{
    public class PopUpMenuControl : Control
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

        public Texture2D BackgroundImage;
        public PopUpMenuControl(string title, Vector2 location)
            : this(title, location, 0)
        {

        }

        public PopUpMenuControl(string title, Vector2 location, float outsideSpacing) :
            base(Vector2.Zero, Vector2.Zero)
        {
            _outsideSpacing = outsideSpacing; 
            Title = title;
            Location = location;

            Load();
        }

        public virtual void Load()
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

            this.MouseOver += new MouseEventHandler(PopUpMenuControl_MouseOver);
            FixMenuSize();
        }
        void PopUpMenuControl_MouseOver(GUIElement sender, MouseEventArgs e)
        {
            if (e.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                var deltaX = e.CurrentMouseState.X - e.PreviousMouseState.X;
                var deltaY = e.CurrentMouseState.Y - e.PreviousMouseState.Y;

                this.MoveAll(this.Location + new Vector2(deltaX, deltaY));
            }
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
            var location = Location;
            Location = Vector2.Zero;
            MoveAll(location);
            this.CenterAboutLocation(true);
        }
        void exitButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            this.Close();
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            if (BackgroundImage != null)
                spriteBatch.Draw(BackgroundImage,
                    new Rectangle((int)Location.X,
                        (int)Location.Y,
                        (int)ElementSize.X,
                        (int)ElementSize.Y),
                        Color.White);
            base.Draw(time, spriteBatch);
        }
    }
}