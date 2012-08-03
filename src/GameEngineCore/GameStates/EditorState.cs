using System;
using System.Linq;
using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.UICore;
using HuntTheWumpus.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HuntTheWumpus.GameEngineCore.Editor;
using HuntTheWumpus.GameEngineCore.Actors;
using System.Collections.Generic;
using HuntTheWumpus.UICore.Controls;
using HuntTheWumpus.GameEngineCore.Frames;

//Added a lot of stuff, mainly for objects in the editor.
//Save files now include objects, not only blocks.
//Press Tab to go into Block edit mode, and shift to go into
//object edit mode. The only issue right now is that there are
//no standard objects. Yet. Also, even if there were, you can't draw them.
//Also also, a list of current editor controls:

//(Universal)
//Tab ------------Tile(Block) edit mode
//Left Shift -----Object edit mode

//(Tile edit mode)

//    Q - New regular block
//    E - Erase tile
//    1 - Half block mode
//    2 - Triangle mode
//    W - Up facing block
//    A - Left facing block
//    S - Down facing block
//    D - Right facing block

//    (Object edit mode)
//    Not implemented yet.

//    Also, save files are now in
//    XNAWumpus/bin/x86/debug/level.rptr. This can be opened with notepad.

namespace HuntTheWumpus.GameEngineCore.GameStates
{
    class EditorState : GameState
    {
        private Size size;

        public UIEngine UIManager;
        public GraphicsEngine GraphicsManager;
        public EditorEngine EditorManager;
        public StandardObjects ObjectCreator;

        Keys[] keysToCheck = new Keys[] { 
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
            Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
            Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
            Keys.Z, Keys.Back, Keys.Space};

        KeyEventArgs lastKeyboardState;
        MouseState currentMouseState;

        private String BlockType = "HalfBlock";
        private String ObjectType = "Block";
        private bool BlockMode = true;

        public override void Load()
        {
            MathUtil.Init((float)DefaultSettings.Settings["PixelsPerMeter"]);
            size = ((Vector2)(Size)Utilities.DefaultSettings.Settings["WindowSize"]) / MathUtil.PixelsPerMeter;

            UIManager = new UIEngine();
            GraphicsManager = new GraphicsEngine();
            EditorManager = new EditorEngine();
            ObjectCreator = new StandardObjects();

            EditorManager.Load();
            GraphicsEngine.Load();
            ObjectCreator.Load();

            InitUI();
        }

        private void InitUI()
        {
            UIManager.AddFrame(new Frame());

            UIManager.ActiveFrame.KeyUp += new KeyEventHandler(KeyUp);
            UIManager.ActiveFrame.MouseOver += new MouseEventHandler(MouseOver);
            UIManager.ActiveFrame.MouseClick += new MouseEventHandler(MouseClick);

            // This is how events should be handled
            UIManager.ActiveFrame.KeyDown += new KeyEventHandler(ActiveFrame_KeyDown);
        }

        void MouseOver(GUIElement sender, MouseEventArgs e)
        {
            currentMouseState = e.CurrentMouseState;
            EditorManager.highlightTile(EditorManager.PixelsToTile(new Vector2(e.CurrentMouseState.X, e.CurrentMouseState.Y)));
        }

        void KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (UIManager.ActiveFrame.ActiveControl == null)
            {
                if (BlockMode)
                {
                    if (e.InterestingKeys.Contains<Keys>(Keys.D1))
                    {
                        BlockType = "HalfBlock";
                    }
                    else if (e.InterestingKeys.Contains<Keys>(Keys.D2))
                    {
                        BlockType = "TriangleBlock";
                    }
                }
                else
                {
                    if (e.InterestingKeys.Contains<Keys>(Keys.D1))
                    {
                        ObjectType = "Block";
                    }
                    if (e.InterestingKeys.Contains<Keys>(Keys.D2))
                    {
                        ObjectType = "PhysicsHatGravity";
                    }
                    if (e.InterestingKeys.Contains<Keys>(Keys.D3))
                    {
                        ObjectType = "InterPortal";
                    }
                    if (e.InterestingKeys.Contains<Keys>(Keys.D4))
                    {
                        ObjectType = "IntraPortal";
                    }
                }
                if (e.InterestingKeys.Contains<Keys>(Keys.G))
                {
                    if (EditorManager.showGrid == false)
                        EditorManager.showGrid = true;
                    else
                        EditorManager.showGrid = false;
                }
                else if (e.InterestingKeys.Contains<Keys>(Keys.Tab))
                {
                    BlockMode = true;
                }
                else if (e.InterestingKeys.Contains<Keys>(Keys.LeftShift))
                {
                    BlockMode = false;
                }
                else if (e.InterestingKeys.Contains<Keys>(Keys.C) &&
                    !UIManager.ActiveFrame.Controls.Any<Control>(c => c is EditorConsole))
                {
                    UIManager.ActiveFrame.AddControl(
                        new EditorConsole(
                            new Vector2(currentMouseState.X, currentMouseState.Y)));
                }
                else if (e.InterestingKeys.Contains<Keys>(Keys.LeftControl))
                {
                    FileIO.SaveToFile(EditorManager.ToList());
                }
                else if (e.InterestingKeys.Contains<Keys>(Keys.RightControl))
                {
                    List<Actor> actors = FileIO.LoadFromFile();
                    EditorManager.ClearAllTiles();
                    bool loadTiles = true;
                    foreach (Actor actor in actors)
                    {
                        if (actor == null)
                            loadTiles = false;
                        else
                        {
                            if (loadTiles)
                                EditorManager.MetersToTile(actor.PolyBody.Position).BlockContents = ObjectCreator.create(actor.objectType);
                            else
                            {
                                if ((actor.objectType == ObjectCreator.objectCodes.IndexOf("InterPortal")) || (actor.objectType == ObjectCreator.objectCodes.IndexOf("IntraPortal")))
                                    EditorManager.MetersToTile(actor.PolyBody.Position).addObjectContent(ObjectCreator.create(actor.objectType, ((ILinked)actor).ID, ((ILinked)actor).PartnerAddress));
                                else
                                    EditorManager.MetersToTile(actor.PolyBody.Position).addObjectContent(ObjectCreator.create(actor.objectType));
                            }
                        }
                    }
                }
                else if (e.InterestingKeys.Contains<Keys>(Keys.C) &&
                        !UIManager.ActiveFrame.Controls.Any<Control>(c => c is EditorConsole))
                {
                    UIManager.ActiveFrame.AddControl(
                        new EditorConsole(
                            new Vector2(currentMouseState.X, currentMouseState.Y)));
                }
                else if (e.InterestingKeys.Contains<Keys>(Keys.LeftControl))
                {
                    FileIO.SaveToFile(EditorManager.ToList());
                }
                else if (e.InterestingKeys.Contains<Keys>(Keys.RightControl))
                {
                    List<Actor> actors = FileIO.LoadFromFile();
                    EditorManager.ClearAllTiles();
                    foreach (Actor actor in actors)
                    {
                        EditorManager.MetersToTile(actor.PolyBody.Position).addObjectContent(ObjectCreator.create(actor.objectType));
                    }
                }
                else if (e.InterestingKeys.Contains<Keys>(Keys.Escape))
                    this.Exit();
                else if (e.InterestingKeys.Contains<Keys>(Keys.OemTilde))
                    GameEngine.Singleton.AddAndLoad(new ConsoleState());
            }
        }

