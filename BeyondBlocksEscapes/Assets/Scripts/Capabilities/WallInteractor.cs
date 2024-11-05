using System.Diagnostics;
using UnityEngine;

namespace BBE {

    [RequireComponent(typeof(Controller))]
    public class WallInteractor : MonoBehaviour
    {
        [Header("Wall Slide")]
        [SerializeField] private bool _activateWallSlide = true;
        [SerializeField, Range(0.1f, 10f)] private float _wallSlideMaxSpeed = 2f;
        [SerializeField, Range(0.05f, 0.5f)] private float _wallStickTime = 0.25f;
        [Header("Wall Jump")]
        [SerializeField] private bool _activateWallJump = true;
        [SerializeField] private Vector2 _wallJumpClimb = new Vector2(4f, 12f);
        [SerializeField] private Vector2 _wallJumpBounce = new Vector2(10.7f, 10f);
        [SerializeField] private Vector2 _wallJumpLeap = new Vector2(14f, 12f);
        [SerializeField, Range(0f, 0.4f)] private float _wallJumpBufferTime = 0.15f;

        [Header("Animation")]
        [SerializeField] private SquashAndStretch _wallJumpSquashAndStretch;
        [SerializeField] private ParticleSystem _wallSlideParticle;

        private CollisionDataRetrieval _collisionDataRetriever;
        private Rigidbody2D _body;
        private Controller _controller;
        private Dash _dash;

        private Vector2 _velocity;
        private bool _onWall, _onGround, _desiredJump, _isJumpReset;
        private float _wallStickCounter, _wallJumpBufferCounter;
        [HideInInspector] public bool WallJumping { get; private set; }
        private float _wallDirectionX;

        private void Start()
        {
            _collisionDataRetriever = GetComponent<CollisionDataRetrieval>();
            _body = GetComponent<Rigidbody2D>();
            _controller = GetComponent<Controller>();
            _dash = GetComponent<Dash>();

            _isJumpReset = true;
        }
        private void Update()
        {
            if (_activateWallJump)
            {
                _desiredJump = _controller.input.RetrieveJumpInput();
            }
        }

        private void FixedUpdate()
        {
            _velocity = _body.velocity;
            _onWall = _collisionDataRetriever.OnWall;
            _onGround = _collisionDataRetriever.OnGround;
            _wallDirectionX = _collisionDataRetriever.ContactNormal.x;

            if (_onWall)
            {
                if (_dash.IsDashing)
                {
                    _dash.StopDash();
                }
            }

            #region Wall Slide
            if (_onWall && _activateWallSlide)
            {
                _dash.ResetDash();
                if(_velocity.y < -_wallSlideMaxSpeed)
                {
                    _velocity.y = -_wallSlideMaxSpeed;
                }
                _wallSlideParticle.Play();
            }
            #endregion


            #region Wall Stick
            if (_activateWallSlide)
            {
                if (_onWall && !_onGround && !WallJumping)
                {
                    if (_wallStickCounter > 0)
                    {
                        _velocity.x = 0;

                        if (_controller.input.RetrieveMoveInput() != 0 &&
                            Mathf.Sign(_controller.input.RetrieveMoveInput()) == Mathf.Sign(_collisionDataRetriever.ContactNormal.x))
                        {
                            _wallStickCounter -= Time.deltaTime;
                        }
                        else
                        {
                            _wallStickCounter = _wallStickTime;
                        }
                    }
                    else
                    {
                        _wallStickCounter = _wallStickTime;
                    }
                }
            }
            #endregion

            #region Wall Jump
            if ((_onWall && _velocity.x == 0) || _onGround)
            {
                WallJumping = false;
            }


            if (_desiredJump && _isJumpReset)
            {
                _wallJumpBufferCounter = _wallJumpBufferTime;
                _desiredJump = false;
                _isJumpReset = false;
            }
            else if(_wallJumpBufferCounter > 0)
            {
                _wallJumpBufferCounter -= Time.deltaTime;
            }
            else if (!_desiredJump)
            {
                _isJumpReset = true;
            }

            if (_onWall && !_onGround)
            {
                if(_wallJumpBufferCounter > 0)
                {
                    if (_dash.IsDashing)
                    {
                        _dash.StopDash();
                    }

                    WallJumping = true;
                    _wallJumpBufferCounter = 0;

                    if (_wallJumpSquashAndStretch != null)
                        _wallJumpSquashAndStretch.PlaySquashAndStretch();

                    if (_controller.input.RetrieveMoveInput() == 0)
                    {
                        _velocity = new Vector2(_wallJumpBounce.x * _wallDirectionX, _wallJumpBounce.y);
                    }
                    else if (Mathf.Sign(-_wallDirectionX) == Mathf.Sign(_controller.input.RetrieveMoveInput()))
                    {
                        _velocity = new Vector2(_wallJumpClimb.x * _wallDirectionX, _wallJumpClimb.y);
                    }
                    else
                    {
                        _velocity = new Vector2(_wallJumpLeap.x * _wallDirectionX, _wallJumpLeap.y);
                    }  
                }
            }

            #endregion

            _body.velocity = _velocity;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _collisionDataRetriever.EvaluateCollision(collision);
            _isJumpReset = false;
            
            if(_collisionDataRetriever.OnWall && !_collisionDataRetriever.OnGround && WallJumping)
            {
                _body.velocity = Vector2.zero;
                WallJumping = false;
            }
        }
    }
}