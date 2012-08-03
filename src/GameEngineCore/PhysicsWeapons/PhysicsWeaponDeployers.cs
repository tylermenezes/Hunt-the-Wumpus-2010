using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HuntTheWumpus.GameEngineCore.PhysicsWeapons;
using Microsoft.Xna.Framework;

using HuntTheWumpus.Utilities;

namespace HuntTheWumpus.GameEngineCore
{
    abstract class PhysicsWeaponDeployer
    {
        public const int EnergyMax = 20;
        /// <summary>
        /// Gives the name of the physics weapon
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Tells by how much the weapon's power should change at each keypress
        /// </summary>
        public float PowerScalingFactor { get; private set; }

        /// <summary>
        /// power of weapon
        /// </summary>
        public float PowerValue { get; set; }

        public float EnergyConsumedPerIter = .1f; //prolly change this

        public float EnergyAvailable { get; private set; }
        public float EnergyNeededToActivate
        {
            get
            {
                var delta = Math.Abs(PowerValue - previousPowerValue) / PowerScalingFactor;
                if (delta < 1)
                    delta = 1;

                return delta;
            }
        }
        /// <summary>
        /// Create a new instance of the appropriate physics weapon
        /// </summary>
        /// <returns></returns>
        public virtual void Activate(float weaponPower, Vector2 target)
        {
            EnergyAvailable -= EnergyNeededToActivate;
            if (EnergyAvailable <= 0)
                this.Destroy();
            else
                activate(weaponPower, target);
            previousPowerValue = PowerValue;
            Console.WriteLine(EnergyAvailable);
        }
        protected float previousPowerValue;
        protected abstract void activate(float weaponPower, Vector2 target);
        protected virtual void whileOn(float weaponPower, Vector2 target) { }
        protected virtual void turnedOff(float weaponPower, Vector2 target) { }

        public void WhileOn(float weaponPower, Vector2 target)
        {
            EnergyAvailable -= EnergyConsumedPerIter;
            if (EnergyAvailable <= 0)
                this.Destroy();
            else
                whileOn(weaponPower, target);

            Console.WriteLine(EnergyAvailable);
        }
        public void TurnedOff(float weaponPower, Vector2 target)
        {
            turnedOff(weaponPower, target);
        }

        public virtual void Destroy()
        {
            var game = GameEngine.Singleton.GetPlayState();
            game.ActiveProfile.PhysicsWeaponInventory.RemoveWeaponDeployer(this);
        }

        public PhysicsWeaponDeployer(string name, float powerScalingFactor, float defaultPowerValue)
        {
            EnergyAvailable = EnergyMax;
            Name = name;
            PowerScalingFactor = powerScalingFactor;
            PowerValue = defaultPowerValue;
        }

        internal static PhysicsWeaponDeployer CreateFromName(string name)
        {
            switch (name)
            {
                case "GravityGrenade":
                    return new GravityGrenadeDeployer();
                case "FrictionFlamethrower":
                    return new FrictionFlamethrowerDeployer();
                case "TimeTrident":
                    return new TimeTridentDeployer();
                case "LightLance":
                    return new LightLanceDeployer();
                case "ElectromagneticWhatever":
                    return new ElectromagneticThingamabobDeployer();
                default:
                    throw new Exception("Not a Valid Weapon Name");
            }
        }
    }

    class GravityGrenadeDeployer : PhysicsWeaponDeployer
    {
        public GravityGrenadeDeployer() :
            base("GravityGrenade", .98f, -9.8f) { }

        protected override void activate(float weaponPower, Vector2 target)
        {
            var game = GameEngine.Singleton.GetPlayState();
            var grenade = new GravityGrenade(game.ActiveMap.MainPlayer, weaponPower, target);

            game.ActiveMap.AddGameObject(grenade);
            game.PhysicsManager.AddRigidBody(grenade.PolyBody);
        }
    }
    class FrictionFlamethrowerDeployer : PhysicsWeaponDeployer
    {
        /// <summary>
        /// # ejections per second
        /// </summary>
        public static float Rate = 30f;
        public static float maxAge = 3f;
        public static float Velocity = 15f;

        float n_acc = 0f;

        public FrictionFlamethrowerDeployer() :
            base("FrictionFlamethrower", .98f, -9.8f) { }

        public override void Activate(float weaponPower, Vector2 target)
        {
            //base.Activate(weaponPower, target);
        }
        protected override void activate(float weaponPower, Vector2 target)
        {
            //never gets called, since Activate is overriden, LLKOLLOOLL
        }

        protected override void whileOn(float weaponPower, Vector2 target)
        {
            var game = (GameEngineCore.GameStates.IPlayable)GameEngine.Singleton.ActiveState;
            var pos = game.ActiveMap.MainPlayer.PolyBody.Position + game.ActiveMap.MainPlayer.FacingDirection * 1f;

            n_acc += 1 / 60f * Rate;

            while (n_acc >= 1)
            {
                n_acc -= 1f;

                var ball = new FlameBall(pos, Vector2.Normalize(target - pos) * Velocity, maxAge);
                game.ActiveMap.AddGameObject(ball);
                game.PhysicsManager.AddSpirit(ball.PolyBody);
            }
        }
        protected override void turnedOff(float weaponPower, Vector2 target)
        {
            //
        }
    }

    class TimeTridentDeployer : PhysicsWeaponDeployer
    {
        public TimeTridentDeployer() :
            base("TimeTrident", .1f, 1f) { }

        protected override void activate(float weaponPower, Vector2 target)
        {
            var game = GameEngine.Singleton.GetPlayState();
            foreach (Actor actor in game.ActiveMap.GameObjects)
            {
                if (!(actor is Actors.Player) &&
                    PhysicsCore.CollisionEngine
                        .TestPointInPoly(actor.PolyBody, target))
                {
                    if (actor is Actors.Portal)
                        return; // DO Cool Time Portal Stuff
                    else actor.PolyBody.Time = this.PowerValue;
                }
            }
        }
    }
    class LightLanceDeployer : PhysicsWeaponDeployer
    {
        public LightLanceDeployer() :
            base("LightLance", 0f, 5f) { }

        protected override void activate(float weaponPower, Vector2 target)
        {
            var game = GameEngine.Singleton.GetPlayState();
            string id = game.ActiveMap.LinkedObjects.Count +
                    "_LightPortal_" + this.EnergyAvailable;

            game.ActiveMap.AddGameObject(
                new Actors.IntraMapPortal(
                    id, "." + id + "_partner", target.X, target.Y));

            Random ran = new Random();

            game.ActiveMap.AddGameObject(
                new Actors.IntraMapPortal(
                    id + "_partner", "." + id,
                    (float)ran.NextDouble() * ((GameStates.TheGameState)game).Size.Width,
                    (float)ran.NextDouble() * ((GameStates.TheGameState)game).Size.Height));

        }
    }
    class ElectromagneticThingamabobDeployer : PhysicsWeaponDeployer
    {
        public ElectromagneticThingamabobDeployer() :
            base("ElectromagneticWhatever", .98f, -9.8f) { }

        protected override void activate(float weaponPower, Vector2 target)
        {
            throw new NotImplementedException();
        }
    }
}
