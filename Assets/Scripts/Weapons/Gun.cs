using UnityEngine;

namespace ShotShooter.Assets.Scripts.Weapons
{
    public class Gun : Weapon
    {
        private bool _isHolding { get; set; } = false;

        protected override void Start()
        {
            base.Start();

            WeaponReady += Reshoot;
        }

        public override void Shoot()
        {
            _isHolding = true;

            base.Shoot();
        }

        public override void Release() => _isHolding = false;

        protected override void Fire()
        {
            Debug.Log("Gun Fired !");
        }

        private void Reshoot()
        {
            if (_isHolding)
            {
                Shoot();
            }
        }

        private void OnDisable()
        {
            WeaponReady -= Reshoot;
        }
    }
}
