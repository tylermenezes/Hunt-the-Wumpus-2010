using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HuntTheWumpus.GameEngineCore.PhysicsWeapons;
using Microsoft.Xna.Framework;
namespace HuntTheWumpus.GameEngineCore
{ 
    public delegate void PhysicsWeaponManagerEventHandler();
    class PhysicsWeaponManager
    {
        public float WeaponPower 
        {
            get
            {
                return (ActiveWeaponDeployer != null) ?
                    ActiveWeaponDeployer.PowerValue : 0;
            }
            private set
            {
                if (ActiveWeaponDeployer != null)
                    ActiveWeaponDeployer.PowerValue = value * ActiveWeaponDeployer.PowerScalingFactor;
            }
        }
        public float EnergyAvailable
        {
            get
            {
                return (ActiveWeaponDeployer != null) ?
                    ActiveWeaponDeployer.EnergyAvailable : 0;
            }
        }

        public List<PhysicsWeaponDeployer> WeaponDeployers { get; private set; }
        public PhysicsWeaponDeployer ActiveWeaponDeployer 
        {
            get
            {
                if (currentDeployerIndex < 0)
                    currentDeployerIndex = 0;

                if (currentDeployerIndex < WeaponDeployers.Count)
                    return WeaponDeployers[currentDeployerIndex];
                else return null;
            }
        }
        private int currentDeployerIndex;

        public void SwitchWeapon(int direction)
        {
            currentDeployerIndex += direction;
            if (currentDeployerIndex >= WeaponDeployers.Count)
                currentDeployerIndex = 0;
            else if (currentDeployerIndex < 0)
                currentDeployerIndex = WeaponDeployers.Count - 1;

            if (WeaponSwitched != null)
                WeaponSwitched();
        }
        public void ChangeWeaponPower(float amount)
        {
            if (ActiveWeaponDeployer != null)
            {
                WeaponPower += amount * ActiveWeaponDeployer.PowerScalingFactor;
                if (ActiveWeaponDeployer.EnergyNeededToActivate > EnergyAvailable)
                    WeaponPower -= amount * ActiveWeaponDeployer.PowerScalingFactor;
                else if (WeaponPowerChanged != null)
                    WeaponPowerChanged();
            }
        }
        public void AddWeaponDeployer(PhysicsWeaponDeployer deployer)
        {
            WeaponDeployers.Add(deployer);
            WeaponPower = ActiveWeaponDeployer.PowerValue;
            currentDeployerIndex = WeaponDeployers.Count - 1;
            
            if (WeaponDeployerAdded != null)
                WeaponDeployerAdded();
            if (WeaponPowerChanged != null)
                WeaponPowerChanged();
        }

        public void RemoveWeaponDeployer(PhysicsWeaponDeployer deployer)
        {
            var deletedIndex = WeaponDeployers.IndexOf(deployer);
            if (currentDeployerIndex >= deletedIndex)
                SwitchWeapon(-1);

            if (currentDeployerIndex > 0)
                WeaponPower = ActiveWeaponDeployer.PowerValue;
            WeaponDeployers.Remove(deployer);

            if (WeaponDeployerRemoved != null)
                WeaponDeployerRemoved();
        }
        public void DeployWeapon(Vector2 target)
        {
            if (WeaponDeployers.Count > 0)
                ActiveWeaponDeployer.Activate(WeaponPower, target);

            if (WeaponDeployed != null)
                WeaponDeployed();
        }
        public void SetOn(Vector2 target)
        {
            if (WeaponDeployers.Count > 0)
                ActiveWeaponDeployer.WhileOn(WeaponPower, target);

            if (WeaponDeployed != null)
                WeaponDeployed();
        }
        public void SetOff(Vector2 target)
        {
            if (WeaponDeployers.Count > 0)
                ActiveWeaponDeployer.TurnedOff(WeaponPower, target);

            if (WeaponDeployed != null)
                WeaponDeployed();
        }

        public PhysicsWeaponManager()
        {
            WeaponDeployers = new List<PhysicsWeaponDeployer>();
        }

        public event PhysicsWeaponManagerEventHandler WeaponSwitched;
        public event PhysicsWeaponManagerEventHandler WeaponPowerChanged;
        public event PhysicsWeaponManagerEventHandler WeaponDeployerAdded;
        public event PhysicsWeaponManagerEventHandler WeaponDeployerRemoved;
        public event PhysicsWeaponManagerEventHandler WeaponDeployed;
    }
}
