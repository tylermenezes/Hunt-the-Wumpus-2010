using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HuntTheWumpus.Utilities;
using HuntTheWumpus.UICore.Controls;
using HuntTheWumpus.UICore;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace HuntTheWumpus.UICore
{
    public class InputBoxFrame : PopUpMenuFrame
    {
        private string[] _inputs;
        private const float controlSpacing = .5f;

        public event FrameEventHandler SubmitInput;
        public Dictionary<String, LabelBox> InputFields
        {
            get;
            private set;
        }

         public InputBoxFrame(string title, string[] inputs)
            : base(title, 10)
        {
            _inputs = inputs;
            InputFields = new Dictionary<String, LabelBox>();
        }


        public override void Load()
        {
            for (int i = 0; i < _inputs.Length; i++)
            {
                LabelBox labelBox = new LabelBox(20);
                labelBox.Initialize();
                labelBox.Text = "";
                labelBox.Location +=
                    Vector2.UnitY * ((controlSpacing + labelBox.ElementSize.Height ) * (2 * i + 1));

                Label label = new Label(
                    new Vector2(0, (controlSpacing + labelBox.ElementSize.Height) * 2 * i), 
                    _inputs[i]+":");
                label.Initialize();
                
                this.AddControl(label);
                this.AddControl(labelBox);

                InputFields.Add(_inputs[i], labelBox);
            }
            
            this.KeyUp += new KeyEventHandler(UserProfileFrame_KeyUp);
            base.Load();
            
        }

        void UserProfileFrame_KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.Enter))
            {
                foreach (LabelBox labelBox in InputFields.Values)
                    if (labelBox.Text == String.Empty)
                        return;
                this.Exit();
                if (SubmitInput != null)
                    SubmitInput(this);
            }
        }
    }
}
