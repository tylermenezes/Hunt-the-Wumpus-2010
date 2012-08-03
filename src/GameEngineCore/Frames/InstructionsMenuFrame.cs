using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HuntTheWumpus.GameEngineCore.GameStates;
using HuntTheWumpus.Utilities;
using HuntTheWumpus.UICore.Controls;
using HuntTheWumpus.UICore;

namespace HuntTheWumpus.GameEngineCore.Frames
{
    class InstructionsMenuFrame: PopUpMenuFrame
    {
        private Label instructionsLabel;

        public InstructionsMenuFrame():
            base("Instructions", 10) { }

        public override void Load()
        {
            instructionsLabel = new Label(Vector2.Zero,
                "A: Move Left\nD: Move Right\nW: Increase Power\nS: Decrease Power\nSpace: Jump\nShift: Enter Portal\nClick: Fire Weapon\nScroll Wheel: Switch Weapon\n\nGoal: Hunt the Wumpus ");
            instructionsLabel.Initialize();
            this.AddControl(instructionsLabel);
            this.BackgroundImage = (Texture2D)ContentRepository.Content["PhysicsTestPixel"];
            base.Load();
        }
        
    }
}
