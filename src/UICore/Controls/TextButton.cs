using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HuntTheWumpus.GameEngineCore; 

using HuntTheWumpus.Utilities;
using HuntTheWumpus.UICore.Controls;
namespace HuntTheWumpus.UICore.Controls
{
    public class TextButton : Label
    {
        public readonly Color MouseOnColor = Color.WhiteSmoke;
        public readonly Color MouseOffColor = Color.Gray;
        private bool DeselectsAtLostControl;
        public TextButton(Vector2 location, string text)
            : base(location, text)  { }

        public override void Initialize()
        {
            base.Initialize((SpriteFont)ContentRepository.Content["TextButton"]);
            this.MouseEnter += new MouseEventHandler(TextButton_MouseEnter);
            this.MouseExit += new MouseEventHandler(TextButton_MouseExit);
            
            ForeColor = MouseOffColor;
        }

        void TextButton_MouseExit(GUIElement sender, MouseEventArgs e)
        {
            ForeColor = MouseOffColor;
        }

        void TextButton_MouseEnter(GUIElement sender, MouseEventArgs e)
        {
            ForeColor = MouseOnColor;
            if (!DeselectsAtLostControl)
            {
                if (Parent is Frame)
                    ((Frame)Parent).LoseControl += new FrameEventHandler(TextButton_LoseControl);
                DeselectsAtLostControl = true;
            }
        }
        void TextButton_LoseControl(Frame sender)
        {
            ForeColor = MouseOffColor;
        }
    }
}
