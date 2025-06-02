using DG.Tweening;
using ShotShooter.Assets.Scripts.Controllers;
using ShotShooter.Assets.Scripts.Damageables;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Weapons
{
    public class ControlledWeapon : Weapon
    {
        [SerializeField] private float _maxDistance = 10;

        [SerializeField] protected int _overshoot = 10;
        [SerializeField] protected float _cooldownDuration = 0.5f;

        [SerializeField] protected GameObject _hitEffectPrefab = default!;

        // The controlled horizontal and vertical displacement of the weapon aim
        [SerializeField] protected AnimationCurve _horizontalCurve = default!;
        [SerializeField] protected AnimationCurve _verticalCurve = default!;

        protected int _currentOvershoot { get; set; } = 0;

        private Tween _cooldownTween { get; set; } = default!;

        private RaycastHit _hitInfo = default!;
        private Ray _ray = new();

        protected override void Start()
        {
            base.Start();

            WeaponReady += ResetOvershoot;
        }

        protected override void Fire()
        {
            _cooldownTween?.Kill();
            _currentOvershoot++;

            CameraShake();

            _ray.origin = CameraController.Instance.transform.position;
            _ray.direction = CameraController.Instance.transform.forward;
            if (Physics.Raycast(_ray, out _hitInfo, _maxDistance))
            {
                if (_hitInfo.collider != null)
                {
                    if (_hitInfo.collider.TryGetComponent<IDamageable>(
                        out var damageable))
                    {
                        damageable.TakeDamage(Damage);
                        HitTarget(damageable);
                    }

                    var hitEffect = Instantiate(_hitEffectPrefab);
                    hitEffect.transform.position = _hitInfo.point;
                    /*var hitEffect = Pooler.Instance
                        .GetOrCreateObject<ParticleSystem>(_hitEffectPrefab);
                    hitEffect.transform.position = hitInfo.point;*/
                }
            }
        }

        private void CameraShake()
        {
            var overshootPercentage = Mathf.Min((float)_currentOvershoot / _overshoot, 1);
            CameraController.Instance.Shake(
                new(_verticalCurve.Evaluate(overshootPercentage) * _recoil,
                    _horizontalCurve.Evaluate(overshootPercentage) * _recoil), _fireRate);
        }

        private void ResetOvershoot()
        {
            _cooldownTween?.Kill();
            _cooldownTween = DOVirtual.DelayedCall(
                _cooldownDuration, () => _currentOvershoot = 0);
        }

        private void OnDisable()
        {
            WeaponReady -= ResetOvershoot;
        }
    }
}
