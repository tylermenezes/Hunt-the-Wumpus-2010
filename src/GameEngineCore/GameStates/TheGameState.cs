using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.UICore.Controls;
using HuntTheWumpus.UICore;
using HuntTheWumpus.Utilities;
using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.GameEngineCore;
using HuntTheWumpus.GameEngineCore.Actors;
using HuntTheWumpus.GameEngineCore.Frames;
using HuntTheWumpus.SoundCore;
namespace HuntTheWumpus.GameEngineCore.GameStates
{
    class TheGameState : GameState, IPlayable
    {
        public Size Size;
        private PowerBar powerBar;
        private EnergyBar energyBar;
        public Map ActiveMap { get; set; }
        public PhysicsEngine PhysicsManager { get; private set; }
        public UIEngine UIManager { get; private set; }
        public BackgroundMusic _backGroundMusic { get; private set; }
        public FPSManager FPSCounter;
        String TEMP;

        UICore.Controls.Label weaponPowerLabel;
        public PlayerProfile ActiveProfile { get; private set; }

        public TheGameState(string ProfileName, bool newProfile)
        {
            if (newProfile)
                ActiveProfile = new PlayerProfile(ProfileName);
            else ActiveProfile = PlayerProfile.LoadFromFile(ProfileName);
        }
        public override void Load()
        {
            MathUtil.Init((float)DefaultSettings.Settings["PixelsPerMeter"]);
            Size = ((Vector2)(Size)Utilities.DefaultSettings.Settings["WindowSize"]) / MathUtil.PixelsPerMeter;

            PhysicsManager = new PhysicsEngine();
            UIManager = new UIEngine();
            _backGroundMusic = new BackgroundMusic();
            GraphicsEngine.Load();

            FPSCounter = new FPSManager();
            
            InitGame();
           // InitPhysicsEngine(); <- happens in initGame by map
            InitUI();

        }
        public override void Update(GameTime time)
        {
            PhysicsManager.Update(1 / 60f);
            _backGroundMusic.Update();
            UIManager.Update(time);
            //Miscellaneous Updates:
            energyBar.updateEnergy(ActiveProfile.PhysicsWeaponInventory.EnergyAvailable); 
            powerBar.updatePower(ActiveProfile.PhysicsWeaponInventory.WeaponPower);
            

            FPSCounter.Update(time);
             
            
            ActiveMap.DynamicObjects.ForEach(actor => ((IDynamic)actor).Update(time));


        }
        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {

            // DrawPolygon((Polygon)ActiveMap.MainPlayer.PolyBody);
            foreach (Actor actor in ActiveMap.GameObjects)
            {
                if ((actor != null) && (actor != ActiveMap.MainPlayer))
                    CreateSprite(actor.GetType().Name,
                        actor.State.Name,
                        actor.Scale,
                        actor.PolyBody.Position,
                        actor.PolyBody.Rotation);

                //Uncomment to show bounding boxes
                //if (actor != null)
                //    DrawPolygon((Polygon)actor.PolyBody);

                if ((actor != null) && (actor.NoTexture))
                {
                    FillPolygon((PolyBody)actor.PolyBody, actor);
                }

            }
            if (ActiveProfile.PhysicsWeaponInventory.ActiveWeaponDeployer != null)
                TEMP = ActiveProfile.PhysicsWeaponInventory.ActiveWeaponDeployer.Name;
            else
                TEMP = "";

            CreateSprite(ActiveMap.MainPlayer.GetType().ToString(),
                ActiveMap.MainPlayer.State.Name,
                ActiveMap.MainPlayer.Scale,
                ((Polygon)ActiveMap.MainPlayer.PolyBody).Position,
                ActiveMap.MainPlayer.PolyBody.Rotation,
                ActiveMap.MainPlayer.FacingDirection,
                TEMP);

            GraphicsEngine.Draw(time);
            UIManager.Draw(time, spriteBatch);
        }
       
