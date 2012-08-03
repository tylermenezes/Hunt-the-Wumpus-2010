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

namespace HuntTheWumpus.GameEngineCore.Editor
{
    class EditorConsole : PopUpMenuControl
    {
        /// <summary>
        /// Object that will be created on mouse click
        /// </summary>
        public string ActiveObject { get; private set; }
        public bool SetActiveObject(string name)
        {
            if (true
                //StandardObjects.IsValidObjectType(name)
                )
            {
                ActiveObject = name;
                return true;
            }
            
            return false;
        }
        public EditorConsole(Vector2 location)
            : base("Enter Object Name:", location, 10)
        {
            ActiveObject = String.Empty;
        }

        private void AcceptCommand()
        {
            if (SetActiveObject(CommandBox.Text))
                CommandBox.ForeColor = AcceptedColor;
            else
                CommandBox.ForeColor = RejectedColor;
        }

        private LabelBox CommandBox;
        private readonly Color AcceptedColor = Color.Blue;
        private readonly Color RejectedColor = Color.Red;
        private readonly Color NuetralColor = Color.Black;
        public override void Load()
        {
            CommandBox = new LabelBox(25);
            CommandBox.Initialize();
            CommandBox.Text = String.Empty;
            this.AddControl(CommandBox);

            this.KeyUp += new KeyEventHandler(EditorConsole_KeyUp);
            this.Selected += new ControlEventHandler(EditorConsole_Selected);
            this.Deselected += new ControlEventHandler(EditorConsole_Deselected);
            base.Load();
        }

        void EditorConsole_Deselected(Control sender)
        {
            CommandBox.Text = ActiveObject;
            CommandBox.ForeColor = AcceptedColor;
        }

        void EditorConsole_Selected(Control sender)
        {
            NuetralizeConsole();
            CommandBox.Text = String.Empty;
        }

        void EditorConsole_KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.Enter))
            {
                AcceptCommand();
            }
            else NuetralizeConsole();
            if (e.InterestingKeys.Contains<Keys>(Keys.Escape))
                CommandBox.Text = String.Empty;
        }

        private void NuetralizeConsole()
        {
            CommandBox.ForeColor = NuetralColor;
        }
    }
}
