using System;
using DG.Tweening;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Utils
{
    public class DisposableEffect : MonoBehaviour
    {
        public event Action<object> Finished;

        private ParticleSystem _particleSystem { get; set; } = default!;

        private void OnEnable()
        {
            _particleSystem ??= GetComponent<ParticleSystem>();
            _particleSystem.Play();

            DOVirtual.DelayedCall(
                _particleSystem.main.duration + _particleSystem.main.startLifetime.constant,
                DisableEffect);
        }

        private void DisableEffect()
        {
            Finished?.Invoke(this);
            Destroy(gameObject);
            // gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            Finished = null;
        }
    }
}
