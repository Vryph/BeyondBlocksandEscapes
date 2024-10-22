using UnityEngine;

namespace BBE
{
    public class CameraManager : MonoBehaviour
    {
        enum CameraMode { Follow, Static }

        [SerializeField, Range(0.5f, 2f)] private float _dampDistance = 0.5f;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField, Range(0.2f, 10f)] private float _duration = 0.5f;
        [SerializeField, Range(0.1f, 5f)] private float _yOffset = 2.5f; 

        private Camera _camera;
        private Rigidbody2D _body;
        private Controller _controller;
        private CollisionDataRetrieval _collisionDataRetriever;
        private float _direction, _hInput, _elapsedTime, _normalizedTime, _t;
        [SerializeField] private Vector2 _clampMinValues, _clampMaxValues;
        [SerializeField] private Vector3 _staticPosition;
        private Vector3 _dampPoint, _defaultPosition;
        private bool _isMoving, _isStaticPositionSet;

        [SerializeField] private CameraMode _cameraMode = CameraMode.Follow;

        private void Awake()
        {
            _camera = Camera.main;
            _body = GetComponent<Rigidbody2D>();
            _controller = GetComponent<Controller>();
            _collisionDataRetriever = GetComponent<CollisionDataRetrieval>();
        }


        private void Update()
        {
            if (_cameraMode == CameraMode.Follow)
            {
                CameraFollow();
            }
            else if (_cameraMode == CameraMode.Static) 
            {
                if (!_isStaticPositionSet)
                {
                    CameraStatic();
                }
            }
        }
        public void CameraFollow()
        {

            _hInput = _controller.input.RetrieveMoveInput();

            if (Mathf.Sign(_hInput) != Mathf.Sign(_controller.input.RetrieveMoveInput()))
            {
                _elapsedTime = 0;
            }

            _defaultPosition = new Vector3(_body.position.x, _body.position.y + _yOffset, -10f);

            _elapsedTime += Time.deltaTime;
            _normalizedTime = Mathf.Clamp01(_elapsedTime / _duration);
            _t = _curve.Evaluate(_normalizedTime);



            if (_hInput != 0)
            {
                if (_isMoving == false)
                {
                    _isMoving = true;
                    _elapsedTime = 0;
                }

                _dampPoint = _defaultPosition;
                _dampPoint.x  += _dampDistance * -Mathf.Sign(_hInput);
            }

            if (_isMoving)
            {
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, _dampPoint, _t);

                if (_hInput == 0) 
                {
                    _isMoving = false;

                    if (_collisionDataRetriever.OnGround)
                    { 
                        _elapsedTime = 0;
                    }
                }
            }
            else 
            { 
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, _defaultPosition, _t); 
            }

            if (_camera.transform.position.x < _clampMinValues.x || _camera.transform.position.x > _clampMaxValues.x || _camera.transform.position.y < _clampMinValues.y || _camera.transform.position.y > _clampMaxValues.y)
            {
                _camera.transform.position = new Vector3(Mathf.Clamp(_camera.transform.position.x, _clampMinValues.x, _clampMaxValues.x), Mathf.Clamp(_camera.transform.position.y, _clampMinValues.y, _clampMaxValues.y), _camera.transform.position.z);
            }
            _isStaticPositionSet = false;
            
        }

        private void CameraStatic()
        {
            _camera.transform.position = _staticPosition;
            _isStaticPositionSet = true;
        }
    }

}
