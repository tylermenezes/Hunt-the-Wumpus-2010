using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus.GameEngineCore.GameStates
{
    interface IPlayable
    {
        Map ActiveMap { get; set;  }
        PhysicsCore.PhysicsEngine PhysicsManager{ get; }
        UICore.UIEngine UIManager { get; }
        PlayerProfile ActiveProfile { get; }
        void Reload();

        void EndGame();
    }
}
