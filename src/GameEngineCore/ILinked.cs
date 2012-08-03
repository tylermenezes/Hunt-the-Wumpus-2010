using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    interface ILinked
    {
        string ID { get; }
        string PartnerAddress { get; }
        void Activate();
    }
}
