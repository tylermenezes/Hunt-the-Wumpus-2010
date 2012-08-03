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
    class HighScoreFrame : PopUpMenuFrame
    {
        private Label[] highScores;
        private const float spacingConstant = 20; 

        public HighScoreFrame(): base("HighScores", 10)
        {
            highScores = new Label[5]; 
        }

        public override void Load()
        {
            for (int i = 0; i < 5 && i < HighScoreManager.Singleton.HighScores.Count; i++)
            {
                var highscore = 
                    HighScoreManager.Singleton.HighScores.ElementAt<HighScore>(i);
                string text = String.Format("Name: {0} Score: {1}",
                    highscore.Name,
                    highscore.Score);
                highScores[i] = new Label(Vector2.UnitY * spacingConstant * i, text);
                highScores[i].Initialize();
                this.AddControl(highScores[i]); 
            }
            base.Load(); 
        }      

  


    }
}
