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

        private Tweener _fireTween { get; set; } = default!;

        protected virtual void Start()
        {
            _currentMagazine = MagazineCapacity;
            _currentAmmo = MaxAmmo;
        }

        public virtual void Shoot()
        {
            if (_fireTween != null
                && _fireTween.IsPlaying())
            {
                return;
            }

            if (_currentMagazine <= 0
                && _currentAmmo <= 0)
            {
                // TODO: Play empty magazine sound
                return;
            }

            Fire();

            _fireTween = DOVirtual
                .Float(0, 1, FireRate, null);

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
    }
}
