using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus.GameEngineCore
{
    struct HighScore
    {
        public int Score;
        public string Name;

        public HighScore(string name, int score)
        {
            Name = name;
            Score = score; 
        }
    }

    class HighScoreManager
    {
        public LinkedList<HighScore> HighScores;
        private static HighScoreManager _Singleton;
        public static HighScoreManager Singleton
        {
            get 
            {
                if (_Singleton == null)
                    _Singleton = new HighScoreManager();
                return _Singleton; 
            }
        }

        private HighScoreManager()
        {
            HighScores = new LinkedList<HighScore>(); 
        }

        public void RankScore(string name, int score) 
        {
            HighScore insertPoint = new HighScore("Null", 0);
            foreach (HighScore highScore in HighScores)
            {
                if (highScore.Score < score)
                {
                    insertPoint = highScore;
                    break; 
                }
            }
            if (insertPoint.Name == "Null")
                   HighScores.AddLast (new HighScore(name, score)); 
            else 
                HighScores.AddBefore (HighScores.Find (insertPoint), new HighScore (name, score)); 
        }

        


    }
}
