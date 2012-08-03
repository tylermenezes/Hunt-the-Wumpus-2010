using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.UICore;
using HuntTheWumpus.Utilities;
using HuntTheWumpus.UICore.Controls;
namespace HuntTheWumpus.GameEngineCore.GameStates
{
    class ConsoleState : GameState
    {
        

        Label debugText = new Label(new Vector2(20, 10), "");

        private const string commandChar = "]";

        UIEngine _consoleSystem;

        string text = "";

        Keys[] keysToCheck = new Keys[] { 
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
            Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
            Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
            Keys.Z, Keys.Back, Keys.Space,
            Keys.D0, Keys.D1, Keys.D2, Keys.D3,
            Keys.D4, Keys.D5, Keys.D6, Keys.D7,
            Keys.D8, Keys.D9};

        KeyboardState currentKeyboardState;
        KeyboardState lastKeyboardState;




        // Core Functions

        public ConsoleState()
        {
            _consoleSystem = new UIEngine();
            
        }
        public override void Update(Microsoft.Xna.Framework.GameTime time)
        {
            if (!_consoleSystem.Update(time))
                this.Exit();
            currentKeyboardState = Keyboard.GetState();

            foreach (Keys key in keysToCheck)
            {
                if (CheckKey(key))
                {
                    AddKeyToText(key);
                    break;
                }
            }

            lastKeyboardState = currentKeyboardState;

            debugText.Text = text;
        }
        public override void Draw(Microsoft.Xna.Framework.GameTime time, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            _consoleSystem.Draw(time, spriteBatch);
        }
        public override void Destroy()
        {
            _consoleSystem.Destroy();
        }

        public override void Load()
        {
            text = commandChar + " ";
            InitUI();
            GraphicsEngine.Load();
        }


        // Init Functions

        private void InitGame()
        {
        }
        void InitUI()
        {
            _consoleSystem.AddFrame(new Frame());
            

            debugText.Initialize((SpriteFont)ContentRepository.Content["ConsoleText"]);
            debugText.ForeColor = Color.Black;
            _consoleSystem.ActiveFrame.AddControl(debugText);

            _consoleSystem.ActiveFrame.KeyUp += new KeyEventHandler(KeyUp);
            _consoleSystem.ActiveFrame.KeyDown += new KeyEventHandler(KeyDown);

        }

