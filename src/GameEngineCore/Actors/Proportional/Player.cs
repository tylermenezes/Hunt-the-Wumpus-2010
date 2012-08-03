using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.GraphicsCore;
using HuntTheWumpus.Utilities;
using Microsoft.Xna.Framework;
namespace HuntTheWumpus.GameEngineCore.Actors
{
    class Player : Actor, IDynamic
    {
        public const float Radius = 1.8f;
        //Walk Speed and MaxWalkSpeed are in meters
        public const float WalkForce = 20;
        public const float MaxWalkSpeed = 5;
        public const float MaxFlySpeed = 3;
        public const float JumpForce = 800;

        
        // Player States //
        public static readonly ActorState StandingState = new ActorState("Standing");
        public static readonly ActorState WalkingState = new ActorState("Walking");
        public static readonly ActorState JumpingState = new ActorState("Jumping");
        // End //

        ActorState changeToState = new ActorState("Empty");
        Vector2 changeToDirection;
        public void SetPlayerState(ActorState newState, GameControler.PlayerEventArgs e)
        {
            if (canChangeState)
            {
                FacingDirection = e.Direction.GetVector(FacingDirection);
                State = newState;

            }
            else if (newState.Name == JumpingState.Name || newState.Name == WalkingState.Name)
            {
                if (e.Direction.GetName() != GameControler.Directions.Static.GetName())
                {
                    FacingDirection = e.Direction.GetVector(FacingDirection);
                    var s = Math.Sign(PolyBody.Velocity.X);
                    var t = s * MaxFlySpeed - PolyBody.Velocity.X;
                    if (Math.Abs(t) <= MaxFlySpeed) //            
                    {
                        PolyBody.Velocity += FacingDirection * new Vector2(MaxFlySpeed / 10, 0);
                        // move.f = WalkForce * FacingDirection * Vector2.UnitX;
                    }
                    else if (Math.Abs(PolyBody.Velocity.X) >= MaxFlySpeed)
                    {
                        PolyBody.Velocity = new Vector2(FacingDirection.X * MaxFlySpeed, PolyBody.Velocity.Y);
                    }
                }
            }
            else
            {
                changeToDirection = e.Direction.GetVector(FacingDirection);
                changeToState = newState;
            }
        }

        public Player(Vector2 position)
        {
            Scale = .5f;
            PolyBody = new PolyBody(1, 4);
            PolyBody.Position = position;
            PolyBody.Rotation = 0;  
            PolyBody.Friction = .15f;
            PolyBody.CanSleep = false;
            PolyBody.IsRotationFixed = true;
            Load();            
        }
        public Player(float x, float y)
            : this(new Vector2(x, y)){}

        public Player(Vector2 position, float scale)
            :this(position)
        {
            Scale = scale;
        }
        public Player(float x, float y, float scale)
            : this(new Vector2(x, y), scale)
        {
        }

        public Vector2 FacingDirection { get; set; }
        public override void Load()
        {
            this.State = Player.StandingState;
            FacingDirection = Vector2.UnitX;

            StateChanged += new ParameterChangeEvent(Player_StateChanged);
            this.PolyBody.Collided += new PhysicsEventHandler(PolyBody_Collided);
            //PolyBody.MakeFixedRotation(); 
            //PolyBody.SetInertiaMoment(1000);
            PolyBody.CanSleep = false;
            //PolyBody.IsRotationFixed = true;
            base.Load();
        }

        public static bool canChangeState = false;
        void PolyBody_Collided(RigidBody sender, object delta)
        {
            var colD = (CollisionEventData)delta;
            var axis = colD.Axis;

            if ((axis + Vector2.UnitY).LengthSquared() < 0.01f)
            {
                canChangeState = true;
                if (changeToState.Name != "Empty")
                {
                    State = changeToState;
                    FacingDirection = changeToDirection;

                    changeToState = new ActorState("Empty");
                }
            }
        }

        void Player_StateChanged(object previous)
        {
            // Below Code will only work when friction is fixed
            /*
            var prevState = (ActorState)previous;
            if (State.Name == WalkingState.Name)
                PolyBody.Velocity += Vector2.UnitX * MaxWalkSpeed * FacingDirection;
            if (prevState.Name == WalkingState.Name)
                PolyBody.Velocity -= Vector2.UnitX * MaxWalkSpeed * FacingDirection;
            */


            if (State.Name == WalkingState.Name)
            {
                if (MaxWalkSpeed - Math.Abs(PolyBody.Velocity.X) > 0)
                {
                    PolyBody.Velocity += FacingDirection * new Vector2(MaxWalkSpeed / 10, 0);
                   // move.f = WalkForce * FacingDirection * Vector2.UnitX;
                }
                else if (Math.Abs(PolyBody.Velocity.X) >= MaxWalkSpeed)
                {
                    PolyBody.Velocity = new Vector2(FacingDirection.X * MaxWalkSpeed, PolyBody.Velocity.Y);
                }
                //PolyBody.AddForce(move);

            }
            else if (State.Name == StandingState.Name)
            {
                PolyBody.Velocity *= Vector2.UnitY;
            }

            ////You can double jump, triple jump, or even n jump. Need to figure out a way to fix this.
            if (State.Name == JumpingState.Name && canChangeState)
            {
                    Force jump = new Force();
                    jump.t = ForceType.Spring;
                    jump.f = Vector2.UnitY * JumpForce;
                    PolyBody.AddForce(jump);
                    canChangeState = false;

            }
            else 
            {
                GraphicsEngine.ResetAnimations();    
            }

        }

        public override void setupGraphics()
        {
            Sprite = new Sprite("Player", State.Name, Scale);
        }

        public override void setupPhysics()
        {
            PolyBody.MakeRect(Sprite.BoundingBox().X, Sprite.BoundingBox().Y);
        }

        public void Update(GameTime gameTime)
        {
        }

        public override void actUpon()
        {
            throw new Exception("Player collided with himself.");
        }


        public Player Clone()
        {
            return new Player(PolyBody.Position);
        }

        protected override string ParseStandardConstructor()
        {
            throw new Exception("The Player Cannot Be Loaded From File");
        }
    }
}
