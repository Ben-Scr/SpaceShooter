using UnityEngine;

namespace SpaceShooter
{
    public class WeaponInstance
    {
        public Weapon Weapon;
        public float LastCallTimeCounter;

        public WeaponInstance(Weapon weapon)
        {
            Weapon = weapon;
            LastCallTimeCounter = weapon.CallInterval;
        }

        public bool CanCall()
        {
            LastCallTimeCounter += Time.deltaTime;
            bool canCall = LastCallTimeCounter >= Weapon.CallInterval;
            return canCall;
        }
    }
}
