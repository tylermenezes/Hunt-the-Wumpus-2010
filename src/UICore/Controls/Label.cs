using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.UICore.Controls
{
    public class Label : Control
    {
        public bool DynamiclyResize = true;
        private string _text;
        public String Text 
        { 
            get { return _text; }
            set
            {
                _text = value;
                if (DynamiclyResize)
                    ElementSize = _font.MeasureString(_text);
            }
        }

        private SpriteFont _font;
        public SpriteFont Font 
        {
            get { return _font; }
            set
            {
                _font = value;
                ElementSize = _font.MeasureString(_text);
            }
        }

        public Color ForeColor { get; set; }

        public Label(Vector2 location, string text)
            : base(location, Vector2.Zero) 
        {
            _text = text;
            ForeColor = Color.Black;
        }

        public virtual void Initialize(SpriteFont font)
        {
            _font = font;
            this.ElementSize = _font.MeasureString(_text);
        }

        public virtual void Initialize()
        {
            _font = (SpriteFont)ContentRepository.Content[
                (String)DefaultSettings.Settings["Font"]];
            this.ElementSize = _font.MeasureString(_text);
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            if (Font == null)
                throw new Exception("Label is not Initialized");
            spriteBatch.DrawString(_font, _text, Location, ForeColor);

            base.Draw(time, spriteBatch);
        }
    }
}
