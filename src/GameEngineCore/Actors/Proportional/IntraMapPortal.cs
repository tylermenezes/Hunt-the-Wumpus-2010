using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    class IntraMapPortal : Portal
    {
        public IntraMapPortal(string id)
            : base(id, String.Empty, 0, 0)
        {
        }
        public IntraMapPortal(string id, string partnerAddress)
            : base(id, partnerAddress, 0, 0)
        {
        }
        public IntraMapPortal(string id, string partnerAddress, float x, float y)
            : base(id, partnerAddress, x, y)
        {
        }

        public override void Enter(Actor transportee)
        {
            if (PartnerAddress != String.Empty)
            {
                string partnerId = PartnerAddress.Split('.')[1];
                var game = GameEngine.Singleton.GetPlayState();
                transportee.PolyBody
                    .Position = game.ActiveMap.LinkedObjects[partnerId].PolyBody
                    .Position;
            }
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
