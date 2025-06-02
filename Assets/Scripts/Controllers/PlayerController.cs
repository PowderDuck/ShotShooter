using System;
using ShotShooter.Assets.Scripts.Damageables;
using ShotShooter.Assets.Scripts.Managers;
using ShotShooter.Assets.Scripts.Weapons;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _weaponHolder = default!;
        [SerializeField] private float _weaponThrowForce = 100f;

        [SerializeField] private float _movementSpeed = 10;
        [SerializeField] private float _rotationSpeed = 10;

        private Weapon _currentWeapon = default!;

        private Vector3 _movement = Vector3.zero;
        private Rigidbody _rigidbody { get; set; } = default!;
        private Animator _animator { get; set; } = default!;

        private Vector3 _currentRotation = Vector3.zero;
        private Vector3 _destinationRotation = Vector3.zero;

        private int _aimingLayerIndex { get; set; } = 0;

        public event Action<Weapon> WeaponChanged;
        public event Action<IDamageable> TargetHit;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();

            _animator = GetComponent<Animator>();
            _aimingLayerIndex = _animator.GetLayerIndex("Aiming Layer");

            TargetHit += EventManager.OnTargetHit;
        }

        private void Update()
        {
            HandleWeapon();
            HandleZoom();

            Rotate();
        }

        private void FixedUpdate() => Move();

        private void HandleWeapon()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _currentWeapon?.Shoot();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _currentWeapon?.Release();
            }
        }

        private void HandleZoom()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                CameraController.Instance.Zoom(true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                CameraController.Instance.Zoom(false);
            }
        }

        private void Move()
        {
            _movement.Set(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical"));

            _rigidbody.velocity = _movementSpeed
                * ((_movement.x * transform.right) + (_movement.z * transform.forward));

            if (_movement.magnitude > 0)
            {
                _destinationRotation.Set(
                    transform.eulerAngles.x,
                    CameraController.Instance.transform.eulerAngles.y,
                    transform.eulerAngles.z);
            }

            _animator.SetFloat("movement", _movement.magnitude);
        }

        private void Rotate()
        {
            var differenceX = Mathf.DeltaAngle(_currentRotation.x, _destinationRotation.x);
            var differenceY = Mathf.DeltaAngle(_currentRotation.y, _destinationRotation.y);
            _currentRotation.Set(
                _currentRotation.x + (Mathf.Clamp(differenceX / _rotationSpeed, -1f, 1f) * _rotationSpeed),
                _currentRotation.y + (Mathf.Clamp(differenceY / _rotationSpeed, -1f, 1f) * _rotationSpeed),
                transform.eulerAngles.z);

            transform.eulerAngles = _currentRotation;
        }

        public void SetWeapon(Weapon weapon)
        {
            if (_currentWeapon != null)
            {
                ManageWeaponComponents(true);

                _currentWeapon
                    .GetComponent<Rigidbody>()
                    .AddForce(new Vector3(0, 1, 1) * _weaponThrowForce);
            }

            _currentWeapon = weapon;

            var hasWeapon = _currentWeapon != null;
            _animator.SetLayerWeight(
                _aimingLayerIndex, hasWeapon ? 1 : 0);

            if (hasWeapon)
            {
                ManageWeaponComponents(false);
                _currentWeapon.transform
                    .SetLocalPositionAndRotation(Vector3.zero, Quaternion.LookRotation(-_weaponHolder.up));
            }

            WeaponChanged?.Invoke(_currentWeapon);

            // TODO: Pickup animation
        }

        private void OnTargetHit(IDamageable damageable)
        {
            TargetHit?.Invoke(damageable);
        }

        private void ManageWeaponComponents(bool enable)
        {
            if (enable)
            {
                _currentWeapon.TargetHit -= OnTargetHit;
            }
            else
            {
                _currentWeapon.TargetHit += OnTargetHit;
            }

            _currentWeapon.transform.SetParent(enable ? null : _weaponHolder);

            _currentWeapon.GetComponentsInChildren<Collider>()[0].enabled = enable;

            var rigidbody = _currentWeapon.GetComponent<Rigidbody>();
            rigidbody.useGravity = enable;
            rigidbody.isKinematic = !enable;
        }

        private void OnDisable()
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.TargetHit -= OnTargetHit;

                TargetHit -= EventManager.OnTargetHit;
            }
        }
    }
}
