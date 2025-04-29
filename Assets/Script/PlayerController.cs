using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float acceleration = 10f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public KeyCode runKey = KeyCode.LeftShift;

    private Vector2 moveInput;
    private float currentSpeed;
    private bool isRunning;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }
    void Update()
    {
        // Ambil input player
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Check for run input
        isRunning = Input.GetKey(runKey) && moveInput != Vector2.zero;

        // Smoothly transition between walk and run speed
        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // Update animator
        animator.SetBool("WalkRight", moveInput != Vector2.zero);

        // Handle flipping sprite based on direction
        if (moveInput.x < -0.1f) // Moving left
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (moveInput.x > 0.1f) // Moving right
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    private void FixedUpdate()
    {
        rb.velocity = moveInput * currentSpeed;
    }

}

