using UnityEngine;

namespace BBE
{
    [RequireComponent(typeof(Controller))]
    public class Move : MonoBehaviour
    {
        [Header("Ground Movement")]
        [SerializeField, Range(0f, 100f)] private float _maxSpeed = 4f;
        [SerializeField, Range(0f, 100f)] private float _maxAcceleration = 35f;
        [SerializeField, Range(0f, 100f)] private float _maxAirAcceleration = 20f;

        [Header("Move Particles")]
        [SerializeField, Range(0, 0.2f)] float _particleFormationPeriod;
        [SerializeField, Range(0, 5)] float _particleSpeedThreshold;

        [Header("Ceiling Edge Detection")]
        [SerializeField, Range(0.005f, 0.5f)] private float _ceilingEdgeCorrectionIntensity = 0.1f; //Can just be the same value as Raylenght from raycastDataRetrieval.

        private Controller _controller;
        private CameraManager _cameraManager;
        private Dash _dashInput;
        private Vector2 _direction, _desiredVelocity, _velocity;
        private Rigidbody2D _body;
        private CollisionDataRetrieval _collisionDataRetriever;
        private RaycastDataRetrieval _raycastDataRetriever;

        [Header("Particles")]
        [SerializeField]private ParticleSystem _moveParticleSystem;

        private float _maxSpeedChange, _acceleration, _particleCounter;
        private bool _onGround, _facingRight, _ceilingEdgeDetected;
        private bool _isDashing = false;
        public bool ShouldLock = false, IsLocked = false;

        private void Awake()
        {
            _cameraManager = GetComponent<CameraManager>(); 
            _body = GetComponent<Rigidbody2D>();
            _dashInput = GetComponent<Dash>();
            _collisionDataRetriever = GetComponent<CollisionDataRetrieval>();
            _raycastDataRetriever = GetComponent<RaycastDataRetrieval>();
            _controller = GetComponent<Controller>();
        }

        private void Update()
        {
            if(_onGround && ShouldLock)
                FreezeCharacter();

            _particleCounter += Time.deltaTime;
            _direction.x = _controller.input.RetrieveMoveInput();
            _desiredVelocity = new Vector2(_direction.x, 0f) * Mathf.Max(_maxSpeed - _collisionDataRetriever.Friction, 0f);
            if (!_onGround || Mathf.Abs(_velocity.x) < _particleSpeedThreshold)
            {
                _moveParticleSystem.Clear();
            }
        }

        private void FixedUpdate()
        {
            if(_dashInput != null) { _isDashing = _dashInput.IsDashing;}
            if (_isDashing)
                _moveParticleSystem.transform.localScale = new Vector3(1, 0.5f, 1); //If I ever Change the ScaleReduction Multiplier I need to adjust this.
            else
                _moveParticleSystem.transform.localScale = new Vector3(1, 1, 1);


            _onGround = _collisionDataRetriever.OnGround;
            _velocity = _body.velocity;
            _ceilingEdgeDetected = _raycastDataRetriever.CeilingEdgeDetected;

            #region Handle Movement

            if (!_isDashing)
            {
                _acceleration = _onGround ? _maxAcceleration : _maxAirAcceleration;
                _maxSpeedChange = _acceleration * Time.deltaTime;
                _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
            }
            #endregion

            #region Temp Particles
            // Really need to move this code out of here eventually
            if (_direction.x < 0f && _facingRight)
            {
                if(_moveParticleSystem != null)
                    _moveParticleSystem.transform.rotation *= Quaternion.Euler(0, 180, 0);
                _facingRight = false;

            }
            else if (_direction.x > 0 && !_facingRight)
            {
                if(_moveParticleSystem !=  null)
                    _moveParticleSystem.transform.rotation *= Quaternion.Euler(0, 180, 0);
                _facingRight = true;
            }


            if (Mathf.Abs(_velocity.x) > _particleSpeedThreshold && _onGround)
            {
                if (_particleCounter > _particleFormationPeriod)
                {
                     _moveParticleSystem.Play();
                     _particleCounter = 0;
                }
            }

            #endregion
                
            if (_velocity.x != 0) { _raycastDataRetriever.SetDirection(); } // Handles the direction in the Raycast code
            if (_ceilingEdgeDetected)
            {
                _body.transform.position += new Vector3(_ceilingEdgeCorrectionIntensity * (_facingRight ? 1 : -1), 0, 0); // Will break when I change the particles
            }

            _body.velocity = _velocity;  //This line should be the last thing on the update 99% of the time, otherwise it would be part of Handle Movement
        }

        public void FreezeCharacter()
        {
            ShouldLock = false;
            IsLocked = !IsLocked;
            
            if (IsLocked)
            {
                _body.constraints = RigidbodyConstraints2D.FreezePosition;
                _cameraManager.enabled = false;
                _dashInput.enabled = false;
            }
            else
            {
                _body.constraints = RigidbodyConstraints2D.FreezeRotation;
                _cameraManager.enabled = true;
                _dashInput.enabled = true;
            }
        }
    }
}
