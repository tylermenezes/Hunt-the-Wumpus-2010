using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.Utilities;
using Microsoft.Xna.Framework;
namespace HuntTheWumpus.GameEngineCore
{
    delegate void ActorCollisionEventHandler(Actor sender, PolyBody collidingBody);
    delegate void ParameterChangeEvent(object previous);
    abstract class Actor
    {
        public event ParameterChangeEvent StateChanged;

        public static readonly ActorState StaticState = new ActorState("Static");
        public String standardObjectType = null;
        public PhysicsCore.PolyBody PolyBody;

        public GraphicsCore.Sprite Sprite;
        public Boolean NoTexture;
        private ActorState _state;
        

        
        public ActorState State
        {
            get { return _state; }
            set
            {
                ActorState previous = _state;
                _state = value;
                
                if (StateChanged != null)
                {
                    StateChanged(previous);
                }
            }
        }

        public Boolean IsPhysicsable { get; set; }
        public Boolean IsCollidable { get; set; }

        private bool _isFixed;
        public Boolean IsFixed
        {
            get
            {
                if (PolyBody == null)
                    return _isFixed;
                return PolyBody.IsFixed;
            }
            set
            {
                _isFixed = value;
                if (PolyBody != null)
                    PolyBody.IsFixed = value;
            }
        }

        // useful events //
        event ActorCollisionEventHandler CollidedWith;

        public Actor()
        {
            IsPhysicsable = true;
            IsCollidable = true;
        }

        public Actor(bool isPhysicsable, bool isCollidable, bool isFixed)
        {
            IsPhysicsable = isPhysicsable;
            IsCollidable = IsCollidable;
            IsFixed = true;
        }

        public virtual void Load()
        {
            NoTexture = false;
            State = StaticState;
            if (IsFixed)
                PolyBody.IsFixed = true;
            setupPhysics();
            PolyBody.Collided += new PhysicsEventHandler(Collided);
        }

        void Collided(PolyBody sender, object ignoreParamater)
        {
            var game = (GameEngineCore.GameStates.IPlayable)GameEngine.Singleton.ActiveState;
            if (game.ActiveMap.MainPlayer.PolyBody == sender)
            {
                this.actUpon();
                if (CollidedWith != null)
                    CollidedWith(this, (PolyBody)sender);
            }
        }

        public abstract void setupPhysics();
        public abstract void actUpon();

        public virtual void Kill()
        {
            var game = (GameEngineCore.GameStates.IPlayable)GameEngine.Singleton.ActiveState;
            game.PhysicsManager.PolyBodies.Remove(this.PolyBody);
            game.ActiveMap.RemoveGameObject(this);
        }

        public override string ToString()
        {
            return String.Format(
                "<{0} position=\"{1}\" velocity=\"{2}\" time=\"{3}\" Friction=\"{4}\" Gravity=\"{5}\" />",
                this.GetType().Name,
                PolyBody.Position.ToVector3() + PolyBody.Rotation * Vector3.UnitZ,
                PolyBody.Velocity.ToVector3() + PolyBody.AngularVelocity * Vector3.UnitZ,
                PolyBody.Time, PolyBody.Friction, PolyBody.Gravity); 

        }
    }
}
