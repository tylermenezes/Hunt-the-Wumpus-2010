using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


using HuntTheWumpus.Utilities;
using HuntTheWumpus.UICore.Controls;
using HuntTheWumpus.UICore;

namespace HuntTheWumpus.UICore.Controls
{
    public class PowerBar: Control 
    {
        private const float maxPowerValue = 20;
        private Size currentWindowSize = (Size)Utilities.DefaultSettings.Settings["WindowSize"];
        private const float powerBarHeight = 10;
        private float powerBarLength;
        private Texture2D _image = (Texture2D)Utilities.ContentRepository.Content["PowerBar"];
        private const float maxPowerBarLength = 250;
        public PowerBar():
            base(Vector2.Zero, Vector2.Zero)
        {
            powerBarLength = maxPowerBarLength;
            this.Location = new Vector2(currentWindowSize.X/2, 44);
        }

        public void updatePower(float amountOfPowerLeft)
        {
            if (amountOfPowerLeft >= 0)
                powerBarLength = amountOfPowerLeft / maxPowerValue * maxPowerBarLength; 
      
            if (amountOfPowerLeft < 0)
            {
                powerBarLength = Math.Abs(amountOfPowerLeft)/ maxPowerValue * maxPowerBarLength;
                this.Location = new Vector2(maxPowerBarLength - (powerBarLength), this.Location.Y );
            }
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_image, new Rectangle((int) this.Location.X, (int) this.Location.Y, (int)powerBarLength, (int)powerBarHeight), Color.White);
            base.Draw(time, spriteBatch);
        }

        

    }
}
