using System;
using ShotShooter.Assets.Scripts.Weapons;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Weapon _currentWeapon = default!;

        public event Action<Weapon>? WeaponChanged;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _currentWeapon?.Shoot();
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

        public void SetWeapon(Weapon weapon)
        {
            _currentWeapon = weapon;
            WeaponChanged?.Invoke(_currentWeapon);

            // TODO: Pickup animation
        }
    }
}
