using System;
using ShotShooter.Assets.Scripts.Damageables;

namespace ShotShooter.Assets.Scripts.Managers
{
    public static class EventManager
    {
        public static event Action<IDamageable> TargetHit;

        public static void OnTargetHit(IDamageable damageable)
        {
            TargetHit?.Invoke(damageable);
        }
    }
}
