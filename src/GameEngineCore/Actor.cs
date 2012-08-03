using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HuntTheWumpus.GameEngineCore
{
    delegate void ActorCollisionEventHandler(Actor sender, RigidBody collidingBody);
    delegate void ParameterChangeEvent(object previous);
    abstract class Actor
    {
        public event ParameterChangeEvent StateChanged;

        public static readonly ActorState StaticState = new ActorState("Static");
        public int objectType = 0;
        public PhysicsCore.PolyBody PolyBody;
        /*{
            get
            {
                if (BodySet.Count > 0)
                    return BodySet[0];
                else
                    throw new Exception("needs atleast one body in set");
            }
            set
            {
                BodySet.Clear();
                BodySet.Add(value);
            }
        }*/

        //public List<PhysicsCore.PolyBody> BodySet = new List<PolyBody>();

        public GraphicsCore.Sprite Sprite;
        public Boolean NoTexture;
        public float Scale;
        public Color color;

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
            setupGraphics();
            setupPhysics();
            PolyBody.Collided += new PhysicsEventHandler(Collided);
        }

        void Collided(RigidBody sender, object ignoreParamater)
        {
            var game = GameEngine.Singleton.GetPlayState();
            if (game.ActiveMap.MainPlayer.PolyBody == sender)
            {
                this.actUpon();
                if (CollidedWith != null)
                    CollidedWith(this, (PolyBody)sender);
            }
        }

        public abstract void setupPhysics();
        public abstract void setupGraphics();
        public virtual void actUpon() { }

        public virtual void Kill()
        {
            var game = GameEngine.Singleton.GetPlayState();
            game.PhysicsManager.PolyBodies.Remove(this.PolyBody);
            game.ActiveMap.RemoveGameObject(this);
        }

        protected bool doNotSave = false;
        public override string ToString()
        {
            if (!doNotSave)
                return String.Format(
                    "<{0} position=\"{1}\" velocity=\"{2}\" time=\"{3}\" friction=\"{4}\" gravity=\"{5}\">{6}</{0}>",
                    this.GetType().Name,
                    (PolyBody.Position.ToVector3() + PolyBody.Rotation * Vector3.UnitZ).ToPrettyString(),
                    (PolyBody.Velocity.ToVector3() + PolyBody.AngularVelocity * Vector3.UnitZ).ToPrettyString(),
                    PolyBody.Time, PolyBody.Friction, (PolyBody.Gravity).ToVector3().ToPrettyString(),
                    this.ParseStandardConstructor());
            else return String.Empty;
        }

        protected abstract string ParseStandardConstructor();
    }
}
