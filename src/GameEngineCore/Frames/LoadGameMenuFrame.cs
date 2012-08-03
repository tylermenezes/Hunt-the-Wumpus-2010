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
    class LoadGameMenuFrame: PopUpMenuFrame
    {
        private Label[] loadGameLabels;
        private const float spacingConstant = 20;
        private const int MaxSavedGames = 10;
        public LoadGameMenuFrame():
            base ("Load Game", 10)
        {
            loadGameLabels = new Label[MaxSavedGames];
        }

        public override void Load()
        {
            var SaveGamesDirectory = (string)Utilities.Globals.Variables["SaveGamesDirectory"];
            string[] savedGames =
                System.IO.Directory.GetFiles(SaveGamesDirectory);

            for (int i = 0; i < MaxSavedGames; i++)
            {
                if (i < savedGames.Length)
                {
                    loadGameLabels[i] =
                        new Label(Vector2.UnitY * spacingConstant * i,
                            GetFileName(savedGames[i]));
                    loadGameLabels[i].MouseClick += new MouseEventHandler(SelectGame);
                }
                else
                    loadGameLabels[i] =
                        new Label(Vector2.UnitY * spacingConstant * i,
                            "Empty Save Slot");
                    loadGameLabels[i].Initialize();
                this.AddControl(loadGameLabels[i]);
            }
            base.Load();

        }

        void SelectGame(GUIElement sender, MouseEventArgs e)
        {
            var label = (Label)sender;
            GameEngine.Singleton.AddAndLoad(new TheGameState(label.Text, false));
        }

        private string GetFileName(string path)
        {
            int startOfName = 0;
            int endOfName = 0;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '\\' ||
                    path[i] == '/')
                    startOfName = i + 1;
                if (path[i] == '.')
                    endOfName = i;
            }
            return path.Substring(startOfName, endOfName - startOfName);
        }
    }
}
