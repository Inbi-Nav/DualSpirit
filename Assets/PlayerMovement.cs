using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
public Rigidbody2D rb;
bool isFacingRight = true;
    
[Header("Movement")]
public float moveSpeed = 5f;
float horizontalMovement;

[Header("Jumping")]
public float jumpPower = 15f;
public int maxJumps = 2;
private int jumpsRemaining;

[Header("GroundCheck")]
public Transform groundCheckPos;
public Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
public LayerMask groundLayer;
bool isGrounded;

[Header("Gravity")]
public float baseGravity = 2f;
public float maxFallSpeed = 18f;
public float fallGravityMult = 2f;

[Header("WallCheck")]
public Transform wallCheckPos;
public Vector2 wallCheckSize = new Vector2(0.49f, 0.03f);
public LayerMask wallLayer;

[Header("WallMovement")]
public float wallSlideSpeed = 2;
bool isWallSliding;

bool isWallJumping;
float wallJumpDirection;
float wallJumpTime = 0.5f;
float wallJumpTimer;
public Vector2 wallJumpPower = new Vector2(5f, 10f);

    

   void Update()
    {
    GroundCheck();
    ProcessGravity();
    ProcessWallSlide();
    ProcessWallJump();

    if (!isWallJumping)
        {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        Flip();
        }
    }

    private void ProcessGravity()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallGravityMult;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if (!isGrounded && WallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)); // Caps fall rate
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }
    
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpsRemaining--;
            }
            else if (context.canceled && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                jumpsRemaining--;
            }
        }

        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); // Jump away from wall
            wallJumpTimer = 0;


            {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); // Wall Jump = 0.5s -- Jump again = 0.6s
        }
    }


    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer)) //checks if set box overlaps with ground
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }


    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void Flip() 
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
