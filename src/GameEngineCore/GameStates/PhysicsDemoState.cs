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
    class PhysicsDemoState : GameState, IPlayable
    {
        Size size;
        Random random = new Random();
        Label debugText = new Label(new Vector2(20, 10), "60 fps");

        SpriteFont font;

        public Map ActiveMap { get; set; }
        public PhysicsEngine PhysicsManager { get; private set; }
        public UIEngine UIManager { get; private set; }
        public GraphicsEngine GraphicsManager;
        public FPSManager gameLoopFPS;

        Texture2D circ;

        List<Vector2> vertices = new List<Vector2>();

        ParticleEmmiter pemitter;

        // Core Functions
        public PlayerProfile ActiveProfile { get; private set; }
        public override void Load()
        {
            ActiveProfile = new PlayerProfile("null");
            MathUtil.Init((float)DefaultSettings.Settings["PixelsPerMeter"]);
            size = ((Vector2)(Size)Utilities.DefaultSettings.Settings["WindowSize"]) / MathUtil.PixelsPerMeter;

            PhysicsManager = new PhysicsEngine();
            UIManager = new UIEngine();
            GraphicsManager = new GraphicsEngine();
            gameLoopFPS = new FPSManager();

            font = (SpriteFont)ContentRepository.Content["Text"];
            //circ = (Texture2D)ContentRepository.Content["Circle"]; 

            GraphicsEngine.Load();

            InitPhysicsEngine();
            InitUI();
        }
        public override void Update(GameTime time)
        {
            float dt = 1 / 60f;

            pemitter.Update(dt);

            PhysicsManager.Update(dt);
            UIManager.Update(time);
            gameLoopFPS.Update(time);
            debugText.Text = gameLoopFPS.GetFPS().ToString() + " fps\n" + (PhysicsManager.PolyBodies.Count).ToString() + " polygons\n" + (PhysicsManager.Spirits.Count).ToString() + " particles";
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {

            foreach (PolyBody body in PhysicsManager.PolyBodies)
            {
                GraphicsEngine.FillPolygon(body, body.tex, Color.Gray);
                DrawPolygon(body);

                //var str = body.timeSinceTouched.ToString();
                //var str = body.PureEnergy ? "1" : "0";
                var str = body.PureEnergy.ToString();
                //if (body.IsVegetated()) str = "veg";

                spriteBatch.DrawString(font, str, body.Position.P2C(), Color.Black);
            }
            foreach (Particle p in PhysicsManager.Spirits)
            {
                //var scale = (p.rad * MathUtil.PixelsPerMeter) / (circ.Width / 2);
                //spriteBatch.Draw(circ, p.Position.P2C(), null, Color.White, p.Rotation, new Vector2(circ.Width / 2, circ.Height / 2), scale, SpriteEffects.None, 0f); 
                DrawPolygon((PolyBody)p);
            }
            foreach (var bfc in PhysicsManager.BinaryForceComponents)
            {
                DrawSpring(bfc);
            }
            UIManager.Draw(time, spriteBatch);
        }


        private void DrawSpring(BinaryForceComponent bfc)
        {
            var end1 = bfc.BodyA.Position + MathUtil.RotatePt(bfc.ConA, bfc.BodyA.Rotation);
            var end2 = bfc.BodyB.Position + MathUtil.RotatePt(bfc.ConB, bfc.BodyB.Rotation);
            GraphicsEngine.DrawLine(end1.P2C(), end2.P2C(), Color.Firebrick, 5);
        }
        private void DrawPolygon(PolyBody body)
        {
            Vector2 position = body.Position;
            Color color;
            if (body.IsSleeping)
                color = Color.Black;
            else
                color = Color.Red;

            GraphicsEngine.DrawPolygon((Polygon)body, color, new Vector2(.1f).P2C().X);
            //GraphicsEngine.DrawForces(position, ((PolyBody)body).currForces, 3, .1f);
        }
        public override void Destroy()
        {
            GameControler.Singleton.Disconnect();
        }

        // Init Functions
        void InitPhysicsEngine()
        {
            pemitter = new ParticleEmmiter(PhysicsManager, new Vector2(1f, .5f), Vector2.One, 10f, 50f, 3f); 

            var ground = new PolyBody(1, 4);
            ground.MakeRect(20, 1, (new Vector2(size.Width / 2, 0)));
            ground.IsFixed = true;
            AddAndInit(ground);
            

            /*var polybody = new PolyBody(1f, (new Random()).Next(5, 5)); //3,8
            polybody.MakeRegularFromRad(.8f);
            polybody.Mass = 1f;
            polybody.Position = new Vector2(size.Width / 2, size.Height / 2);
            polybody.Restitution = 0.0f;

            AddAndInit(polybody);*/

            var rotPlat = new PolyBody(1f, 4);
            rotPlat.MakeRect(8, 1);
            rotPlat.Mass = 4f;
            rotPlat.Position = new Vector2(size.Width / 2, size.Height / 2);
            rotPlat.Restitution = 0.0f;
            rotPlat.IsPivot = true;

            AddAndInit(rotPlat);


            #region bouncer
            float width1 = 4f;
            float width2 = 2f;

            float height = .5f;

            var p1 = new PolyBody(3, 4);
            p1.MakeRect(width1, height);
            p1.Position = new Vector2(size.Width / 2, 1);
            var p1_con1 = -(Vector2.UnitX * width1 / 2 * 3 / 4);
            var p1_con2 = (Vector2.UnitX * width1 / 2 * 3 / 4);

            var p2 = new PolyBody(1, 4);
            p2.MakeRect(width2, height);
            p2.Position = new Vector2(size.Width / 2, 3.5f);
            var p2_con1 = -(Vector2.UnitX * width2 / 2 * 3 / 4) - (Vector2.UnitY * height/2 * 2 / 3);
            var p2_con2 = (Vector2.UnitX * width2 / 2 * 3 / 4) - (Vector2.UnitY * height/2 * 2 / 3);


            Spring s1 = new Spring(p1, p2, p1_con1, p2_con1, 3f, 30f, 1.0f);
            Spring s2 = new Spring(p1, p2, p1_con2, p2_con2, 3f, 30f, 1.0f);

            Spring s3 = new Spring(p1, p2, p1_con1, p2_con2, 3f, 30f, 1.0f);
            Spring s4 = new Spring(p1, p2, p1_con2, p2_con1, 3f, 30f, 1.0f);

            s1.SetLengthToCurrent();
            s2.SetLengthToCurrent();


            PhysicsManager.AddBinaryForceComponent(s1);
            PhysicsManager.AddBinaryForceComponent(s2);
            PhysicsManager.AddBinaryForceComponent(s3);
            PhysicsManager.AddBinaryForceComponent(s4);

            AddAndInit(p1);
            AddAndInit(p2);
            #endregion bouncer


        }
        private void AddAndInit(PolyBody p)
        {
            PhysicsManager.AddRigidBody(p);
            p.tex = GraphicsGenerator.GenerateTexFromPolygon(p);
        }
        private void InitGame()
        {
        }
        void InitUI()
        {
            UIManager.AddFrame(new Frame());

            debugText.Initialize((SpriteFont)ContentRepository.Content["Text"]);
            debugText.ForeColor = Color.Black;
            UIManager.ActiveFrame.AddControl(debugText);

            UIManager.ActiveFrame.KeyUp += new KeyEventHandler(KeyUp);
            UIManager.ActiveFrame.MouseClick += new MouseEventHandler(MouseClick);
            UIManager.ActiveFrame.MouseDown += new MouseEventHandler(ActiveFrame_MouseDown);
        }

 
        // Helper Functions

        // Events
        void KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.Escape))
                this.Exit();
            if (e.InterestingKeys.Contains<Keys>(Keys.OemTilde))
                GameEngine.Singleton.AddAndLoad(new ConsoleState());
            if (vertices.Count > 0 && e.InterestingKeys.Contains(Keys.Enter))
            {
                var body = new PolyBody(1, vertices.Count);
                body.ConstructFromVertices(vertices.ToArray());
                body.IsFixed = true;
                PhysicsManager.AddRigidBody(body);
                body.tex = GraphicsGenerator.GenerateTexFromPolygon(body);
                vertices.Clear();
            }
        }
        void MouseClick(GUIElement sender, MouseEventArgs e)
        {
            if (e.CurrentMouseState.LeftButton == ButtonState.Released)
            {
                pemitter.TurnOff();
            }
            
            if (e.isClicked(MouseButtons.Right))
            {
                int vertexCount = (new Random()).Next(4, 4);
                if (vertexCount >= 9)
                {
                    vertexCount = 25;
                }
                var polybody = new PolyBody(1f, vertexCount);
                polybody.MakeRegularFromRad(.8f);
                polybody.Mass = 1f;
                polybody.Position = new Vector2(e.CurrentMouseState.X, e.CurrentMouseState.Y).C2P();
                polybody.Restitution = 0.1f;
                polybody.Friction = .20f;

                AddAndInit(polybody);
                //vertices.Add(new Vector2(e.CurrentMouseState.X, e.CurrentMouseState.Y).C2P());
            }
        }
        void ActiveFrame_MouseDown(GUIElement sender, MouseEventArgs e)
        {
            if (e.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                pemitter.Target = new Vector2(e.CurrentMouseState.X, e.CurrentMouseState.Y).C2P();
                pemitter.TurnOn();
            }
        }

        public void EndGame()
        {
            this.Exit();
        }

        public void Reload()
        {
            InitPhysicsEngine();
        }
    }
}
