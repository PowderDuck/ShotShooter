using System;
using DG.Tweening;
using ShotShooter.Assets.Scripts.Controllers;
using ShotShooter.Assets.Scripts.Damageables;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Weapons
{
    public abstract class Weapon : MonoBehaviour, IWeapon
    {
        [field: SerializeField]
        public float Damage { get; private set; }

        [SerializeField] protected int _magazineCapacity = 20;
        [SerializeField] protected int _maxAmmo = 60;
        [SerializeField] protected float _fireRate = 0.5f;
        [SerializeField] protected float _recoil = 1;
        [SerializeField] private AudioClip _fireSound = default!;

        [SerializeField] protected float _reloadDuration = 1;

        [SerializeField] private Vector3 _aimingRotation = new(-90, -120, 0);

        protected int _currentMagazine { get; set; } = 0;
        protected int _currentAmmo { get; set; } = 0;

        private bool _isFiring { get; set; } = false;
        private bool _isReloading { get; set; } = false;

        private AudioSource _audioSource { get; set; } = default!;

        private PlayerController _enteredPlayer { get; set; } = default!;

        public event Action WeaponReady;
        public event Action<IDamageable> TargetHit;

        private const float PickupDelay = 0.5f;

        protected virtual void Start()
        {
            _currentMagazine = _magazineCapacity;
            _currentAmmo = _maxAmmo;

            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _fireSound;
        }

        public virtual void Shoot()
        {
            if (_isFiring || _isReloading)
            {
                return;
            }

            if (_currentMagazine <= 0 && _currentAmmo <= 0)
            {
                // TODO: Play empty magazine sound
                return;
            }

            _isFiring = true;
            _audioSource.Play();
            Fire();

            DOVirtual.DelayedCall(_fireRate, EnableWeapon);

            if (_currentMagazine > 0)
            {
                _currentMagazine--;
                if (_currentMagazine <= 0 && _currentAmmo > 0)
                {
                    _isReloading = true;
                    DOVirtual.DelayedCall(_reloadDuration, Reload);
                }
            }
        }

        public virtual void Release() { }

        protected abstract void Fire();

        public virtual void Reload()
        {
            var supplyAmmo = Mathf.Min(
                _currentAmmo, _magazineCapacity - _currentMagazine);

            if (supplyAmmo > 0)
            {
                _currentMagazine += supplyAmmo;
                _currentAmmo -= supplyAmmo;
            }

            _isReloading = false;
        }

        private void EnableWeapon()
        {
            _isFiring = false;
            WeaponReady?.Invoke();
        }

        protected void HitTarget(IDamageable damageable)
        {
            TargetHit?.Invoke(damageable);
        }

        protected virtual void OnTriggerEnter(Collider collider)
        {
            if (_enteredPlayer != null)
            {
                return;
            }

            if (collider.TryGetComponent<PlayerController>(out var player))
            {
                player.SetWeapon(this);

                transform.localEulerAngles = _aimingRotation;

                _enteredPlayer = player;
                _enteredPlayer.WeaponChanged += OnWeaponChanged;
            }
        }

        protected virtual void OnWeaponChanged(Weapon weapon)
        {
            if (_enteredPlayer != null && weapon != this)
            {
                DOVirtual.DelayedCall(PickupDelay, ResetWeapon);
            }
        }

        private void ResetWeapon()
        {
            _enteredPlayer.WeaponChanged -= OnWeaponChanged;
            _enteredPlayer = null;
        }
    }
}
