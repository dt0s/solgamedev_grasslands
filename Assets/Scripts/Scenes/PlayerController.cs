using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D body;
    
    private float moveInput;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    
    public float speed;
    public float jumpForce;
    public Transform feetPos;
    public LayerMask whatIsGround;
    public float checkRadius;
    private bool movementCooldown;
    private float jumpTimeCounter;
    private bool isCharging;
    public float jumpTime;
    public float maxJump;
    
    
    private float jumpDirection;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        jumpTimeCounter = jumpTime;
        movementCooldown = false;
    }
    
    private void FixedUpdate()
    {

    }
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        moveInput = Input.GetAxisRaw("Horizontal");
        if ((moveInput > 0 || moveInput < 0) && isCharging && movementCooldown == false)
        {
            Debug.Log("moving");
            body.velocity = new Vector2(moveInput * speed, body.velocity.y);    
        }
        else
        {
            Debug.Log(isGrounded);
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("start jump");
                isCharging = true;
                jumpTimeCounter = jumpTime;
            }
            if (Input.GetKey(KeyCode.Space) && isCharging && isGrounded)
            {
                if (jumpTimeCounter > 0)
                {
                    jumpTimeCounter -= Time.deltaTime;
                }
                else
                {
                    isCharging = false;
                }
            }
            if (isCharging == false && jumpTimeCounter < jumpTime && isGrounded)
            {
                movementCooldown = true;
                jumpDirection = Input.GetAxisRaw("Horizontal");
                if (jumpDirection == 0)
                {
                    body.velocity = new Vector2(0, (jumpTime - jumpTimeCounter) * jumpForce);
                    isCharging = true;
                } else if (jumpDirection > 0)
                {
                    body.velocity = new Vector2(jumpDirection * speed, (jumpTime - jumpTimeCounter) * jumpForce);
                    isCharging = true;
                } else if (jumpDirection < 0)
                {
                    body.velocity = new Vector2(jumpDirection * speed, (jumpTime - jumpTimeCounter) * jumpForce);
                    isCharging = true;
                }
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isCharging = false;
            }
        }
    }
}
