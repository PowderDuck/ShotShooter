using DG.Tweening;
using UnityEngine;

namespace ShotShooter.Assets.Scripts.Controllers
{
    public class CameraController : Singleton<CameraController>
    {
        [SerializeField] private GameObject _target = default!;
        [SerializeField] private Vector3 _offset = new(2, 1, -4);
        [SerializeField] private Vector3 _zoomOffset = new(2, 1, -2);
        [SerializeField] private float _zoomDuration = 0.5f;

        [SerializeField] private Vector2 _boundaries = new(360f, 90f);
        [SerializeField] private float _rotationSpeed = 1f;

        private Vector2 _currentRotation = Vector2.zero;
        private Vector2 _destinationRotation = Vector2.zero;
        private Vector3 _radialOffset = Vector3.zero;

        private Vector2 _mouseDelta = Vector2.zero;

        private Tweener _zoomTween { get; set; } = default!;
        private Vector3 _currentOffset { get; set; } = default!;

        private void Start()
        {
            _currentOffset = _offset;
        }

        private void Update()
        {
            UpdateRotation();

            if (_target != null)
            {
                transform.position = _target.transform.position
                    + (RadialOffset() * _currentOffset.magnitude);
                /*transform.position = _target.transform.position
                    + _currentOffset + (RadialOffset() * _currentOffset.magnitude);*/
            }
        }

        private void UpdateRotation()
        {
            _mouseDelta.Set(
                Input.GetAxis("Mouse X"),
                Input.GetAxis("Mouse Y"));

            _destinationRotation += _mouseDelta;
            _destinationRotation.Set(
                Mathf.Clamp(_destinationRotation.x % 360f, -_boundaries.x, _boundaries.x),
                Mathf.Clamp(_destinationRotation.y, -_boundaries.y, _boundaries.y));

            var difference = _destinationRotation - _currentRotation;
            _currentRotation.Set(
                _currentRotation.x + (Mathf.Clamp(difference.x / _rotationSpeed, -1f, 1f) * _rotationSpeed),
                _currentRotation.y + (Mathf.Clamp(difference.y / _rotationSpeed, -1f, 1f) * _rotationSpeed));

            transform.eulerAngles = new(-_currentRotation.y, _currentRotation.x);
        }

        public void SetTarget(GameObject target) => _target = target;

        public void Shake(float amplitude)
        {
            transform.DOShakeRotation(0.2f);
        }

        public void Zoom(bool zoom)
        {
            var duration = _zoomDuration;
            if (_zoomTween != null && _zoomTween.IsPlaying())
            {
                duration = _zoomDuration - _zoomTween.Elapsed();
            }

            _zoomTween.Kill();
            _zoomTween = DOVirtual.Vector3(
                _currentOffset,
                zoom ? _zoomOffset : _offset,
                duration,
                position => _currentOffset = position)
                .SetEase(Ease.Linear);
        }

        private Vector3 RadialOffset()
        {
            var horizontal = new Vector3(
                Mathf.Sin(_currentRotation.x * Mathf.Deg2Rad),
                Mathf.Cos(_currentRotation.x * Mathf.Deg2Rad),
                0f);

            return (horizontal * Mathf.Sin(_currentRotation.y * Mathf.Deg2Rad))
                + new Vector3(0f, Mathf.Cos(_currentRotation.y * Mathf.Deg2Rad), 0f);
        }
    }
}
