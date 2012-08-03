using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus.GameEngineCore
{
    struct ActorState
    {
        public string Name
        {
            get { return _name; }   
        }
        private string _name;
        public ActorState(string name)
        {
            _name = name;
        }
    }
}
