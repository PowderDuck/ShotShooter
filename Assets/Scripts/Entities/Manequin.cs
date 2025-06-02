using ShotShooter.Assets.Scripts.Damageables;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Entities
{
    public class Manequin : MonoBehaviour, IDamageable
    {
        [field: SerializeField]
        public float Health { get; private set; } = 10;

        private Animator _animator { get; set; } = default!;

        private int _hitIndex { get; set; } = 0;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            _animator.SetTrigger(
                Health <= 0 ? "die" : $"take_damage_{++_hitIndex % 2}");

            if (Health <= 0)
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }
}
