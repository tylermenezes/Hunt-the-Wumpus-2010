using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus.Utilities
{
    public static class Globals
    {
        /// <summary>
        /// Global Variable:
        ///     spriteBatch
        ///     GraphicsDeviceManager
        ///     XNAGame
        /// </summary>
        public static Dictionary<string, Object> Variables { get; set; }
        static Globals()
        {
            Variables = new Dictionary<string, object>();
        }

    }
}
