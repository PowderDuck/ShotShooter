using DG.Tweening;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Controllers
{
    public class CameraController : Singleton<CameraController>
    {
        [SerializeField] private GameObject _target = default!;
        [SerializeField] private Vector3 _staticOffset = new(1, 2, 0);
        [SerializeField] private Vector3 _zoomOutOffset = new(2, 1, -4);
        [SerializeField] private Vector3 _zoomInOffset = new(2, 1, -2);
        [SerializeField] private float _zoomDuration = 0.5f;

        [SerializeField] private float _yBoundary = 30f;
        [SerializeField] private float _rotationSpeed = 1f;

        private Vector2 _currentRotation = Vector2.zero;
        private Vector2 _destinationRotation = Vector2.zero;
        private Vector2 _shakeRotation = Vector2.zero;

        private Vector2 _mouseDelta = Vector2.zero;

        private Tweener _zoomTween { get; set; } = default!;
        private Vector3 _currentOffset { get; set; } = default!;

        private Tweener _shakeTween { get; set; } = default!;

        private void Start()
        {
            _currentOffset = _zoomOutOffset;
        }

        private void Update()
        {
            UpdatePositionAndRotation();
        }

        private void UpdatePositionAndRotation()
        {
            _mouseDelta.Set(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y"));

            _destinationRotation += _mouseDelta;
            _destinationRotation.Set(
                _destinationRotation.x,
                Mathf.Clamp(_destinationRotation.y, -_yBoundary, _yBoundary));

            var difference = _destinationRotation - _currentRotation;
            _currentRotation.Set(
                _currentRotation.x + (Mathf.Clamp(difference.x / _rotationSpeed, -1f, 1f) * _rotationSpeed),
                _currentRotation.y + (Mathf.Clamp(difference.y / _rotationSpeed, -1f, 1f) * _rotationSpeed));

            var radial = RadialOffset();
            if (_target != null)
            {
                transform.position = _target.transform.position
                    + (_target.transform.right * _staticOffset.x)
                    + (_target.transform.up * _staticOffset.y)
                    + (_target.transform.forward * _staticOffset.z)
                    + (radial * _currentOffset.magnitude);
            }

            transform.rotation = Quaternion.LookRotation(-radial);
        }

        public void Shake(Vector2 eulerOffset, float duration)
        {
            _shakeTween?.Kill(true);

            var initialRotation = transform.eulerAngles;
            _shakeRotation = new Vector3(
                initialRotation.x + eulerOffset.x,
                initialRotation.y + eulerOffset.y,
                initialRotation.z);

            transform.eulerAngles = _shakeRotation;

            _shakeTween = DOVirtual.Vector3(
                eulerOffset,
                Vector3.zero,
                duration,
                rotation => _shakeRotation = rotation);
        }

        public void Zoom(bool zoom)
        {
            var duration = _zoomDuration;
            if (_zoomTween != null
                && _zoomTween.IsActive()
                && _zoomTween.IsPlaying())
            {
                duration = _zoomDuration - _zoomTween.Elapsed();
            }

            _zoomTween.Kill();
            _zoomTween = DOVirtual.Vector3(
                _currentOffset,
                zoom ? _zoomInOffset : _zoomOutOffset,
                duration,
                position => _currentOffset = position)
                .SetEase(Ease.Linear);
        }

        private Vector3 RadialOffset()
        {
            var rotation = _currentRotation + _shakeRotation;
            var horizontal = new Vector3(
                Mathf.Sin(rotation.x * Mathf.Deg2Rad),
                0f,
                Mathf.Cos(rotation.x * Mathf.Deg2Rad));

            return (horizontal * Mathf.Sin((rotation.y + 90) * Mathf.Deg2Rad))
                + new Vector3(0f, Mathf.Cos((rotation.y + 90) * Mathf.Deg2Rad), 0f);
        }
    }
}
