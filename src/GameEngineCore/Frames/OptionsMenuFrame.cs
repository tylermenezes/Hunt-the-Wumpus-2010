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
    class OptionsMenuFrame: PopUpMenuFrame
    {
        private Label TriviaQuestions;
        private Label WindowSizeX;
        private Label WindowSizeY;
        private Label PixelsPerMeter; 

        private TrueFalseButton trueFalseButton;
        private LabelBox WindowSizeXBox;
        private LabelBox WindowSizeYBox;
        private LabelBox PixelsPerMeterBox;

        private ImageButton EnterButton;
            
        private const float spacingConstant = 40;
        private const float spacingConstant2 = 250;

        private PopUpMenuFrame userMessageFrame; 

        public OptionsMenuFrame()
            : base ("Options")
        {
            TriviaQuestions = new Label(Vector2.UnitY * spacingConstant, "Add Trivia Questions: ");
            WindowSizeX = new Label(Vector2.UnitY * 2 * spacingConstant, "Window Size - X: ");
            WindowSizeY = new Label(Vector2.UnitY * 3 * spacingConstant, "Window Size - Y: ");
            PixelsPerMeter = new Label(Vector2.UnitY * 4 * spacingConstant, "Pixels Per Meter: "); 

            trueFalseButton = new TrueFalseButton(false, Vector2.Zero); 
            WindowSizeXBox = new LabelBox(3);
            WindowSizeYBox = new LabelBox(3);
            PixelsPerMeterBox = new LabelBox(2);

            EnterButton = new ImageButton(Vector2.Zero);

            userMessageFrame = new PopUpMenuFrame("Please Restart Game After Closing This Window \n Or Else, Your Changed Settings Will Not Occur"); 
        }

        public override void Load()
        {
            TriviaQuestions.Initialize();
            trueFalseButton.Location = new Vector2(TriviaQuestions.Location.X + spacingConstant2, TriviaQuestions.Location.Y);
            trueFalseButton.MouseClick += new MouseEventHandler(trueFalseButton_MouseClick);
            this.AddControl(TriviaQuestions);
            this.AddControl(trueFalseButton);


            Size currentWindowSize = (Size) Utilities.DefaultSettings.Settings["WindowSize"];

            WindowSizeX.Initialize();
            WindowSizeXBox.Initialize();
            WindowSizeXBox.Text = currentWindowSize.X.ToString(); 
            WindowSizeXBox.Location = new Vector2(WindowSizeX.Location.X + spacingConstant2, WindowSizeX.Location.Y); 
            this.AddControl(WindowSizeX);
            this.AddControl(WindowSizeXBox);

            WindowSizeY.Initialize();
            WindowSizeYBox.Initialize();
            WindowSizeYBox.Text = currentWindowSize.Y.ToString();
            WindowSizeYBox.Location = new Vector2(WindowSizeY.Location.X + spacingConstant2, WindowSizeY.Location.Y); 
            this.AddControl(WindowSizeY);
            this.AddControl(WindowSizeYBox);

            PixelsPerMeter.Initialize();
            PixelsPerMeterBox.Initialize();
            PixelsPerMeterBox.Text = Utilities.DefaultSettings.Settings["PixelsPerMeter"].ToString();
            PixelsPerMeterBox.Location = new Vector2(PixelsPerMeter.Location.X + spacingConstant2, PixelsPerMeter.Location.Y); 
            this.AddControl(PixelsPerMeter);
            this.AddControl(PixelsPerMeterBox);

            EnterButton.Initialize((Texture2D)Utilities.ContentRepository.Content["EnterButton"]);
            EnterButton.Location = new Vector2(PixelsPerMeterBox.Location.X + PixelsPerMeter.ElementSize.X - EnterButton.ElementSize.X, PixelsPerMeter.Location.Y + spacingConstant);
            EnterButton.MouseClick += new MouseEventHandler(EnterButton_MouseClick);
            this.AddControl(EnterButton);
            

            base.Load();
        }

        void trueFalseButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            if (trueFalseButton.TrueOrFalse)
                trueFalseButton.TrueOrFalse = false;
            else
                trueFalseButton.TrueOrFalse = true;
        }

        void EnterButton_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            Size previousWindowSize = (Size)Utilities.DefaultSettings.Settings["WindowSize"];
            float previousPixelsPerMeterValue = (float) Utilities.DefaultSettings.Settings["PixelsPerMeter"];
            bool previousTriviaQuestionsValue = (bool) Utilities.DefaultSettings.Settings["Trivia Questions"];

            bool triviaQuestionsValue;
            float windowSizeXValue;
            float windowSizeYValue;
            float pixelsPerMeterValue;

            if (trueFalseButton.TrueOrFalse)
                triviaQuestionsValue = true;
            else
                triviaQuestionsValue = false;

            windowSizeXValue = float.Parse(WindowSizeXBox.Text);
            windowSizeYValue = float.Parse(WindowSizeYBox.Text);
            pixelsPerMeterValue = float.Parse(PixelsPerMeterBox.Text);
            Utilities.DefaultSettings.Settings["WindowSize"] = new Size(windowSizeXValue,windowSizeYValue); 
            Utilities.DefaultSettings.Settings["PixelsPerMeter"] =  pixelsPerMeterValue;

            bool changedSettings = false;

            if (pixelsPerMeterValue != previousPixelsPerMeterValue
                || triviaQuestionsValue != previousTriviaQuestionsValue
                || windowSizeXValue != previousWindowSize.X
                || windowSizeYValue != previousWindowSize.Y)
                changedSettings = true;
           if (changedSettings)
           {
               this.Exit();
               UIManager.AddAndLoad(userMessageFrame);
           }
           this.Exit();
        }
    }
}