        private void CreateSprite(String ActorName,String ActorState, float scale, Vector2 position, float roation)
        {
            
            Sprite Actor = new Sprite(ActorName, ActorState, scale, position, roation);
            GraphicsEngine.AddSprite(Actor);
        }
        private void CreateSprite(String ActorName, String ActorState, float scale, Vector2 position, float roation, Vector2 direction, string weapon)
        {

            Sprite Actor = new Sprite(ActorName, ActorState, scale, position, roation, direction, weapon);
            GraphicsEngine.AddSprite(Actor);

        }
        private void ResetAnimation(String ActorName, String ActorState, float scale, Vector2 position, float roation, Vector2 direction, string weapon)
        {
            Sprite Actor = new Sprite(ActorName, ActorState, scale, position, roation, direction, weapon);

        }
        private void DrawPolygon(Polygon body)
        {
            Vector2 pos = body.Position;
            GraphicsEngine.DrawPolygon(body, Color.Black, 3f);
        }
        private void FillPolygon(PolyBody body, Actor actor)
        {
            GraphicsEngine.FillPolygon(body, ((PolyBody)body).tex, actor.color);
        }
        public override void Destroy()
        {
            GameControler.Singleton.Disconnect();
            ActiveProfile.SaveCurrentMap();
            PlayerProfile.SaveToFile(ActiveProfile);
        }

        public void EndGame()
        {
            this.Exit();
        }

        private void InitGraphics()
        {
            foreach (Actor actor in ActiveMap.GameObjects)
            {
                if ((actor != null) && (actor.NoTexture))
                {
                    PolyBody body = (PolyBody)actor.PolyBody;
                    body.tex = GraphicsGenerator.GenerateTexFromPolygon(body);
                }
            }
        }
        private void InitUI()
        {
            Size currentWindowSize = (Size)Utilities.DefaultSettings.Settings["WindowSize"];
            var frame = new Frame();
            //weaponPowerLabel = new HuntTheWumpus.UICore.Controls.Label(Vector2.Zero, ActiveProfile.PhysicsWeaponInventory.WeaponPower.ToString());
            //weaponPowerLabel.Initialize();
            powerBar = new PowerBar();
            Label powerBarTitle = new Label(new Vector2(0, 35), "Power");
            powerBarTitle.Initialize();
            energyBar = new EnergyBar(); 
            Label energyBarTitle = new Label(new Vector2(0, 50), "Energy");
            energyBarTitle.Initialize();
            Label playerNameTitle = new Label(new Vector2(currentWindowSize.X/2 - 100, 10),"Player Name: "); 
            Label playerName = new Label(new Vector2(currentWindowSize.X/2 + 30, 10), ActiveProfile.ID);

           _backGroundMusic.pickBackgroundMusic(1);


            playerNameTitle.Initialize(); 
            playerName.Initialize(); 
            frame.AddControl(playerName); 
            frame.AddControl(playerNameTitle);
 

            //frame.AddControl(weaponPowerLabel);
            UIManager.AddAndLoad(frame);
            frame.AddControl(powerBar);
            frame.AddControl(powerBarTitle);
            frame.AddControl(energyBar);
            frame.AddControl(energyBarTitle);
            
          


            GameControler.Singleton.Connect(UIManager.ActiveFrame);
            UIManager.ActiveFrame.KeyUp += new KeyEventHandler(ActiveFrame_KeyUp);

            GameControler.Singleton.Move += new GameControler.PlayerEventHandler(PlayerMove);
            GameControler.Singleton.Stop += new GameControler.PlayerEventHandler(PlayerStop);
            GameControler.Singleton.EnterPortal += new GameControler.InteractEventHandler(EnterPortal);
            GameControler.Singleton.ChangePhysicsGunSettings += new GameControler.PlayerEventHandler(ChangePhysicsGunSettings);
            GameControler.Singleton.SwitchPhysicsGun += new GameControler.PlayerEventHandler(SwitchPhysicsGun);
            GameControler.Singleton.Jump += new GameControler.PlayerEventHandler(PlayerJump);
            GameControler.Singleton.ManipulatePhysics += new GameControler.ManipulatePhysicsEventHandler(ManipulatePhysics);

            GameControler.Singleton.ManipulatePhysicsOn += new GameControler.ManipulatePhysicsEventHandler(Singleton_ManipulatePhysicsOn);
            GameControler.Singleton.ManipulatePhysicsOff += new GameControler.ManipulatePhysicsEventHandler(Singleton_ManipulatePhysicsOff);
        }



