using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    public float speed;
    public float jumpForce;

    public float fallMultiplier = 2.5f;
    public float highJumpMultiplier = 0.5f;

    bool isGrounded = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;

    public float rememberGroundedFor;
    float lastTimeGrounded;

    public int defaultAdditionalJumps = 1;
    int additionalJumps;

    public ParticleSystem jumpTrail;
    public ParticleSystem groundedTrail;

    public float jumpBufferTime;
    private float lastTimeJumpPressed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        additionalJumps = defaultAdditionalJumps;
        lastTimeJumpPressed = -1;
    }

    void Update()
    {
        Move();
        Jump();
        BetterJump();
        CheckIfGrounded();

        if (isGrounded)
        {
            groundedTrail.startColor = Color.red;
        }
        else
        {
            groundedTrail.startColor = Color.white;
        }
    }


    void Move() {
        float x = Input.GetAxisRaw("Horizontal");

        float moveBy = x * speed;

        rb.velocity = new Vector2(moveBy, rb.velocity.y);
    }

    void Jump() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastTimeJumpPressed = Time.time;
        }
        bool jumpExpected = Time.time - lastTimeJumpPressed <= jumpBufferTime;
        if (jumpExpected && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || additionalJumps > 0)) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            additionalJumps--;
        }
    }

    void BetterJump()
    {

        bool goingUp = rb.velocity.y > 0;
        bool highJumping = Input.GetKey(KeyCode.Space);
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
            jumpTrail.startColor = Color.red;
        } else if (goingUp)
        {
            if (highJumping)
            {
                rb.gravityScale = highJumpMultiplier;
                jumpTrail.startColor = Color.yellow;
            }
            else
            {
                rb.gravityScale = 2f;
                jumpTrail.startColor = Color.green;
            }
        }
        else
        {
            rb.gravityScale = 1.0f;
            jumpTrail.startColor = Color.blue;
        }
    }

    void CheckIfGrounded() {
        Collider2D colliders = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if (colliders != null) {
            isGrounded = true;
            additionalJumps = defaultAdditionalJumps;
        } else {
            if (isGrounded) {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }
    
}
