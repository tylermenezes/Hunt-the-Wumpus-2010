using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using HuntTheWumpus.UICore;
using HuntTheWumpus.Utilities;
namespace HuntTheWumpus.GameEngineCore
{
    /// <summary>
    /// Translates user input into actions in the game
    /// Naturaly, it is subject to change, if our ideas change
    /// </summary>
    class GameControler
    {
        public Frame Client { get; private set; }
        static GameControler _singleton;
        public static GameControler Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new GameControler();
                return _singleton;
            }
        }

        private GameControler() { Client = null; }
        public struct Direction
        {
            private Func<Vector2, Vector2> _getVector;
            private String _name;
            public Direction(String name, Func<Vector2, Vector2> getter) { _getVector = getter; _name = name; }
            public Vector2 GetVector(Vector2 original) { return _getVector.Invoke(original); }
            public String GetName() { return _name; }
        }

        public struct Directions
        {
            public static readonly Direction Right = new Direction("Right", orig => new Vector2(1, orig.Y));
            public static readonly Direction Left = new Direction("Left", orig => new Vector2(-1, orig.Y));
            public static readonly Direction Up = new Direction("Up", orig => new Vector2(orig.X, 1));
            public static readonly Direction Down = new Direction("Down", orig => new Vector2(orig.X, -1));
            public static readonly Direction Front = new Direction("Front", orig => orig);
            public static readonly Direction Back = new Direction("Back", orig => new Vector2(-orig.X, orig.Y));
            public static readonly Direction Static = new Direction("Static", orig => orig);
        }
        /// <summary>
        /// Control player character
        /// ex. move around, shoot arrow
        /// </summary>
        public delegate void PlayerEventHandler(PlayerEventArgs e);

        /// <summary>
        /// Interacting with other objects, 
        /// ex. pushing blocks, opening doors,  switching levers
        /// </summary>
        public delegate void InteractEventHandler(InteractEventArgs e);

        /// <summary>
        /// manipulating physics
        /// </summary>
        public delegate void ManipulatePhysicsEventHandler(ManipulatePhysicsEventArgs e);


        public class PlayerEventArgs : EventArgs
        {
            public Direction Direction { get; private set; }

            public PlayerEventArgs(Direction direction)
            {
                Direction = direction;
            }
        }

        public class InteractEventArgs : EventArgs
        {
            public Actor Selection { get; private set; }

            public InteractEventArgs(Actor selection)
            {
                Selection = selection;
            }
        }
        public class ManipulatePhysicsEventArgs : EventArgs
        {
            public Vector2 Target { get; private set; }

            public ManipulatePhysicsEventArgs(Vector2 target)
            {
                Target = target;
            }
        }

        public event PlayerEventHandler Stop;
        public event PlayerEventHandler Move;
        public event PlayerEventHandler Jump;
        public event PlayerEventHandler Shoot;
        public event PlayerEventHandler SwitchPhysicsGun;
        public event PlayerEventHandler ChangePhysicsGunSettings;

        /// <summary>
        /// pushing, opening etc.
        /// </summary>
        public event InteractEventHandler EnterPortal;
        public event ManipulatePhysicsEventHandler ManipulatePhysics;

        public event ManipulatePhysicsEventHandler ManipulatePhysicsOn;
        public event ManipulatePhysicsEventHandler ManipulatePhysicsOff;

        private void OnStop(PlayerEventArgs e)
        {
            if (Stop != null)
                Stop(e);
        }
        private void OnMove(PlayerEventArgs e)
        {
            if (Move != null)
                Move(e);
        }
        private void OnJump(PlayerEventArgs e)
        {
            if (Jump != null)
                Jump(e);
        }
        private void OnShoot(PlayerEventArgs e)
        {
            if (Shoot != null)
                Shoot(e);
        }
        private void OnSwitchPhysicsGun(PlayerEventArgs e)
        {
            if (SwitchPhysicsGun != null)
                SwitchPhysicsGun(e);
        }
        private void OnChangePhysicsGunSettings(PlayerEventArgs e)
        {
            if (ChangePhysicsGunSettings != null)
                ChangePhysicsGunSettings(e);
        }
        private void OnEnterPortal(InteractEventArgs e)
        {
            if (EnterPortal != null)
                EnterPortal(e);
        }
        private void OnManipulatePhysics(ManipulatePhysicsEventArgs e)
        {
            if (ManipulatePhysics != null)
                ManipulatePhysics(e);
        }
        private void OnManipulatePhysicsOn(ManipulatePhysicsEventArgs e)
        {
            if (ManipulatePhysicsOn != null)
                ManipulatePhysicsOn(e);
        }
        private void OnManipulatePhysicsOff(ManipulatePhysicsEventArgs e)
        {
            if (ManipulatePhysicsOff != null)
                ManipulatePhysicsOff(e);
        }

        private void HandleKeydownEvents(GUIElement sender, KeyEventArgs keyEvents)
        {
            if (keyEvents.InterestingKeys.Contains<Keys>(Keys.Space))
            {
                if (keyEvents.InterestingKeys.Contains<Keys>(Keys.D))
                    OnJump(new PlayerEventArgs(Directions.Right));
                else if (keyEvents.InterestingKeys.Contains<Keys>(Keys.A))
                    OnJump(new PlayerEventArgs(Directions.Left));
                else
                    OnJump(new PlayerEventArgs(Directions.Static));
            }
            else if (keyEvents.InterestingKeys.Contains<Keys>(Keys.D))
                OnMove(new PlayerEventArgs(Directions.Right));
            else if (keyEvents.InterestingKeys.Contains<Keys>(Keys.A))
                OnMove(new PlayerEventArgs(Directions.Left));
            else if (keyEvents.InterestingKeys.Contains<Keys>(Keys.W))
                OnChangePhysicsGunSettings(
                    new PlayerEventArgs(Directions.Up));
            else if (keyEvents.InterestingKeys.Contains<Keys>(Keys.S))
                OnChangePhysicsGunSettings(
                    new PlayerEventArgs(Directions.Down));
        }

        private void HandleKeyupEvents(GUIElement sender, KeyEventArgs keyEvents)
        {
            if (keyEvents.InterestingKeys.Contains<Keys>(Keys.D) ||
                keyEvents.InterestingKeys.Contains<Keys>(Keys.A) ||
                keyEvents.InterestingKeys.Contains<Keys>(Keys.Space))
                OnStop(new PlayerEventArgs(Directions.Static));
            else if (keyEvents.InterestingKeys.Contains<Keys>(Keys.F))
                OnShoot(new PlayerEventArgs(Directions.Front));
            else if (keyEvents.InterestingKeys.Contains<Keys>(Keys.LeftShift))
            {
                var game = GameEngine.Singleton.GetPlayState();
                var player = game.ActiveMap.MainPlayer.PolyBody;

                foreach (Actor portal in game.ActiveMap.LinkedObjects.Values)
                {
                    if (portal is Actors.Portal && PhysicsCore.CollisionEngine.TestCollisionPoly(
                            portal.PolyBody, player))
                    {
                        OnEnterPortal(new InteractEventArgs(portal));
                        return;
                    }
                }
            }
        }

        private void HandleMouseClickEvents(GUIElement sender, MouseEventArgs mouseEvents)
        {
            var game = GameEngine.Singleton.GetPlayState();
            var point =
                new Vector2((float)mouseEvents.PreviousMouseState.X,
                    (float)mouseEvents.PreviousMouseState.Y).C2P();
            OnManipulatePhysics(new ManipulatePhysicsEventArgs(point));

            if (mouseEvents.CurrentMouseState.LeftButton == ButtonState.Released)
                OnManipulatePhysicsOff(new ManipulatePhysicsEventArgs(point));
            //foreach (Actor actor in game.ActiveMap.GameObjects)
            //{
                //if (PhysicsCore.CollisionEngine.PolygonContainsPoint(
                //    actor.PolyBody,
                //    point))
                //{
                //    OnManipulatePhysics(new InteractEventArgs(actor));
                //    return;
                //}
            //}
        }
        void Client_MouseDown(GUIElement sender, MouseEventArgs e)
        {
            var point =
                new Vector2((float)e.PreviousMouseState.X,
                    (float)e.PreviousMouseState.Y).C2P();

            if (e.PreviousMouseState.LeftButton == ButtonState.Pressed)
                OnManipulatePhysicsOn(new ManipulatePhysicsEventArgs(point));
        }

        private void HandleMouseScrollEvents(GUIElement sender, MouseEventArgs e)
        {
            if (e.CurrentMouseState.ScrollWheelValue > e.PreviousMouseState.ScrollWheelValue)
                OnSwitchPhysicsGun(new PlayerEventArgs(Directions.Up));
            else
                OnSwitchPhysicsGun(new PlayerEventArgs(Directions.Down));
        }

        public void Connect(Frame client)
        {
            Client = client;

            // if Client := Windows Client
            Client.KeyDown += new KeyEventHandler(HandleKeydownEvents);
            Client.KeyUp += new KeyEventHandler(HandleKeyupEvents);
            Client.MouseClick += new MouseEventHandler(HandleMouseClickEvents);
            Client.MouseScroll += new MouseEventHandler(HandleMouseScrollEvents);
            Client.MouseDown += new MouseEventHandler(Client_MouseDown);

            // else if Client := Xbox Client
            //      **Not Yet Implemented**
        }

        public void Disconnect()
        {
            Client = null;

            Stop = null;
            Move = null;
            Jump = null;
            Shoot = null;
            SwitchPhysicsGun = null;
            EnterPortal = null;
            ManipulatePhysics = null;
            ChangePhysicsGunSettings = null;
        }
        /// <summary>
        /// For future(?) XBox support
        /// </summary>
        private void handleGamepadEvents()
        {
            throw new NotImplementedException();
        }
    }
}
