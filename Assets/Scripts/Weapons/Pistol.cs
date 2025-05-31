using ShotShooter.Assets.Scripts.Controllers;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Weapons
{
    public class Pistol : Weapon
    {
        protected override void Fire()
        {
            CameraController.Instance.Shake(30);
            Debug.Log("Pistol Fired !");
        }
    }
}
