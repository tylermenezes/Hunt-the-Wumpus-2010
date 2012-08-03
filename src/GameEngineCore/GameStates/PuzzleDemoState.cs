using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.UICore;
using HuntTheWumpus.Utilities;
using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.GameEngineCore;
using HuntTheWumpus.GameEngineCore.Actors;
namespace HuntTheWumpus.GameEngineCore.GameStates
{
    class PuzzleDemoState : GameState, IPlayable
    {
        private Size Size;

        public Map ActiveMap { get; set; }
        public PhysicsEngine PhysicsManager { get; private set; }
        public GUIEngine UIManager { get; private set; }

        UICore.Controls.Label weaponPowerLabel;
        public PlayerProfile ActiveProfile { get; private set; }

        public PuzzleDemoState(string ProfileName, bool newProfile)
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
            UIManager = new GUIEngine();
           
            GraphicsEngine.Load();
            
            InitGame();
           // InitPhysicsEngine(); <- happens in initGame by map
            InitUI();

        }
        public override void Update(GameTime time)
        {
            PhysicsManager.Update(1 / 60f);
            UIManager.Update(time);
            ActiveMap.DynamicObjects.ForEach(actor => ((IDynamic)actor).Update(time));
        }
        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            
           // DrawPolygon((Polygon)ActiveMap.MainPlayer.PolyBody);
            foreach (Actor actor in ActiveMap.GameObjects)
            {
                if (actor != ActiveMap.MainPlayer )
                    CreateSprite(actor.GetType().Name, 
                        actor.State.Name, 
                        actor.Scale, 
                        actor.PolyBody.Position, 
                        actor.PolyBody.Rotation);
        
                //Uncomment to show bounding boxes
                //DrawPolygon((Polygon)actor.PolyBody);

                if (actor.NoTexture)
                {
                    FillPolygon((PolyBody)actor.PolyBody, actor);
                    
                }
                
            }
            CreateSprite(ActiveMap.MainPlayer.GetType().ToString(),
                ActiveMap.MainPlayer.State.Name,
                ActiveMap.MainPlayer.Scale,
                ((Polygon)ActiveMap.MainPlayer.PolyBody).Position,
                ActiveMap.MainPlayer.PolyBody.Rotation,
                ActiveMap.MainPlayer.FacingDirection);

