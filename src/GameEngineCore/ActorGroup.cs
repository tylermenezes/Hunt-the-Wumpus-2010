using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuntTheWumpus.PhysicsCore;
using Microsoft.Xna.Framework;

namespace HuntTheWumpus.GameEngineCore
{
    class ActorGroup
    {
        public List<Actor> actors;
        public List<BinaryForceComponent> bfcs;
    }

    class BounceUnit : ActorGroup
    {
        public BounceUnit(Vector2 pos)
        {
        }
    }
}
