namespace ShotShooter.Assets.Scripts.Weapons
{
    public class Gun : ControlledWeapon
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

        private void Reshoot()
        {
            if (_isHolding)
            {
                Shoot();
            }
        }

        protected override void OnWeaponChanged(Weapon weapon)
        {
            base.OnWeaponChanged(weapon);

            OnDisable();
        }

        private void OnDisable()
        {
            WeaponReady -= Reshoot;
        }
    }
}
