using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using HuntTheWumpus.GameEngineCore.GameStates;
using HuntTheWumpus.Utilities;
using HuntTheWumpus.UICore.Controls;
using HuntTheWumpus.UICore;


namespace HuntTheWumpus.GameEngineCore.Frames
{
    class UserProfileFrame : UICore.PopUpMenuFrame
    {
        private LabelBox labelBox;
        public UserProfileFrame()
            : base("Enter User Name", 10)
        {
        }

        public override void Load()
        {
            labelBox = new LabelBox(10);
            labelBox.Initialize();
            labelBox.Location = Vector2.Zero;
            labelBox.Text = "PLAYER1";
            this.AddControl(labelBox);

            this.KeyUp += new KeyEventHandler(UserProfileFrame_KeyUp);
            base.Load();
            
        }

        void UserProfileFrame_KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (labelBox.Text != String.Empty && 
                e.InterestingKeys.Contains<Keys>(Keys.Enter))
            {
                this.Exit();

            }
        }
    }
}
