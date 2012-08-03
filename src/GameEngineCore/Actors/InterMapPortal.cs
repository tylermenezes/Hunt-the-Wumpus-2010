using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    class InterMapPortal : Portal
    {
        public Map DestinationMap { get; set; }
        public string DestinationPortalID { get; set; }


        public InterMapPortal(string id, float x, float y)
            : base(id, x, y)
        {
            GC.Collect();
        }

        public InterMapPortal(string id, Map destinationMap, string destinationPortalID, float x, float y)
            : this(id, x, y)
        {
            DestinationMap = destinationMap;
            DestinationPortalID = destinationPortalID;
        }

        public override void Enter(Actor transportee)
        {
            if (DestinationMap != null)
            {
                if (transportee is Actors.Player)
                {
                    transportee.PolyBody.Velocity = Microsoft.Xna.Framework.Vector2.Zero;
                    transportee.PolyBody.AngularVelocity = 0;
                    if (DestinationMap == null)
                        GameEngine.Singleton.ActiveState.Exit();
                    else
                        DestinationMap.Load(DestinationPortalID);
                }
                else
                    throw new Exception("Only Players can Pass Through InterMapPortal");
            }
        }
    }
}