        void SwitchPhysicsGun(GameControler.PlayerEventArgs e)
        {
            var direction = e.Direction.GetVector(Vector2.Zero).Y;
            ActiveProfile.PhysicsWeaponInventory.SwitchWeapon((int)direction);
        }

        void ChangePhysicsGunSettings(GameControler.PlayerEventArgs e)
        {
            var direction = e.Direction.GetVector(Vector2.Zero).Y;
            ActiveProfile.PhysicsWeaponInventory.ChangeWeaponPower(direction);
        }
        void PlayerJump(GameControler.PlayerEventArgs e)
        {

            ActiveMap.MainPlayer.SetPlayerState(Player.JumpingState, e);

        }

        void ManipulatePhysics(GameControler.ManipulatePhysicsEventArgs e)
        {
            ActiveProfile.PhysicsWeaponInventory.DeployWeapon(e.Target);
            ActiveMap.MainPlayer.FacingDirection = Vector2.UnitX*(float)Math.Sign(e.Target.X - ActiveMap.MainPlayer.PolyBody.Position.X);
        }

        void Singleton_ManipulatePhysicsOff(GameControler.ManipulatePhysicsEventArgs e)
        {
            ActiveProfile.PhysicsWeaponInventory.SetOff(e.Target);
        }

        void Singleton_ManipulatePhysicsOn(GameControler.ManipulatePhysicsEventArgs e)
        {
            ActiveProfile.PhysicsWeaponInventory.SetOn(e.Target);
            ActiveMap.MainPlayer.FacingDirection = Vector2.UnitX * (float)Math.Sign(e.Target.X - ActiveMap.MainPlayer.PolyBody.Position.X);
        }

        void EnterPortal(GameControler.InteractEventArgs e)
        {
            _backGroundMusic.endCurrentBackgroundMusic();
            ((Portal)e.Selection).Activate();
        }

        void PlayerStop(GameControler.PlayerEventArgs e)
        {
            ActiveMap.MainPlayer.SetPlayerState(Player.StandingState, e);
        }

        void PlayerMove(GameControler.PlayerEventArgs e)
        {
            ActiveMap.MainPlayer.SetPlayerState(Player.WalkingState, e);
        }

