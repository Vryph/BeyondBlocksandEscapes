using UnityEngine;

namespace BBE
{
    public class Dash : MonoBehaviour
    {
        [Header("Dash Settings")]
        [SerializeField] private float _dashSpeed = 15f;
        [SerializeField] private float _dashTime = 0.2f;
        [SerializeField] private float _dashCooldownTime = 1f;

        

        [SerializeField] private float _scaleReductionMultiplier = 0.5f;

        [Header("Gambiarra")] private float _jumpBufferTime = 0.1f;
        private float _jumpBufferCounter;


        private Controller _controller;
        private Rigidbody2D _body;
        private CollisionDataRetrieval _collisionDataRetriever;


        [SerializeField]private bool _desiredJump, _desiredDash, _canDash, _onGround, _wasGrounded;
        private float _dashCounter, _dashCooldownCounter;

        public bool IsDashing { get; private set; }

        private Vector2 _dashDirection;
        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _body = GetComponent<Rigidbody2D>();
            _collisionDataRetriever = GetComponent<CollisionDataRetrieval>();
            _controller = GetComponent<Controller>();
        }

        private void Update()
        {
            SetDirection();

            _desiredJump = _controller.input.RetrieveJumpInput();
            _desiredDash = _controller.input.RetrieveDashInput();
        }

        private void FixedUpdate()
        {
            _onGround = _collisionDataRetriever.OnGround;

            if(_desiredDash && _canDash)
            {
                _dashCounter = _dashTime;
                _canDash = false;
                IsDashing = true;

                _jumpBufferCounter = _jumpBufferTime;
            }

            if(_desiredJump && IsDashing && _jumpBufferCounter <= 0)
            {
                StopDash();
            }

            if (_jumpBufferCounter > 0)
            {
                _jumpBufferCounter -= Time.fixedDeltaTime;
            }

            if (IsDashing)
            {
                DashAction();
            }
            else 
            {
                _dashCooldownCounter -= Time.fixedDeltaTime;
                if (_onGround && !_canDash)
                    _wasGrounded = true;
                if (_dashCooldownCounter < 0 && _wasGrounded)
                {
                    _canDash = true;
                }
            }

            _desiredJump = false;


        }

        private void DashAction()
        {
            if (_dashCounter > 0)
            {
                transform.localScale = new Vector3(_originalScale.x, _originalScale.y * _scaleReductionMultiplier, _originalScale.z);
                _body.velocity = _dashDirection * _dashSpeed;
                _body.velocity = new Vector2(_body.velocity.x,0);
                _dashCounter -= Time.fixedDeltaTime;
            }
            else
            {
                StopDash();
            }
        }

        private void StopDash()
        {
            transform.localScale = _originalScale;
            _wasGrounded = false;
            _dashCounter = 0;
            _dashCooldownCounter = _dashCooldownTime;
            IsDashing = false;
        }

        public void SetDirection()
        {
            if (_body.velocity.x != 0)
            {
                _dashDirection = _body.velocity.x < 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
            }
        }
    }
}