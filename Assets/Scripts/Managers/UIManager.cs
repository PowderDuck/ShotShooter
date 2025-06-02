using DG.Tweening;
using ShotShooter.Assets.Scripts.Damageables;
using UnityEngine;
using UnityEngine.UI;

namespace ShotShooter.Assets.Scripts.Managers
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private Image _hitEffectImage = default!;
        [SerializeField] private float _flashDuration = 0.5f;
        [SerializeField] private float _maxAlpha = 0.2f;

        private Tweener _hitEffectTween { get; set; } = default!;

        private Color _hitEffectColor = Color.white;

        private void Start()
        {
            _hitEffectColor = _hitEffectImage.color;

            EventManager.TargetHit += OnTargetHit;
        }

        private void OnTargetHit(IDamageable damageable)
        {
            _hitEffectTween?.Kill();

            _hitEffectTween = DOVirtual.Float(
                _maxAlpha,
                0,
                _flashDuration,
                HitEffectFlashing);

            _ = damageable;
        }

        private void HitEffectFlashing(float transparency)
        {
            _hitEffectColor.a = transparency;
            _hitEffectImage.color = _hitEffectColor;
        }

        private void OnDisable() => EventManager.TargetHit -= OnTargetHit;
    }
}
