using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.Utilities;
using Microsoft.Xna.Framework;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    abstract class Portal : Actor, ILinked
    {
        public bool Active { get; set; }
        public const float Radius = 2;

        public string ID { get; private set; }
        public string PartnerAddress { get; private set; }

        public Portal(string id, string partnerAddress, float x, float y)
        {
            Scale = .25f;
            PolyBody = new PolyBody(1, 10);
            PolyBody.Position = new Vector2(x, y);
            IsPhysicsable = false;
            
            ID = id;
            PartnerAddress = partnerAddress;

            Active = true;
            Load();
        }
        public override void setupGraphics()
        {
            
        }

        public override void setupPhysics()
        {
            var body = (PolyBody)PolyBody;
            body.MakeRegularFromRad(Radius);
            IsFixed = true;
        }


        public abstract void Enter(Actor transportee);

        public void Activate()
        {
            if (Active)
            {
                var game = GameEngine.Singleton.GetPlayState();
                Enter(game.ActiveMap.MainPlayer);
            }
        }
    }
}
