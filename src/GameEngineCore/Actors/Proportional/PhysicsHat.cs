using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus.GameEngineCore.Actors
{
    /// <summary>
    /// Unlocks a physics weapon when collected, in other words, adds it to the manager
    /// </summary>
    class PhysicsHat : Actor
    {
        /// <summary>
        /// The physics weapon this hat unlocks
        /// </summary>
        public PhysicsWeaponDeployer WeaponUnlocked { get; private set; }

        public const float Radius = .5f;

        public PhysicsHat(string weaponUnlocked) : this(weaponUnlocked, 0, 0)
        {
        }

        public PhysicsHat(string weaponUnlocked, float x, float y)
        {
            Scale = 1f;
            WeaponUnlocked = PhysicsWeaponDeployer.CreateFromName(weaponUnlocked);
            PolyBody = new HuntTheWumpus.PhysicsCore.PolyBody(1, 7);
            PolyBody.Position = new Microsoft.Xna.Framework.Vector2(x, y);
            Load();
        }

        public override void setupGraphics()
        {
            
        }

        public override void setupPhysics()
        {
            PolyBody.MakeRegularFromRad(Radius);
        }

        public override void actUpon()
        {
            var game = GameEngine.Singleton.GetPlayState();
            game.ActiveProfile.PhysicsWeaponInventory.AddWeaponDeployer(WeaponUnlocked);
            Console.WriteLine("You just obtained a " + WeaponUnlocked.GetType().Name);

            this.Kill();
        }

        protected override string ParseStandardConstructor()
        {
            return String.Format("<string>{0}</string>", WeaponUnlocked.Name);
        }
    }
}
