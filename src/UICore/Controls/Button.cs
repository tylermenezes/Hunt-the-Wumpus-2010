using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HuntTheWumpus.UICore.Controls
{
    public class ImageButton : Control
    {
        private Texture2D _image;
        public Texture2D Image
        {
            get { return _image; }
            set
            {
                _image = value;
                this.ElementSize = new Vector2(_image.Width, _image.Height);
            }
        }
        public Color Tint { get; set; }

        private bool DeselectsAtLostControl;
        public readonly Color MouseOnColor = Color.Gray;
        public readonly Color MouseOffColor = Color.White;
        public ImageButton(Vector2 location)
            : base(location, Vector2.Zero)
        {
            Tint = MouseOffColor;
            this.MouseEnter += new MouseEventHandler(ImageButton_MouseEnter);
            this.MouseExit += new MouseEventHandler(ImageButton_MouseExit);
        }

        public void Initialize(Texture2D image)
        {
            Image = image;
        }
        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            if (_image == null)
                throw new Exception("Button is not initialized");
            spriteBatch.Draw(Image, Location, Tint);
            base.Draw(time, spriteBatch);
        }

        void ImageButton_MouseExit(GUIElement sender, MouseEventArgs e)
        {
            Tint = MouseOffColor;
        }

        void ImageButton_MouseEnter(GUIElement sender, MouseEventArgs e)
        {
            Tint = MouseOnColor;
            if (!DeselectsAtLostControl)
            {
                if (Parent is Frame)
                    ((Frame)Parent).LoseControl += new FrameEventHandler(ImageButton_LoseControl);
                DeselectsAtLostControl = true;
            }
        }
        void ImageButton_LoseControl(Frame sender)
        {
            Tint = MouseOffColor;
        }
    }
}
