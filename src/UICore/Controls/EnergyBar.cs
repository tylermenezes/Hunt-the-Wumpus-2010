using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.UICore.Controls
{
    public class EnergyBar: Control
    {
        public Texture2D _image;
        private const float energyBarHeight = 10;
        private float energyBarLength; 
        private const float totalEnergy = 20f;
        private Size currentWindowSize = (Size)Utilities.DefaultSettings.Settings["WindowSize"];
        private float maxEnergyValue = 570; 

        public EnergyBar() :
            base(Vector2.Zero, Vector2.Zero)
        {
            _image = (Texture2D)Utilities.ContentRepository.Content["EnergyBar"];
            energyBarLength = maxEnergyValue; 
            this.Location = new Vector2(70,60); 
            
        }

        public void updateEnergy(float amountOfEnergyLeft)
        {
            energyBarLength = amountOfEnergyLeft / totalEnergy * maxEnergyValue ;      
        }

        public override void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_image, new Rectangle( (int) this.Location.X ,(int) this.Location.Y, (int) energyBarLength, (int) energyBarHeight), Color.White);
            base.Draw(time, spriteBatch);
        }
        
    }
}
