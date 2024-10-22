using UnityEngine;

namespace BBE
{
    [RequireComponent(typeof(Controller))]
    public class Jump : MonoBehaviour
    {
        [SerializeField, Range(0f, 100f)] private float _jumpHeight = 3f;
        [SerializeField, Range(0, 5)] private int _maxAirJumps = 0;
        [SerializeField, Range(0f, 5f)] private float _downwardMovementMultiplier = 3f;
        [SerializeField, Range(0f, 5f)] private float _upwardMovementMultiplier = 1.7f;
        [SerializeField, Range(0, 0.4f)] private float _coyoteTime = 0.2f;
        [SerializeField, Range(0, 0.4f)] private float _jumpBufferTime = 0.2f;

        private Controller _controller;
        private Dash _dashInput;
        private Rigidbody2D _body;
        private CollisionDataRetrieval _collisionDataRetriever;
        private RaycastDataRetrieval _raycastDataRetriever;
        private Vector2 _velocity;

        private int _jumpPhase;
        private float _defaultGravityScale, _jumpSpeed, _coyoteCounter, _jumpBufferCounter;

        private bool _desiredJump, _onGround, _isJumping, _isJumpReset, _ceilingEdgeDetected;
        private bool _isDashing = false;


        private void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _dashInput = GetComponent<Dash>();
            _collisionDataRetriever = GetComponent<CollisionDataRetrieval>();
            _raycastDataRetriever = GetComponent<RaycastDataRetrieval>();
            _controller = GetComponent<Controller>();

            _isJumpReset = true;
            _defaultGravityScale = 1f;
        }

        private void Update()
        {
            _desiredJump = _controller.input.RetrieveJumpInput();
        }

        private void FixedUpdate()
        {
            _onGround = _collisionDataRetriever.OnGround;
            if (_dashInput != null)
            {
                _isDashing = _dashInput.IsDashing;
            }
            _velocity = _body.velocity;

            if ((_onGround || _isDashing) && _body.velocity.y == 0)
            {
                _jumpPhase = 0;
                _coyoteCounter = _coyoteTime;
                _isJumping = false;
            }
            else
            {
                _coyoteCounter -= Time.deltaTime;
            }

            if (_desiredJump && _isJumpReset)
            {
                _isJumpReset= false;
                _desiredJump = false;
                _jumpBufferCounter = _jumpBufferTime;
            }
            else if(_jumpBufferCounter > 0)
            {
                _jumpBufferCounter -= Time.deltaTime;
            }
            else if (!_desiredJump)
            {
                _isJumpReset = true;
            }

            if (_jumpBufferCounter > 0)
            {
                JumpAction();
            }

            if (_controller.input.RetrieveJumpInput() && _body.velocity.y > 0)
            {
                _body.gravityScale = _upwardMovementMultiplier;
            }
            else if(!_controller.input.RetrieveJumpInput() || _body.velocity.y < 0)
            {
                _body.gravityScale = _downwardMovementMultiplier;
            }
            else if (_body.velocity.y == 0)
            {
                _body.gravityScale = _defaultGravityScale;
            }


            #region Ceiling Edge Detection
            if ((!_onGround && _body.velocity.y > 0) || _ceilingEdgeDetected)
            { 
                _raycastDataRetriever.CeilingEdgeDetection();
                _ceilingEdgeDetected = _raycastDataRetriever.CeilingEdgeDetected;
            }


            #endregion


            _body.velocity = _velocity;
        }



        private void JumpAction()
        {
            if (_coyoteCounter > 0f || (_jumpPhase < _maxAirJumps && _isJumping))
            {
                if (_isJumping)
                {
                    _jumpPhase += 1;
                }

                _coyoteCounter = 0;
                _jumpBufferCounter = 0;
                _jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * _jumpHeight);
                _isJumping = true;

                if (_velocity.y > 0f)
                {
                    _jumpSpeed = Mathf.Max(_jumpSpeed - _velocity.y, 0f);
                }
                else if (_velocity.y < 0f)
                {
                    _jumpSpeed += Mathf.Abs(_body.velocity.y);
                }
                _velocity.y += _jumpSpeed;
            }
        }
    }
}