        void MouseClick(GUIElement sender, MouseEventArgs e)
        {
            if (!BlockMode)
            {
                if ((ObjectType == "InterPortal") || (ObjectType == "IntraPortal"))
                {
                    InputBoxFrame portalInputBox = new InputBoxFrame("New " + ObjectType, new string[] { "Portal ID", "ID of Portal To Link To" });
                    portalInputBox.SubmitInput += new FrameEventHandler(
                        inputBox =>
                            EditorManager.PixelsToTile(new Vector2(currentMouseState.X, currentMouseState.Y)).addObjectContent(ObjectCreator.create(ObjectCreator.objectCodes.IndexOf(ObjectType), ((InputBoxFrame)inputBox).InputFields["Portal ID"].Text, ((InputBoxFrame)inputBox).InputFields["ID of Portal To Link To"].Text)));
                    UIManager.AddAndLoad(portalInputBox);
                }
                else
                {
                    EditorManager.PixelsToTile(new Vector2(currentMouseState.X, currentMouseState.Y)).addObjectContent(ObjectCreator.create(ObjectCreator.objectCodes.IndexOf(ObjectType)));
                }
            }
        }

        public override void Update(GameTime time)
        {
            UIManager.Update(time);

            // WE HAVE A GUIENGINE! See InitUI Function
            //currentKeyboardState = Keyboard.GetState();

            //foreach (Keys key in keysToCheck)
            //{
            //    if (CheckKey(key))
            //    {
            //        KeyPressed(key);
            //        break;
            //    }
            //}
        }

        void ActiveFrame_KeyDown(GUIElement sender, KeyEventArgs e)
        {
            // Only look at the first key, because only one event can happen at a time
            // hence: e.InterestingKeys[0]
            if (UIManager.ActiveFrame.ActiveControl == null &&
                    keysToCheck.Contains<Keys>(e.InterestingKeys[0]))
                KeyPressed(e.InterestingKeys[0]);
            lastKeyboardState = e;
        }

        private void KeyPressed(Keys key)
        {
            if (BlockMode)
            {
                switch (key)
                {
                    case Keys.Q:
                        EditorManager.PixelsToTile(new Vector2(currentMouseState.X, currentMouseState.Y)).BlockContents = ObjectCreator.create(ObjectCreator.objectCodes.IndexOf("RegularBlock"));
                        break;
                    case Keys.E:
                        EditorManager.PixelsToTile(new Vector2(currentMouseState.X, currentMouseState.Y)).BlockContents = null;
                        break;
                    case Keys.A:
                        EditorManager.PixelsToTile(new Vector2(currentMouseState.X, currentMouseState.Y)).BlockContents = ObjectCreator.create(ObjectCreator.objectCodes.IndexOf(BlockType + "Left"));
                        break;
                    case Keys.D:
                        EditorManager.PixelsToTile(new Vector2(currentMouseState.X, currentMouseState.Y)).BlockContents = ObjectCreator.create(ObjectCreator.objectCodes.IndexOf(BlockType + "Right"));
                        break;
                    case Keys.S:
                        EditorManager.PixelsToTile(new Vector2(currentMouseState.X, currentMouseState.Y)).BlockContents = ObjectCreator.create(ObjectCreator.objectCodes.IndexOf(BlockType + "Down"));
                        break;
                    case Keys.W:
                        EditorManager.PixelsToTile(new Vector2(currentMouseState.X, currentMouseState.Y)).BlockContents = ObjectCreator.create(ObjectCreator.objectCodes.IndexOf(BlockType + "Up"));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (key)
                {
                    case Keys.E:
                        EditorManager.PixelsToTile(new Vector2(currentMouseState.X, currentMouseState.Y)).clearTopObjectContents();
                        break;
                    default:
                        break;
                }
            }
        }

        private bool CheckKey(Keys theKey)
        {
            return lastKeyboardState.InterestingKeys.Contains<Keys>(theKey);
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            EditorManager.Draw(time, spriteBatch);
            UIManager.Draw(time, spriteBatch);
        }
    }
}
