using UnityEngine;

namespace ShotShooter.Assets.Scripts.Entities
{
    public class Poolable
    {
        public GameObject Target { get; }
        public bool IsAvailable { get; private set; } = true;

        public Poolable(GameObject target)
        {
            Target = target;
        }

        public GameObject Lend()
        {
            IsAvailable = false;

            return Target;
        }

        public void Return() => IsAvailable = true;
    }
}
