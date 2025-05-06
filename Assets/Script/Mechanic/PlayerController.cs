using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float acceleration = 10f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Interaction Settings")]
    public float interactionRange = 2f;
    public LayerMask interactableLayer;
    public GameObject interactionUI;        // UI "Tekan F" di Canvas
    public Vector3 uiWorldOffset = new Vector3(0, 1.5f, 0);


    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    private SpriteRenderer spriteRenderer; // Added SpriteRenderer reference

    // Internal
    private Vector2 moveInput;
    private float currentSpeed;
    private bool isRunning;
    private Interacable currentTarget;
    private Transform currentTargetTransform;
    private bool isFacingRight = true; // Track facing direction

    [Header("Photo Capture")]
    public FlashEffect flashEffect;
    public PhotoUI photoUI;
    public AudioSource shutterSound;
    public KeyCode captureKey = KeyCode.Space;


    void Start()
    {
        // Auto-assign jika belum di-drag di Inspector
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (interactionUI != null) interactionUI.SetActive(false);

        // Set interpolation for smoother movement
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }

    void Update()
    {
        // Jangan lakukan apa-apa kalau game sedang di-pause
        if (PauseController.IsGamePaused) return;

        HandleMovementInput();
        HandleAnimationAndFlip();

        // Cek NPC/objek interactable di sekitar
        CheckForInteractable();

        // Kalau ada target, posisikan UI & cek input F
        if (currentTarget != null)
        {
            PositionUIAboveTarget();
            if (Input.GetKeyDown(KeyCode.F))
                currentTarget.Interact();
        }

        if (Input.GetKeyDown(captureKey))
        {
            CapturePhoto();
        }
    }

    void FixedUpdate()
    {
        // Gerakkan Rigidbody
        rb.velocity = moveInput * currentSpeed;
    }

    private void HandleMovementInput()
    {
        // Ambil input WASD / Arrow
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Run jika shift + bergerak
        isRunning = Input.GetKey(runKey) && moveInput != Vector2.zero;
        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
    }

    private void HandleAnimationAndFlip()
    {
        // Set parameter animasi (sesuaikan nama di Animator)
        animator.SetBool("WalkRight", moveInput != Vector2.zero);

        // OPTION 1: Using SpriteRenderer.flipX (if you have a SpriteRenderer)
        if (spriteRenderer != null)
        {
            // Only flip when there's significant horizontal movement
            if (moveInput.x < -0.1f && isFacingRight)
            {
                isFacingRight = false;
                spriteRenderer.flipX = true;
            }
            else if (moveInput.x > 0.1f && !isFacingRight)
            {
                isFacingRight = true;
                spriteRenderer.flipX = false;
            }
        }
        // OPTION 2: Using rotation (if your character is more complex)
        else
        {
            // Only change rotation when direction actually changes
            if (moveInput.x < -0.1f && isFacingRight)
            {
                isFacingRight = false;
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (moveInput.x > 0.1f && !isFacingRight)
            {
                isFacingRight = true;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void CheckForInteractable()
    {
        // Cari collider di radius
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);
        if (hit != null)
        {
            var interactable = hit.GetComponent<Interacable>();
            if (interactable != null && interactable.canInteract())
            {
                currentTarget = interactable;
                currentTargetTransform = hit.transform;
                interactionUI.SetActive(true);
                return;
            }
        }

        // Tidak ada target valid
        currentTarget = null;
        currentTargetTransform = null;
        interactionUI.SetActive(false);
    }

    private void PositionUIAboveTarget()
    {
        // Dapatkan world pos + offset
        Vector3 worldPos = currentTargetTransform.position + uiWorldOffset;
        // Konversi ke layar (screen) pos
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        // Set posisi RectTransform UI
        RectTransform rt = interactionUI.GetComponent<RectTransform>();
        rt.position = screenPos;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualisasi radius interaksi di Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    private void CapturePhoto()
    {
        if (flashEffect != null) flashEffect.PlayFlash();
        if (shutterSound != null) shutterSound.Play();
        if (photoUI != null) photoUI.ShowPhoto();
    }
}