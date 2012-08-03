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
    public class PauseMenuFrame: PopUpMenuFrame 
    {
        private TextButton helpButton;
        private TextButton continueButton;
        private TextButton quitButton;
        private const float spacingConstant = 40; 
        
        public PauseMenuFrame():
            base("Paused Game") 
        {
            continueButton = new TextButton (Vector2.UnitY * spacingConstant,"Continue"); 
            helpButton = new TextButton (Vector2.UnitY * spacingConstant * 2, "Help");
            quitButton = new TextButton(Vector2.UnitY * spacingConstant * 3, "Quit"); 
        }

        public override void Load()
        {
            continueButton.Initialize();
            this.AddControl(continueButton); 
            continueButton.MouseClick +=new MouseEventHandler(continueButton_MouseClick);

            helpButton.Initialize(); 
            this.AddControl(helpButton); 
            helpButton.MouseClick+=new MouseEventHandler(helpButton_MouseClick);

            quitButton.Initialize(); 
            this.AddControl(quitButton); 
            quitButton.MouseClick+=new MouseEventHandler(quitButton_MouseClick);
            base.Load();
        }

        void quitButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            GameEngine.Singleton.DestroyAndRemoveStates(2);
        }

        void helpButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            InstructionsMenuFrame instructionsFrame = new InstructionsMenuFrame();
            UIManager.AddAndLoad(instructionsFrame); 
        }


        void continueButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            this.Exit(); 
        }
        

    }
}