        bool isCaps = false;
        void KeyDown(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.LeftShift) ||
                e.InterestingKeys.Contains<Keys>(Keys.RightShift))
                isCaps = true;
            else isCaps = false;
        }

        // Helper Functions
        public void ParseText()
        {
            try
            {
                string[] lines = text.Split('\n');
                string line = lines[lines.Length - 1];
                line = line.Substring(line.IndexOf(commandChar) + 2);

                for (int i = 0; i < line.Length; i++)
                {
                    if (line.Substring(i, 1) != " ")
                    {
                        line = line.Substring(i);
                        break;
                    }
                }

                string[] args;
                try
                {
                    args = line.Substring(line.IndexOf(" ") + 1).Split(' ');
                }
                catch
                {
                    args = new string[0];
                }

                int firstSpace = line.IndexOf(" ");
                string cmd;

                if (firstSpace < 0)
                {
                    cmd = line;
                }
                else
                {
                    cmd = line.Substring(0, firstSpace);
                }

                text += "\n\n";

                switch (cmd)
                {
                    case "help":
                        text += "Commands:";
                        text += "\n     loadstate [state]       - Loads [state]";
                        text += "\n     pushstate [state]       - Pushes [state] onto the state stack.";
                        text += "\n     popstate [n]            - Pops [n] states off the game.";
                        text += "\n     openmap [map] [portal]  - Opens [map] and spawns the player at [portal] ! Case Sensitive !";
                        text += "\n     resetstate              - Resets the current state to new.";
                        text += "\n     clear                   - Clears the console";
                        text += "\n     exit                    - Closes the console";
                        text += "\n     help                    - Shows this help message";
                        break;

                    case "exit":
                        GameEngine.Singleton.RemoveState();
                        break;

                    case "clear":
                        text = "";
                        break;

                    case "loadstate":
                    case "load":
                        GameState s;

                        try
                        {
                            s = getStateFromName(args[0]);
                        }
                        catch
                        {
                            text += args[0] + " is not a valid GameState.";
                            break;
                        }

                        GameEngine.Singleton.RemoveStates(2);
                        GameEngine.Singleton.AddAndLoad(s);
                        break;

                    case "pushstate":
                    case "push":
                        GameState p;
                        try
                        {
                            p = getStateFromName(args[0]);
                        }
                        catch
                        {
                            text += args[0] + " is not a valid GameState.";
                            break;
                        }
                        GameEngine.Singleton.RemoveState();
                        GameEngine.Singleton.AddAndLoad(p);
                        GameEngine.Singleton.AddState(this);
                        break;

                    case "popstate":
                    case "pop":
                        int statesToPop;
                        try
                        {
                            statesToPop = int.Parse(args[0]);
                        }
                        catch
                        {
                            text += "Argument must be a number";
                            break;
                        }

                        if (statesToPop < 1)
                        {
                            text += "Argument must be a natural number.";
                            break;
                        }

                        try
                        {
                            GameEngine.Singleton.RemoveStates(statesToPop + 1); // Account for console being a state.
                            GameEngine.Singleton.AddState(this);
                        }
                        catch
                        {
                            text += "There are not that many states in the game.";
                            break;
                        }

                        break;

                    case "resetstate":
                    case "reset":
                        GameEngine.Singleton.RemoveState();
                        GameState g = GameEngine.Singleton.ActiveState;
                        string stateName = g.GetType().FullName;
                        GameEngine.Singleton.RemoveState();
                        GameEngine.Singleton.AddAndLoad((GameState)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(stateName));
                        break;
                    case "openmap":
                        var game = GameEngine.Singleton.GetPlayState();
                        game.ActiveProfile.SetCurrentMap(args[0], args[1], true);
                        this.Exit();
                        break;
                    case "everybody":
                        text += "Shots!, Shots!, Shots!, Shots!, Shots!, Shots!, Shots!, Shots!, Shots!, Shots! " ;
                        break;

                    default:
                        text += "Command '" + cmd + "' not recognized. Type 'help' for a list of all commands.";
                        break;
                }

                text += "\n\n" + commandChar + " ";
            }
            catch (Exception e)
            {
                text += "Invalid Input.\n" + "] ";
            }


        }

        private static GameState getStateFromName(string name) {
            GameState g;
            g = (GameState)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("HuntTheWumpus.GameEngineCore.GameStates." + name, true);
            if(g == null) {
                g = (GameState)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("HuntTheWumpus.GameEngineCore.GameStates." + name + "State", true);
            }
            return g;
        }

        // Events
        void KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.Escape))
                this.Exit();
            if (e.InterestingKeys.Contains<Keys>(Keys.Enter))
                ParseText();
           
        }
       
        //Reload

        public void Reload()
        {
            
        }
        private void AddKeyToText(Keys key)
        {
            string newChar = "";


            if (key == Keys.Back)
            {
                if (text.Length != 0)
                    text = text.Remove(text.Length - 1);
                return;
            }

            var keyName = Enum.GetName(typeof(Keys), key);
            if (keyName.ToLower() == "space")
            {
                text += " ";
            }else if(keyName.ToLower().Substring(0, 1) == "d" && keyName.Length == 2) {
                text += keyName.Substring(1);
            }
            else
            {
                text += (isCaps) ? keyName : keyName.ToLower();
            }

            if (currentKeyboardState.IsKeyDown(Keys.RightShift) ||
                currentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                newChar = newChar.ToUpper();
            }
            text += newChar;
        }

        private bool CheckKey(Keys theKey)
        {
            return lastKeyboardState.IsKeyDown(theKey) && currentKeyboardState.IsKeyUp(theKey);
        }


    }
}
