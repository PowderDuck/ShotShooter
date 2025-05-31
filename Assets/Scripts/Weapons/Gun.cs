using DG.Tweening;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Weapons
{
    public class Gun : Weapon
    {
        private Tween _fireTween { get; set; } = default!;

        public override void Shoot()
        {
            base.Shoot();

            _fireTween = DOVirtual.DelayedCall(FireRate, Shoot);
        }

        protected override void Fire()
        {
            Debug.Log("Gun Fired !");
        }
    }
}
