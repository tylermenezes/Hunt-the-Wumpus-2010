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
using HuntTheWumpus.GameEngineCore;
using HuntTheWumpus.GameEngineCore.Actors;

namespace HuntTheWumpus.GameEngineCore.GameStates
{
    class GraphicsDemoState : GameState, IPlayable
    {
        private Size Size;
        GraphicsEngine GraphicsManager;

        public Map ActiveMap { get; set; }
        public PhysicsEngine PhysicsManager { get; private set; }
        public GUIEngine UIManager { get; private set; }
        public PlayerProfile ActiveProfile { get; private set; }
        public override void Load()
        {
            ActiveProfile = new PlayerProfile("null");

            MathUtil.Init((float)DefaultSettings.Settings["PixelsPerMeter"]);
            Size = ((Vector2)(Size)Utilities.DefaultSettings.Settings["WindowSize"]) / MathUtil.PixelsPerMeter;

            PhysicsManager = new PhysicsEngine();
            UIManager = new GUIEngine();
            GraphicsManager = new GraphicsEngine();
            GraphicsTemp.load();

            InitGame();
             // InitPhysicsEngine(); <-- called in InitGame
            InitUI();
        }
        public override void Update(GameTime time)
        {
            PhysicsManager.Update(1 / 60f);
            UIManager.Update(time);
            ActiveMap.MainPlayer.Update(time);
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {


            DrawPolygon((Polygon)ActiveMap.MainPlayer.PolyBody);
            foreach (Actor actor in ActiveMap.GameObjects)
            {
                DrawPolygon((Polygon)actor.PolyBody);
            }
        }

        private void DrawPolygon(Polygon body)
        {
            Vector2 pos = body.Position;
            GraphicsTemp.DrawPolygon(body, Color.Black, 10f);
        }
        public override void Destroy()
        {
            GameControler.Singleton.Disconnect();
        }

        private void InitUI()
        {
            UIManager.AddAndLoad(new Frame());

            GameControler.Singleton.Connect(UIManager.ActiveFrame);
            UIManager.ActiveFrame.KeyUp += new KeyEventHandler(ActiveFrame_KeyUp);
            UIManager.ActiveFrame.MouseClick += new MouseEventHandler(ActiveFrame_MouseClick);

            GameControler.Singleton.Move += new GameControler.PlayerEventHandler(PlayerMove);
            GameControler.Singleton.Stop += new GameControler.PlayerEventHandler(PlayerStop);
            GameControler.Singleton.Jump += new GameControler.PlayerEventHandler(PlayerJump);
        }

        void PlayerStop(GameControler.PlayerEventArgs e)
        {
            ActiveMap.MainPlayer.State = Player.StandingState;
        }

        void PlayerMove(GameControler.PlayerEventArgs e)
        {
            ActiveMap.MainPlayer.State = Player.WalkingState;
            ActiveMap.MainPlayer.FacingDirection = e.Direction.GetVector(ActiveMap.MainPlayer.FacingDirection);
        }

        void PlayerJump(GameControler.PlayerEventArgs e)
        {
            ActiveMap.MainPlayer.State = Player.JumpingState;
        }

        void ActiveFrame_KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.Escape))
                this.Exit();
            if (e.InterestingKeys.Contains<Keys>(Keys.OemTilde))
                GameEngine.Singleton.AddAndLoad(new ConsoleState());
        }

        void ActiveFrame_MouseClick(GUIElement sender, MouseEventArgs e)
        {
            var block = new Block(1, e.CurrentMouseState.X, e.CurrentMouseState.Y);
            ActiveMap.AddGameObject(block);
            PhysicsManager.AddPolyBody(block.PolyBody);
        }

        private void InitGame()
        {
            var mainPlayer = new Player(Vector2.Zero);

            new Map(
                new Actor[] 
                { 
                    new Platform(Size.Width),
                    new Block(1, Size.X / 2, 0)
                }).Load(mainPlayer);

        }

        private void InitPhysicsEngine()
        {
            PhysicsManager.AddPolyBody(ActiveMap.MainPlayer.PolyBody);
            foreach (Actor actor in ActiveMap.GameObjects)
            {
                PhysicsManager.AddPolyBody(actor.PolyBody);
            }
        }

        public void Reload()
        {
            InitPhysicsEngine();
        }
    }
}
