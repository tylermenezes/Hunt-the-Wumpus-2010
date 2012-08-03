using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HuntTheWumpus.Utilities;


namespace HuntTheWumpus.GraphicsCore
{
    public class Sprite
    {
        private Animation _animation;
       
        private String _lookUpName;
        public Vector2 Position {get; private set;}
        public float Roation {get; private set;}        
        public float Scale { get; private set; }
        public SpriteEffects spriteEffects{ get; private set; }


        public Sprite(String actorName, string actorState, float scale)
        {        
            spriteEffects = SpriteEffects.None;

            Scale = scale;

            string[] split = actorName.Split('.');
            _lookUpName = split[split.Length - 1] + "-" + actorState; //this seems silly but IDK it works

            if (AnimationRepository.Animations.ContainsKey(_lookUpName))
                _animation = (Animation)AnimationRepository.Animations[_lookUpName];
            else
                _animation = (Animation) AnimationRepository.Animations["none"];

            

        }


        public Sprite(String actorName, string actorState, float scale, Vector2 position, float roation)
            :this(actorName, actorState, scale)
        {
            Position = position;
            Roation = roation;
        }

        public Sprite(String actorName, string actorState, float scale, Vector2 position, float roation, Vector2 direction, String weapon)
        {

            spriteEffects = SpriteEffects.None;

            Scale = scale;

            string[] split = actorName.Split('.');
            _lookUpName = split[split.Length - 1] + "-" + actorState; //this seems silly but IDK it works
            _lookUpName += "-" + weapon;
            if (AnimationRepository.Animations.ContainsKey(_lookUpName))
                _animation = (Animation)AnimationRepository.Animations[_lookUpName];
            else
                _animation = (Animation)AnimationRepository.Animations["none"];

            Position = position;
            Roation = roation;

            if (direction.X < 0)
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public Animation currentAnimation()
        {
            return _animation;
        }

        public Vector2 BoundingBox()
        {
            Vector2 ConvertToPhysics;
            ConvertToPhysics = new Vector2( _animation.SourceBox.X * Scale/ 60f, _animation.SourceBox.Y* Scale/ 60f);
            return ConvertToPhysics;
        }


    }
}
