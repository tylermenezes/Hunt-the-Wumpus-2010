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
    public class MainMenuFrame: Frame
    {
        private TextButton newGameButton;
        private TextButton loadGameButton;
        private TextButton highScoreButton;
        private TextButton optionsButton;
        private TextButton instructionsButton;
        private TextButton exitButton; 

        private const float buttonSpacing = 5;
        private const float edgeSpacing = 20;

        public override void Load()
        {
            this.BackgroundImage = (Texture2D)ContentRepository.Content["TitleScreen"];
            exitButton = new TextButton(new Vector2(edgeSpacing, ElementSize.Y - edgeSpacing), "Exit");
            exitButton.Initialize(); 
            exitButton.Location = new Vector2(exitButton.Location.X, exitButton.Location.Y - exitButton.ElementSize.Y);
            exitButton.MouseClick += new MouseEventHandler(exitButton_MouseClick);
            this.AddControl(exitButton);

            instructionsButton = new TextButton(new Vector2(edgeSpacing, exitButton.Location.Y - buttonSpacing), "Instructions");
            instructionsButton.Initialize();
            instructionsButton.Location = new Vector2(instructionsButton.Location.X, instructionsButton.Location.Y - instructionsButton.ElementSize.Y);
            instructionsButton.MouseClick += new MouseEventHandler(instructionsButton_MouseClick);
            this.AddControl(instructionsButton);

            optionsButton = new TextButton(new Vector2(edgeSpacing, instructionsButton.Location.Y - buttonSpacing), "Options");
            optionsButton.Initialize();
            optionsButton.Location = new Vector2(optionsButton.Location.X, optionsButton.Location.Y - optionsButton.ElementSize.Y);
            optionsButton.MouseClick += new MouseEventHandler(optionsButton_MouseClick);
            this.AddControl(optionsButton);

            highScoreButton = new TextButton(new Vector2(edgeSpacing, optionsButton.Location.Y - buttonSpacing), "High Score");
            highScoreButton.Initialize(); 
            highScoreButton.Location = new Vector2(highScoreButton.Location.X, highScoreButton.Location.Y - highScoreButton.ElementSize.Y);
            highScoreButton.MouseClick += new MouseEventHandler(highScoreButton_MouseClick);
            this.AddControl(highScoreButton);

            loadGameButton = new TextButton(new Vector2(edgeSpacing, highScoreButton.Location.Y - buttonSpacing), "Load Game");
            loadGameButton.Initialize(); 
            loadGameButton.Location = new Vector2(loadGameButton.Location.X, loadGameButton.Location.Y - loadGameButton.ElementSize.Y);
            loadGameButton.MouseClick += new MouseEventHandler(loadGameButton_MouseClick);
            this.AddControl(loadGameButton);

            newGameButton = new TextButton(new Vector2(edgeSpacing, loadGameButton.Location.Y - buttonSpacing), "New Game");
            newGameButton.Initialize(); 
            newGameButton.Location = new Vector2(newGameButton.Location.X, newGameButton.Location.Y - newGameButton.ElementSize.Y);
            newGameButton.MouseClick += new MouseEventHandler(newGameButton_MouseClick);
            this.AddControl(newGameButton);

            this.KeyUp += new KeyEventHandler(MainMenuFrame_KeyUp);
            
        }

        void MainMenuFrame_KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.OemTilde))
                GameEngine.Singleton.AddAndLoad(new ConsoleState());
            else if (e.InterestingKeys.Contains<Keys>(Keys.E))
                GameEngine.Singleton.AddAndLoad(new EditorState());
        }

        void optionsButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            UIManager.AddAndLoad(new OptionsMenuFrame()); 
        }

        void loadGameButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            UIManager.AddAndLoad(new LoadGameMenuFrame());
        }

        void highScoreButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            UIManager.AddAndLoad(new HighScoreFrame()); 
        }

        void instructionsButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            UIManager.AddAndLoad(new InstructionsMenuFrame());
        }

        void newGameButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {

            // UNCOMMENT TO PICK MAP
            // GameEngine.Singleton.AddAndLoad(new PhysicsDemoState());
            // GameEngine.Singleton.AddAndLoad(new ConsoleState ());
            //  GameEngine.Singleton.AddAndLoad(new BouncerDemoState());
            //  GameEngine.Singleton.AddAndLoad(new MapDemoState());
            //  GameEngine.Singleton.AddAndLoad(new GraphicsDemoState());
            //  GameEngine.Singleton.AddAndLoad(new GravityManipulationDemo());
            // GameEngine.Singleton.AddAndLoad(new PuzzleDemoState());
            //  GameEngine.Singleton.AddAndLoad(new MapDemoState());
            // GameEngine.Singleton.AddAndLoad(new EditorState());
            // UIManager.AddAndLoad(new UserProfileFrame());

            InputBoxFrame userProfileFrame = new InputBoxFrame("New Game", new string[] { "Enter Profile Name" });
            userProfileFrame.SubmitInput += new FrameEventHandler(
                inputBox =>
                    GameEngine.Singleton.AddAndLoad(
                    new TheGameState(((InputBoxFrame)inputBox).InputFields["Enter Profile Name"].Text, true)));
            UIManager.AddAndLoad(userProfileFrame);

        }


        void exitButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            this.Exit();
        }


    }
}