        void ActiveFrame_KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.Escape))
            {
                _backGroundMusic.endCurrentBackgroundMusic();
                PauseMenuFrame pauseMenu = new PauseMenuFrame();
                GameEngine.Singleton.AddAndLoad(new PauseMenuState()); 
            }
            if (e.InterestingKeys.Contains<Keys>(Keys.OemTilde))
                GameEngine.Singleton.AddAndLoad(new ConsoleState());
            if (e.InterestingKeys.Contains<Keys>(Keys.P))
            {
                _backGroundMusic.endCurrentBackgroundMusic();
               
            }
            if (e.InterestingKeys.Contains<Keys>(Keys.O))
                _backGroundMusic.fadeOutCurrentBackgroundMusic();
        }

        private Dictionary<string, Actor[]> _hardCodedMaps;
        public Dictionary<string, Actor[]> HardCodedMaps
        {
            get 
            { 
                return _hardCodedMaps; 
            }
            set
            {
                _hardCodedMaps = value;
            }
        }
        private void InitGame()
        {
            HardCodedMaps = new Dictionary<string, Actor[]>();
            
            /*var map1 =
                    new Actor[] 
                { 
                    new Floor(Size.Width, Color.Blue),
                    //new FromArtPlatform(Size.Width, Size.Width/2, 0),
                    new Block(12, .25f, 6, 5),
                    new PhysicsHat("GravityGrenade", Size.Width /4, 
                        PhysicsHat.Radius),
                    new PhysicsHat("FrictionFlamethrower", Size.Width /3, 
                        PhysicsHat.Radius),
                   // new Bouncer(1, Color.Red, Size.Width - 1, 1),
                  
                    new Actors.IntraMapPortal("Entrance", String.Empty, 2, 10),
                   new Actors.InterMapPortal("Exit", "TestMap2.Entrance", Size.Width - Portal.Radius, Portal.Radius)
               };

            var map2 =
                    new Actor[] 
                    { 
                        new Platform(1, Color.Orange, 1, Size.Height / 2),
                        new Platform(1, Color.Purple, 2.5f, Size.Height / 2),
                        new MovablePlatform(5000, 1.1f, Color.Green, Platform.DefaultThickness, 4.5f, Size.Height / 2),
                        new MovablePlatform(200,     2, Color.HotPink, Platform.DefaultThickness, Size.Width - 4, 1),
                            new Platform(Portal.Radius, Color.Yellow, Size.Width - Portal.Radius, Size.Height / 2),
                        new Bouncer(1, Color.Red, Size.Width - Portal.Radius, 
                           Size.Height / 2 + Portal.Radius),
                       new Actors.InterMapPortal("Entrance", "TestMap1.Exit", Portal.Radius, 
                           Size.Height / 2 + Portal.Radius),
                       new Actors.IntraMapPortal("Bottom", ".Top", 2, 1),
                       new Actors.IntraMapPortal("Top", ".Bottom", Size.Width - 4, Size.Y / 2),
                       new Actors.InterMapPortal("Exit", String.Empty, Size.Width - Portal.Radius, 
                           Size.Height / 2 + Portal.Radius)
                   };


            map2[2].PolyBody.Gravity = Vector2.Zero;
            map2[3].PolyBody.Gravity = Vector2.Zero;
            //map2.StaticActors[3].PolyBody.RotationEnabled = false;
            */


            //
            // sean's test map:

            var map1 =
                    new Actor[] 
                { 
                    new Floor(Size.Width, Color.Blue),
                    //new FromArtPlatform(Size.Width, Size.Width/2, 0),
                    new Block(12, .25f, 6, 5),
                    new PhysicsHat("GravityGrenade", Size.Width /4, 
                        PhysicsHat.Radius),
                    new PhysicsHat("FrictionFlamethrower", Size.Width /3, 
                        PhysicsHat.Radius),

                  
                    new Actors.IntraMapPortal("Entrance", String.Empty, 2, 10),
                   new Actors.InterMapPortal("Exit", "TestMap2.Entrance", Size.Width - Portal.Radius, Portal.Radius)
               };

            var map2 =
                    new Actor[] 
                    { 
                        new Platform(1, Color.Orange, 1, Size.Height / 2),
                        new Platform(1, Color.Purple, 2.5f, Size.Height / 2),
                        new MovablePlatform(5000, 1.1f, Color.Green, Platform.DefaultThickness, 4.5f, Size.Height / 2),
                        new MovablePlatform(200,     2, Color.HotPink, Platform.DefaultThickness, Size.Width - 4, 1),
                            new Platform(Portal.Radius, Color.Yellow, Size.Width - Portal.Radius, Size.Height / 2),
                        new Bouncer(1, Color.Red, Size.Width - Portal.Radius, 
                           Size.Height / 2 + Portal.Radius),
                       new Actors.InterMapPortal("Entrance", "TestMap1.Exit", Portal.Radius, 
                           Size.Height / 2 + Portal.Radius),
                       new Actors.IntraMapPortal("Bottom", ".Top", 2, 1),
                       new Actors.IntraMapPortal("Top", ".Bottom", Size.Width - 4, Size.Y / 2),
                       new Actors.InterMapPortal("Exit", String.Empty, Size.Width - Portal.Radius, 
                           Size.Height / 2 + Portal.Radius)
                   };


            map2[2].PolyBody.Gravity = Vector2.Zero;
            map2[3].PolyBody.Gravity = Vector2.Zero;
            //map2.StaticActors[3].PolyBody.RotationEnabled = false;

            //
            // sean's test map:

            var finalMap1 =
                new Actor[]
                {
                    new Actors.IntraMapPortal("Entrance", String.Empty, 2, 10),
                    new Actors.InterMapPortal("Exit", "finalMap2.Entrance", 15, 3),
                    new Platform(5, Color.Blue, 4, 3, 0f),
                    new Platform(5, Color.BurlyWood, 10, 3, 0f),
                    new Block(12, .25f, 10, 4),
                    new Block(12, .25f, 10, 5.25f),
                    new Block(12, .25f, 10, 6.5f),
                    new Block(12, .25f, 10, 7.75f),
                    new Block(12, .25f, 10, 9),
                    new Block(12, .25f, 10, 10.25f),
                    new Block(12, .25f, 9, 4),
                    new Block(12, .25f, 9, 5.25f),
                    new Block(12, .25f, 9, 6.5f),
                    new Block(12, .25f, 9, 7.75f),
                    new Block(12, .25f, 9, 9),
                    new Block(12, .25f, 9, 10.25f),
                    new Block(12, .25f, 11, 4),
                    new Block(12, .25f, 11, 5.25f),
                    new Block(12, .25f, 11, 6.5f),
                    new Block(12, .25f, 11, 7.75f),
                    new Block(12, .25f, 11, 9),
                    new Block(12, .25f, 11, 10.25f),
                    new PhysicsHat("FrictionFlamethrower",4,4),
                };


            var finalMap2 =
                new Actor[]
                {
                    new Actors.IntraMapPortal("Entrance", String.Empty, 2, 4),
                    new Actors.InterMapPortal("Exit", "finalMap3.Entrance", 16, 3),
                    new Platform(3,Color.Coral, 3, 2),
                    new Block(12, .25f, 4, 3),
                    new PhysicsHat("GravityGrenade", 4,4),
                    new Platform(12, Color.Blue, 7, 10, .785397f),
                    new Platform(2, Color.Blue, 14, 12)
                 };
            var finalMap3 =
                new Actor[]
                {
                    new Actors.IntraMapPortal("Entrance", String.Empty, 2, 15),
                    new Actors.InterMapPortal("Exit", "finalMap1.Entrance", 16, 3), 
                    new Platform(4, Color.Blue, 1, 15, (float)Math.PI/2f),
                    new Platform(4, Color.Blue, 3, 15, (float)Math.PI/2f),
                    new Platform(5, Color.Blue, 2, 11, 2),
                    new Platform(5, Color.Blue, 4, 11, 2),          
                    new PhysicsHat("FrictionFlamethrower", 2, 15 ),
                    
                    //new Platform(4, .5f, Color.Red, 6, 4, -.55f), //4.3 | 6.4
                    Platform.GeneratePlatform(new Vector2(4.0f, 4.5f), new Vector2(7f, 3.0f), .5f, Color.Red, 0f),
                    Platform.GeneratePlatform(new Vector2(7f, 3.0f), new Vector2(10f, 3.0f), .5f, Color.Red, 0f),
                    Platform.GeneratePlatform(new Vector2(10f, 3.0f), new Vector2(12f, 3.5f), .5f, Color.Red, 0f),

                    new Block(.01f, .25f, 8, 4, 0.0f)
                 };
            //end test map listing
            //

            HardCodedMaps.Add("finalMap1", finalMap1);
            HardCodedMaps.Add("finalMap2", finalMap2);
            HardCodedMaps.Add("finalMap3", finalMap3);
            

            if (!ActiveProfile.IsSavedProfileNull()) ActiveProfile.Load();
            else ActiveProfile.SetCurrentMap("finalMap1", "Entrance", true);


            ActiveProfile.PhysicsWeaponInventory.WeaponPowerChanged += new PhysicsWeaponManagerEventHandler(RefreshWeaponPowerLabel);
            ActiveProfile.PhysicsWeaponInventory.WeaponSwitched += new PhysicsWeaponManagerEventHandler(RefreshWeaponPowerLabel);
        }

        void RefreshWeaponPowerLabel()
        {
            // TODO: FIX THIS PLEASE OH GOD
            try
            {
                weaponPowerLabel.Text = ActiveProfile.PhysicsWeaponInventory.WeaponPower.ToString();
            }
            catch {/*ADVANCED ERROR HANDLING*/ }
        }

        private void InitPhysicsEngine()
        {
            PhysicsManager = new PhysicsEngine();

            foreach (Actor actor in ActiveMap.GameObjects)
            {
                if (actor != null)
                {
                    if (actor.IsPhysicsable)
                        PhysicsManager.AddRigidBody(actor.PolyBody);
                }
            }
        }
        public void Reload()
        {
            InitPhysicsEngine();
            InitGraphics();
        }
    }
}
