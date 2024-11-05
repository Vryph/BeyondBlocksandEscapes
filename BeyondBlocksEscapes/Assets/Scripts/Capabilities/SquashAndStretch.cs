using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace BBE {
    public class SquashAndStretch : MonoBehaviour
    {
        [Header("Notes")]
        [SerializeField, Multiline(2)] private string _notes;

        [Header("Squash and Stretch Core")]
        [SerializeField, Tooltip("Default to current GameObject if not set")] private Transform _targetTransform;
        [SerializeField] private SquashStechAxis _targetAxis = SquashStechAxis.Y;
        [SerializeField, Range(0, 1f)] private float _animationDuration = 0.25f;
        [SerializeField] private bool _canBeOverwritten;
        [SerializeField] private bool _playOnStart;
        [SerializeField] private bool _playsEveryTime = true;
        [SerializeField, Range(0, 100f)] private float _percentChanceToPlay = 100f;

        [Flags]
        public enum SquashStechAxis 
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4
        }

        [Header("Animation Settings")]
        [SerializeField] private float _initialScale = 1f;
        [SerializeField] private float _maximumScale = 1.3f;
        [SerializeField] private bool _resetToInitialScaleAfterAnimation = true;
        [SerializeField] private bool _reverseAnimationCurveAfterPlaying;
        private bool _isReversed;


        [SerializeField] private AnimationCurve _squashAndStretchCurve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, 1f),
            new Keyframe(1f, 0f)
        );


        [Header("Looping Settings")]
        [SerializeField] private bool _looping;
        [SerializeField] private float _loopingDelay = 0.5f;

        private Coroutine _squashAndStretchCoroutine;
        private WaitForSeconds _loopingDelayWaitForSeconds;
        private Vector3 _initialScaleVector;

        private bool _affectX => (_targetAxis & SquashStechAxis.X) != 0;
        private bool _affectY => (_targetAxis & SquashStechAxis.Y) != 0;
        private bool _affectZ => (_targetAxis & SquashStechAxis.Z) != 0;

        private static event Action _squashAndStretchAllObjectsLikeThis;

        private void Awake()
        {
            if(_targetTransform == null)
                _targetTransform = transform;

            _initialScaleVector = _targetTransform.localScale;
            _loopingDelayWaitForSeconds = new WaitForSeconds(_loopingDelay);
        }

        public static void SquashAndStretchAllObjectsLikeThis()
        {
            _squashAndStretchAllObjectsLikeThis?.Invoke();
        }

        private void OnEnable()
        {
            _squashAndStretchAllObjectsLikeThis += PlaySquashAndStretch;
        }

        private void OnDisable()
        {
            if(_squashAndStretchCoroutine != null)
                StopCoroutine(_squashAndStretchCoroutine);

            _squashAndStretchAllObjectsLikeThis -= PlaySquashAndStretch;
        }

        private void Start()
        {
            if (_playOnStart)
                CheckForAndStartCoroutine();
        }

        [ContextMenu("Play Squash and Stretch")]
        public void PlaySquashAndStretch()
        {
            if (_looping && !_canBeOverwritten)
                return;

            CheckForAndStartCoroutine();
        }

        private void CheckForAndStartCoroutine()
        {
            if(_targetAxis == SquashStechAxis.None)
            {
                Debug.Log("Axis to affect is set to None.", gameObject);
                return;
            }

            if (_squashAndStretchCoroutine != null)
            {
                StopCoroutine(_squashAndStretchCoroutine);
                if(_playsEveryTime && _resetToInitialScaleAfterAnimation)
                    transform.localScale = _initialScaleVector;
            }

            _squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
        }

        private IEnumerator SquashAndStretchEffect()
        {
            do
            {
                if (!_playsEveryTime)
                {
                    float _random = UnityEngine.Random.Range(0f, 100f);
                    if (_random > _percentChanceToPlay)
                    {
                        yield return null;
                        continue;
                    }
                }


                if(_reverseAnimationCurveAfterPlaying)
                    _isReversed = !_isReversed;

                float _elapsedTime = 0;
                Vector3 _originalScale = _initialScaleVector;
                Vector3 _modifiedScale = _originalScale;

                while(_elapsedTime < _animationDuration)
                {
                    _elapsedTime += Time.deltaTime;

                    float _curvePosition;

                    if(_isReversed)
                        _curvePosition = 1 - (_elapsedTime / _animationDuration);
                    else
                        _curvePosition = _elapsedTime / _animationDuration;

                    float _curveValue = _squashAndStretchCurve.Evaluate(_curvePosition);
                    float _remappedValue = _initialScale + (_curveValue * (_maximumScale - _initialScale));

                    float _minimumThreshold = 0.0001f;
                    if (Math.Abs(_remappedValue) < _minimumThreshold)
                        _remappedValue = _minimumThreshold;

                    if (_affectX)
                        _modifiedScale.x = _originalScale.x * _remappedValue;
                    else
                        _modifiedScale.x = _originalScale.x / _remappedValue;

                    if (_affectY)
                        _modifiedScale.y = _originalScale.y * _remappedValue;
                    else
                        _modifiedScale.y = _originalScale.y / _remappedValue;

                    if (_affectZ)
                        _modifiedScale.z = _originalScale.z * _remappedValue;
                    else
                        _modifiedScale.z = _originalScale.z / _remappedValue;

                    transform.localScale = _modifiedScale;

                    yield return null;
                }

                if(_resetToInitialScaleAfterAnimation)
                    transform.localScale = _originalScale;

                if (_looping)
                {
                    yield return _loopingDelayWaitForSeconds;
                }
            } while (_looping);
        }
    }
}