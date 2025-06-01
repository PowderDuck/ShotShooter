using System;
using ShotShooter.Assets.Scripts.Weapons;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Weapon _currentWeapon = default!;
        [SerializeField] private Transform _weaponHolder = default!;

        [SerializeField] private float _movementSpeed = 10;

        private Vector3 _movement = Vector3.zero;
        private Rigidbody _rigidbody { get; set; } = default!;
        private Animator _animator { get; set; } = default!;

        private Vector3 _dynamicRotation = Vector3.zero;

        public event Action<Weapon> WeaponChanged;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _currentWeapon?.Shoot();
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _currentWeapon?.Release();
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                CameraController.Instance.Zoom(true);
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                CameraController.Instance.Zoom(false);
            }
        }

        private void FixedUpdate() => Movement();

        private void Movement()
        {
            _movement.Set(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical"));

            _rigidbody.velocity = _movementSpeed
                * ((_movement.x * transform.right) + (_movement.z * transform.forward));

            _dynamicRotation.Set(
                transform.eulerAngles.x,
                CameraController.Instance.transform.eulerAngles.y,
                transform.eulerAngles.z);
            transform.eulerAngles = _dynamicRotation;

            _animator.SetFloat("movement", _movement.magnitude);
        }

        public void SetWeapon(Weapon weapon)
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.transform.SetParent(null);
                ManageWeaponComponents(true);
            }

            _currentWeapon = weapon;
            _currentWeapon.transform.SetParent(_weaponHolder);
            _currentWeapon.transform
                .SetLocalPositionAndRotation(Vector3.zero, Quaternion.LookRotation(-_weaponHolder.up));
            ManageWeaponComponents(false);

            WeaponChanged?.Invoke(_currentWeapon);

            // TODO: Pickup animation
        }

        private void ManageWeaponComponents(bool enable)
        {
            _currentWeapon.GetComponent<Collider>().enabled = enable;
            _currentWeapon.GetComponent<Rigidbody>().useGravity = enable;
            _currentWeapon.GetComponent<Rigidbody>().isKinematic = !enable;
        }
    }
}