            GraphicsEngine.Draw(time);
            UIManager.Draw(time, spriteBatch);
        }
        private void CreateSprite(String ActorName,String ActorState, float scale, Vector2 position, float roation)
        {
            
            Sprite Actor = new Sprite(ActorName, ActorState, scale, position, roation);
            GraphicsEngine.AddSprite(Actor);
        }
        private void CreateSprite(String ActorName, String ActorState, float scale, Vector2 position, float roation, Vector2 direction)
        {

            Sprite Actor = new Sprite(ActorName, ActorState, scale, position, roation, direction);
            GraphicsEngine.AddSprite(Actor);

        }
        private void DrawPolygon(Polygon body)
        {
            Vector2 pos = body.Position;
            GraphicsEngine.DrawPolygon(body, Color.Black, 10f);
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

        private void InitUI()
        {
            var frame = new Frame();
            weaponPowerLabel = new HuntTheWumpus.UICore.Controls.Label(Vector2.Zero, ActiveProfile.PhysicsWeaponInventory.WeaponPower.ToString());
            weaponPowerLabel.Initialize();
            frame.AddControl(weaponPowerLabel);
            UIManager.AddAndLoad(frame);


            foreach (Actor actor in ActiveMap.GameObjects)
            {
                if (actor.NoTexture)
                {
                    PolyBody body = (PolyBody)actor.PolyBody;
                        body.tex = GraphicsGenerator.GenerateTexFromPolygon(body);
                }
            }

            GameControler.Singleton.Connect(UIManager.ActiveFrame);
            UIManager.ActiveFrame.KeyUp += new KeyEventHandler(ActiveFrame_KeyUp);

            GameControler.Singleton.Move += new GameControler.PlayerEventHandler(PlayerMove);
            GameControler.Singleton.Stop += new GameControler.PlayerEventHandler(PlayerStop);
            GameControler.Singleton.EnterPortal += new GameControler.InteractEventHandler(EnterPortal);
            GameControler.Singleton.ChangePhysicsGunSettings += new GameControler.PlayerEventHandler(ChangePhysicsGunSettings);
            GameControler.Singleton.SwitchPhysicsGun += new GameControler.PlayerEventHandler(SwitchPhysicsGun);
            GameControler.Singleton.Jump += new GameControler.PlayerEventHandler(PlayerJump);
            GameControler.Singleton.ManipulatePhysics += new GameControler.ManipulatePhysicsEventHandler(ManipulatePhysics);
        }

        void SwitchPhysicsGun(GameControler.PlayerEventArgs e)
        {
            var direction = e.Direction.GetVector(Vector2.Zero).X;
            ActiveProfile.PhysicsWeaponInventory.SwitchWeapon((int)direction);
        }

        void ChangePhysicsGunSettings(GameControler.PlayerEventArgs e)
        {
            var direction = e.Direction.GetVector(Vector2.Zero).Y;
            ActiveProfile.PhysicsWeaponInventory.ChangeWeaponPower(direction);
        }
        void PlayerJump(GameControler.PlayerEventArgs e)
        {
            ActiveMap.MainPlayer.State = Player.JumpingState;
        }

        void ManipulatePhysics(GameControler.ManipulatePhysicsEventArgs e)
        {
            ActiveProfile.PhysicsWeaponInventory.DeployWeapon(e.Target);
        }

        void EnterPortal(GameControler.InteractEventArgs e)
        {
            ((Portal)e.Selection).Activate();
            foreach (Actor actor in ActiveMap.GameObjects)
            {
                if (actor.NoTexture)
                {
                    PolyBody body = (PolyBody)actor.PolyBody;
                    body.tex = GraphicsGenerator.GenerateTexFromPolygon(body);
                }
            }
        }

        void PlayerStop(GameControler.PlayerEventArgs e)
        {
            ActiveMap.MainPlayer.State = Player.StandingState;
        }

        void PlayerMove(GameControler.PlayerEventArgs e)
        {
            ActiveMap.MainPlayer.FacingDirection = e.Direction.GetVector(ActiveMap.MainPlayer.FacingDirection);
            ActiveMap.MainPlayer.State = Player.WalkingState;
        }

        void ActiveFrame_KeyUp(GUIElement sender, KeyEventArgs e)
        {
            if (e.InterestingKeys.Contains<Keys>(Keys.Escape))
                this.Exit();
            if (e.InterestingKeys.Contains<Keys>(Keys.OemTilde))
                GameEngine.Singleton.AddAndLoad(new ConsoleState());
        }

        private void InitGame()
        {
            if (ActiveProfile.IsSavedProfileNull())
            {
                var map1 =
                    new Map("TestMap1",
                        new Actor[] 
                    { 
                        new Floor(Size.Width, Color.Blue),
                        //new FromArtPlatform(Size.Width, Size.Width/2, 0),
                        new Block(50, .25f, 6, 5),
                        new PhysicsHat("GravityGrenade", Size.Width /4, 
                            PhysicsHat.Radius),
                        new Bouncer(1, Color.Red, Size.Width - 1, 1),
                      // new Actors.IntraMapPortal("Entrance", Portal.Radius, Portal.Radius, false),
                        new Actors.IntraMapPortal("Entrance", 2, 2, false),
                       new Actors.InterMapPortal("Exit", Size.Width - Portal.Radius, Portal.Radius)
                   });

                var map2 =
                    new Map("TestMap2",
                        new Actor[] 
                        { 
                            new Platform(1, Color.Orange, 1, Size.Height / 2),
                            new Platform(1, Color.Purple, 2.5f, Size.Height / 2),
                            new MovablePlatform(5000, 1.1f, Color.Green, Platform.Thickness, 4.5f, Size.Height / 2),
                            new MovablePlatform(200,     2, Color.HotPink, Platform.Thickness, Size.Width - 4, 1),
                            new Platform(Portal.Radius, Color.Yellow, Size.Width - Portal.Radius, Size.Height / 2),
                            new Bouncer(1, Color.Red, Size.Width - Portal.Radius, 
                               Size.Height / 2 + Portal.Radius),
 
                           new Actors.InterMapPortal("Entrance", Portal.Radius, 
                               Size.Height / 2 + Portal.Radius),
                           new Actors.IntraMapPortal("Bottom", 2, 1, true),
                           new Actors.IntraMapPortal("Top", Size.Width - 4, Size.Y / 2, true),
                           new Actors.InterMapPortal("Exit", Size.Width - Portal.Radius, 
                               Size.Height / 2 + Portal.Radius)
                       });

                ((InterMapPortal)map1.LinkedObjects["Exit"]).DestinationMap = map2;
                ((InterMapPortal)map1.LinkedObjects["Exit"]).DestinationPortalID = "Entrance";

                ((InterMapPortal)map2.LinkedObjects["Entrance"]).DestinationMap = map1;
                ((InterMapPortal)map2.LinkedObjects["Entrance"]).DestinationPortalID = "Exit";

                map2.LinkedObjects["Bottom"] = map2.LinkedObjects["Top"];
                map2.LinkedObjects["Top"] = map2.LinkedObjects["Bottom"];


                map2.GameObjects[2].PolyBody.Gravity = Vector2.Zero;
                map2.GameObjects[3].PolyBody.Gravity = Vector2.Zero;
                //map2.StaticActors[3].PolyBody.RotationEnabled = false;

                map1.UnLoaded += new MapEventHandler(SaveMap);
                map2.UnLoaded += new MapEventHandler(SaveMap);

                map1.Load("Entrance");

                ActiveProfile.PhysicsWeaponInventory.WeaponPowerChanged += new PhysicsWeaponManagerEventHandler(RefreshWeaponPowerLabel);
                ActiveProfile.PhysicsWeaponInventory.WeaponSwitched += new PhysicsWeaponManagerEventHandler(RefreshWeaponPowerLabel);
            }
            else ActiveProfile.Load();
        }

        void SaveMap(Map sender)
        {
            ActiveProfile.SaveCurrentMap();
        }

        void RefreshWeaponPowerLabel()
        {
            weaponPowerLabel.Text = ActiveProfile.PhysicsWeaponInventory.WeaponPower.ToString();
        }

        private void InitPhysicsEngine()
        {
            PhysicsManager = new PhysicsEngine();

            foreach (Actor actor in ActiveMap.GameObjects)
            {
                if (actor.IsPhysicsable)
                    PhysicsManager.AddPolyBody(actor.PolyBody);
            }
        }
        public void Reload()
        {
            InitPhysicsEngine();
        }
    }
}
