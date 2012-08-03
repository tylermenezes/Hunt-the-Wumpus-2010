using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.UICore.Controls
{
    public class TrueFalseButton: Control
    {
        private Texture2D trueImage = (Texture2D) Utilities.ContentRepository.Content["TrueButton"];
        private Texture2D falseImage = (Texture2D) Utilities.ContentRepository.Content["FalseButton"];
        private Texture2D _image;

        public bool TrueOrFalse { get; set; }
        private void setImage()
        {
            if (TrueOrFalse)
                _image = trueImage;
            else
                _image = falseImage; 
        }

        public TrueFalseButton(bool trueOrFalse, Vector2 location) :
            base(location, Vector2.Zero)
        {
            this.TrueOrFalse = trueOrFalse;
            this.ElementSize = new Vector2(trueImage.Width, trueImage.Height);
            setImage();
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            setImage();
            spriteBatch.Draw(_image, Location, Color.White);
            base.Draw(time, spriteBatch);
        }

 
         
    }
}
