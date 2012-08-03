using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    class InterMapPortal : Portal
    {
        public InterMapPortal(string id)
            : base(id, String.Empty, 0, 0)
        {
        }
        public InterMapPortal(string id, string partnerAddress)
            : base(id, partnerAddress, 0, 0)
        {
        }
        public InterMapPortal(string id, string partnerAddress, float x, float y)
            : base(id, partnerAddress, x, y)
        {
        }

        public override void Enter(Actor transportee)
        {
            if (transportee is Actors.Player)
            {
                transportee.PolyBody.Velocity = Microsoft.Xna.Framework.Vector2.Zero;
                transportee.PolyBody.AngularVelocity = 0;

                var game = GameEngine.Singleton.GetPlayState();
                if (PartnerAddress == String.Empty)
                    game.EndGame();
                else
                {
                    string[] address = PartnerAddress.Split('.');
                    game.ActiveProfile.SetCurrentMap(address[0], address[1], true);
                }
            }
            else
                throw new Exception("Only Players can Pass Through InterMapPortal");            
        }

        protected override string ParseStandardConstructor()
        {
            return String.Format(
                @"<string>{0}</string>
                  <string>{1}</string>",
                ID, PartnerAddress);
        } 
    }
}
