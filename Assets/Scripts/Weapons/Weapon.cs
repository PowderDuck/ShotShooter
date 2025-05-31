using System;
using DG.Tweening;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [field: SerializeField]
        public int MagazineCapacity { get; private set; }

        [field: SerializeField]
        public int MaxAmmo { get; private set; }

        [field: SerializeField]
        public float FireRate { get; private set; } = 0.5f;

        [SerializeField] protected ParticleSystem _hitEffectPrefab = default!;

        [SerializeField] protected AnimationCurve _verticalCurve = default!;
        [SerializeField] protected AnimationCurve _horizontalCurve = default!;

        protected int _currentMagazine { get; set; } = 0;
        protected int _currentAmmo { get; set; } = 0;

        private bool _isFiring { get; set; } = false;

        public event Action WeaponReady;

        protected virtual void Start()
        {
            _currentMagazine = MagazineCapacity;
            _currentAmmo = MaxAmmo;
        }

        public virtual void Shoot()
        {
            if (_isFiring)
            {
                return;
            }

            if (_currentMagazine <= 0
                && _currentAmmo <= 0)
            {
                // TODO: Play empty magazine sound
                return;
            }

            _isFiring = true;
            Fire();

            DOVirtual.DelayedCall(FireRate, EnableWeapon);

            if (_currentMagazine > 0)
            {
                _currentMagazine--;
                if (_currentMagazine <= 0
                    && _currentAmmo > 0)
                {
                    Reload();
                }
            }
        }

        public virtual void Release() { }

        protected abstract void Fire();

        public virtual void Reload()
        {
            var supplyAmmo = Mathf.Min(
                _currentAmmo, MagazineCapacity - _currentMagazine);

            if (supplyAmmo > 0)
            {
                _currentMagazine += supplyAmmo;
                _currentAmmo -= supplyAmmo;

                // TODO: Play reload animation
            }
        }

        private void EnableWeapon()
        {
            _isFiring = false;
            WeaponReady?.Invoke();
        }
    }
}
