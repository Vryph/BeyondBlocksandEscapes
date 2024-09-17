using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Rigidbody2D rb;
    Camera mainCamera;

    //Collision
    [Header("Ground Collision Check parameters")]
    public Vector2 floorCheckSize = new Vector2(1f, 1f);
    public float floorCheckDistance = 0.4f;

    //Movement
    [Header("Ground Movement Parameters")]
    public float maxMoveSpeed = 5;
    public float debugVelocity = 0f;
    [SerializeField] private float horizontalMoveInput = 0f;

    //Jump
    [Header("Jump Parameters")]
    public float jumpVelocity = 10f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 1f;
    [SerializeField] private float coyoteTimeCounter = 0f;
    [SerializeField] private float jumpBufferCounter = 0f;
    [SerializeField] private float terminalVelocity = 25f;

    [SerializeField] private bool isGrounded = false;


    //[SerializeField] private bool hasDoubleJump = false;
    //public float doubleJumpVelocity = 8f;
    //public float doubleJumpHorizontalBoost = 4f;
    //public float doubleJumpGravity = 3f;
    public float risingGravity = 2.4f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        CameraFollow();
        debugVelocity = rb.velocity.x;
        //Gets the player's inputs
        horizontalMoveInput = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(horizontalMoveInput * maxMoveSpeed, rb.velocity.y);
    }

    private void CheckGround()
    {
        int mask = ~LayerMask.GetMask("Player");
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, floorCheckSize, 0, Vector2.down, floorCheckDistance, mask);

        if (rb.velocity.y <= 0f && hit.collider != null)
        {
            coyoteTimeCounter = coyoteTime;
            //hasDoubleJump = true;
            rb.gravityScale = risingGravity;
        }


        isGrounded = false; //Sets Grounded to false every fixedUpdate
        if (coyoteTimeCounter > 0f)
            isGrounded = true;
        else if (hit.collider != null)
            isGrounded = true;
    }

    private int ShouldJump()
    {
        if (jumpBufferCounter > 0f)
        {
            if (isGrounded)
            {
                return 1;
            }
            //else if (hasDoubleJump) return 2;
        }
        return 0;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }

    /*
    private void DoubleJump()
    {
        hasDoubleJump = false;
        //if( horizontalMoveInput > 0)  rb.AddForce(Vector2.right * doubleJumpHorizontalBoost, ForceMode2D.Impulse);  Trying to make a boost, too bug to present
        //else if (horizontalMoveInput < 0) rb.AddForce(Vector2.left * doubleJumpHorizontalBoost, ForceMode2D.Impulse);
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpVelocity);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
        rb.gravityScale = doubleJumpGravity;
    }
    */


    private void UpdateTimers()
    {
        jumpBufferCounter -= Time.fixedDeltaTime;
        coyoteTimeCounter -= Time.fixedDeltaTime;
    }

    private void LimitTerminalVelocity()
    {
        if (rb.velocity.y <= -terminalVelocity)
            rb.velocity = new Vector2(rb.velocity.x, -terminalVelocity);
    }


    private void FixedUpdate()
    {
        Move();
        CheckGround();
        switch (ShouldJump())
        {
            case 0: break;
            case 1:
                Jump(); break;
            case 2:
               //DoubleJump();
               break;
        }
        LimitTerminalVelocity();
        UpdateTimers();
    }


    private void CameraFollow()
    {
        mainCamera.transform.position = new Vector3(rb.transform.position.x, rb.transform.position.y, mainCamera.transform.position.z);
        mainCamera.transform.position = new Vector3(Mathf.Clamp(mainCamera.transform.position.x, -4.4f, -4.4f), Mathf.Clamp(mainCamera.transform.position.y, 1.3f, 200f), mainCamera.transform.position.z);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position + Vector3.down * floorCheckDistance, floorCheckSize);
    }
}
