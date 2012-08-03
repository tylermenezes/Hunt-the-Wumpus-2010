using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.UICore.Controls
{
    public delegate void LabelBoxEventHandler();
    public class LabelBox : Label
    {
        Keys[] textKeys = new Keys[] { 
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
            Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
            Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
            Keys.Z};

        Keys[] numKeys = new Keys[] {Keys.D0, Keys.D1, Keys.D2, Keys.D3,
            Keys.D4, Keys.D5, Keys.D6, Keys.D7,
            Keys.D8, Keys.D9};

        public int MaxLength { get; set; }

        public LabelBox(int maxLength) :
            base(Vector2.Zero, " ")
        {
            this.KeyUp += new KeyEventHandler(LabelBox_KeyUp);
            this.KeyDown += new KeyEventHandler(LabelBox_KeyDown);

            MaxLength = maxLength;
            this.DynamiclyResize = false;
        }

        public override void Initialize()
        {
            base.Initialize();
            this.Text = String.Empty;
            this.ElementSize.Width = MaxLength*this.ElementSize.Width;
        }
        private static string RepeatString(string str, int maxLength)
        {
            StringBuilder builder = new StringBuilder(str);
            for (int i = 0; i < maxLength; i++)
                builder.Append(str);
            return builder.ToString();
        }


        bool caps;
        void LabelBox_KeyDown(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.LeftShift) ||
                e.InterestingKeys.Contains<Keys>(Keys.RightShift))
                caps = true;
            else caps = false;
        }
        void LabelBox_KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains(Keys.Back) && this.Text != String.Empty)
                    Text = Text.Remove(this.Text.Length - 1);
            else if (e.InterestingKeys[0] == Keys.Space)
                this.Text += " ";
            else if (textKeys.Contains(e.InterestingKeys[0]) && this.Text.Length < MaxLength)
                this.Text += caps ? 
                    e.InterestingKeys[0].ToString() : 
                    e.InterestingKeys[0].ToString().ToLower();
            else if (numKeys.Contains(e.InterestingKeys[0]))
            {
                string tempText = e.InterestingKeys[0].ToString();
                this.Text += tempText.Remove(0, tempText.Length -1 );
            }

        }

    }
}
