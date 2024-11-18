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

        [Header("Animation")]
        [SerializeField] private SquashAndStretch _dashSquashAndStretch;

        private Controller _controller;
        private Rigidbody2D _body;
        private CollisionDataRetrieval _collisionDataRetriever;


        private bool _desiredDash, _canDash, _onGround, _isDashReset;
        private float _dashCounter, _dashCooldownCounter, _hInput;

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
            _desiredDash = _controller.input.RetrieveDashInput();
            
        }

        private void FixedUpdate()
        {
            _onGround = _collisionDataRetriever.OnGround;

            if(_desiredDash && _canDash && _isDashReset)
            {
                _dashCounter = _dashTime;
                _canDash = false;
                _isDashReset = false;
                IsDashing = true;

                if (_dashSquashAndStretch != null)
                    _dashSquashAndStretch.PlaySquashAndStretch(); // Has to play the animation here, I know it isn't good - I reget how I made this dash work
                SoundManager.PlaySound(SoundType.Dash, 0.65f);
            }
            else if (!_desiredDash)
            {
                _isDashReset = true;
            }

            if (IsDashing)
            {
                DashAction();
            }
            else 
            {
                _dashCooldownCounter -= Time.fixedDeltaTime;
            }

            if (_onGround)
                ResetDash();

        }

        private void DashAction()
        {
            if (_dashCounter > 0)
            {
                if (_originalScale == transform.localScale)
                {
                    transform.localScale = new Vector3(_originalScale.x, _originalScale.y * _scaleReductionMultiplier, _originalScale.z);

                    if (_onGround)
                    {
                        float scaleDifferential = _originalScale.y - transform.localScale.y;
                        transform.position = new Vector3(transform.position.x, transform.position.y - scaleDifferential / 2, transform.position.z);
                    }
                }

                _body.velocity = new Vector2(_dashDirection.x * _dashSpeed, 0);
                _dashCounter -= Time.fixedDeltaTime;
            }
            else
            {
                StopDash();
            }
        }

        public void StopDash()
        {
            if (IsDashing)
            {
                transform.localScale = _originalScale;
                _dashCounter = 0;
                _dashCooldownCounter = _dashCooldownTime;
                IsDashing = false;
            }
        }

        public void SetDirection()
        {
            if (!IsDashing)
            {
                _hInput = _controller.input.RetrieveMoveInput();
                if (_hInput != 0)
                {
                    _dashDirection = _hInput < 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
                }
            }
        }

        public void ResetDash()
        {
            if (!IsDashing && !_canDash)
            {
                if(_dashCooldownCounter < 0)
                {
                    _canDash = true;
                }
            }
        }
    }
}