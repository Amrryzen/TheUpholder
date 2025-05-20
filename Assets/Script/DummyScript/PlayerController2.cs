using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;         // Untuk Slider
using TMPro;
using UnityEngine.Rendering.Universal; // Untuk Light2D

public class PlayerController2 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float acceleration = 10f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    public LayerMask interactableLayer;
    public GameObject interactionUI;        // UI "Tekan F" di Canvas
    public Vector3 uiWorldOffset = new Vector3(0, 1.5f, 0);

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Photo Capture")]
    public FlashEffects flashEffect;
    public string photoQuestID = "photoQuest1";
    public PhotoUI2 photoUI;
    public AudioSource shutterSound;
    public KeyCode captureKey = KeyCode.Space;
    public float captureRange = 5f;        // Jarak kemampuan memotret
    public LayerMask captureLayer;         // Layer objek yang bisa difoto

    [Header("Flashlight Settings")]
    public Flashlight2 flashlight;      // Drag GameObject berisi komponen Flashlight
    public Image batteryImage;        // Drag UI Slider
    public float maxBattery = 100f;
    public float batteryDrainRate = 5f;
    public float rechargeDelay = 3f;

    // Internal
    private Vector2 moveInput;
    private float currentSpeed;
    private bool isRunning;
    private Interacable currentTarget;
    private Transform currentTargetTransform;
    private bool isFacingRight = true;
    private float currentBattery;
    private bool isRecharging = false;
    private QuestManager2 questManager;
    private float photoTimer = 0f;       // Cooldown timer untuk memotret
    private float photoCooldown = 1f;    // Delay antara foto

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (interactionUI != null) interactionUI.SetActive(false);
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Inisialisasi baterai
        currentBattery = maxBattery;
        UpdateBatteryUI();
        
        // Dapatkan referensi quest manager
        questManager = QuestManager2.Instance;
        if (questManager == null)
        {
            Debug.LogWarning("QuestManager tidak ditemukan! Pastikan objek QuestManager sudah ada di scene.");
        }
    }

    void Update()
    {
        if (PauseController.IsGamePaused) return;

        // Update timer foto
        if (photoTimer > 0)
        {
            photoTimer -= Time.deltaTime;
        }

        HandleMovementInput();
        HandleAnimationAndFlip();

        CheckForInteractable();
        if (currentTarget != null)
        {
            PositionUIAboveTarget();
            if (Input.GetKeyDown(KeyCode.F))
                currentTarget.Interact();
        }

        // Hanya bisa memotret jika quest foto aktif dan cooldown selesai
        if (Input.GetKeyDown(captureKey) && photoTimer <= 0)
        {
            if (questManager != null && questManager.IsQuestActive(photoQuestID))
            {
                CapturePhoto();
                photoTimer = photoCooldown; // Set cooldown
            }
            else
            {
                Debug.Log("Tidak ada quest fotografi yang aktif!");
            }
        }

        HandleFlashlight();
    }

    void FixedUpdate()
    {
        if (PauseController.IsGamePaused) return;
        rb.velocity = moveInput * currentSpeed;
    }

    private void HandleMovementInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        isRunning = Input.GetKey(runKey) && moveInput != Vector2.zero;
        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
    }

    private void HandleAnimationAndFlip()
    {
        animator.SetBool("WalkRight", moveInput != Vector2.zero);

        if (spriteRenderer != null)
        {
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
        else
        {
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

        currentTarget = null;
        currentTargetTransform = null;
        interactionUI.SetActive(false);
    }

    private void PositionUIAboveTarget()
    {
        Vector3 worldPos = currentTargetTransform.position + uiWorldOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        RectTransform rt = interactionUI.GetComponent<RectTransform>();
        rt.position = screenPos;
    }

    private void CapturePhoto()
    {
        // Hanya izinkan jika quest photo aktif
        if (questManager == null || !questManager.IsQuestActive(photoQuestID))
            return;

        // Efek flash, suara shutter, dan UI
        flashEffect?.PlayFlash();
        shutterSound?.Play();
        photoUI?.ShowPhoto();

        // Deteksi objek di sekitar dengan layer yang bisa difoto
        DetectPhotoObjects();

        // Publikasikan event untuk quest objective
        questManager.PublishEvent("OnPhotoCaptured", photoQuestID);
    }

    private void DetectPhotoObjects()
    {
        // Deteksi objek di sekitar player
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, captureRange, captureLayer);
        
        bool photoProcessed = false;
        
        foreach (Collider2D collider in hitColliders)
        {
            // Jika objek memiliki tag yang sesuai dengan quest
            string objectTag = collider.tag;
            Debug.Log($"Mendeteksi objek dengan tag: {objectTag}");
            
            // Proses foto pada quest manager
            if (questManager != null)
            {
                questManager.ProcessPhoto(objectTag);
                photoProcessed = true;
            }
        }
        
        if (!photoProcessed)
        {
            Debug.Log("Tidak ada objek quest yang terdeteksi dalam foto.");
        }
    }

    private void HandleFlashlight()
    {
        // Toggle flashlight dengan E (tidak memicu recharge)
        if (Input.GetKeyDown(KeyCode.E) && flashlight != null)
        {
            bool turnOn = !flashlight.IsOn() && currentBattery > 0f;
            flashlight.Toggle(turnOn);
        }

        // Drain baterai saat nyala
        if (flashlight != null && flashlight.IsOn())
        {
            currentBattery -= batteryDrainRate * Time.deltaTime;
            currentBattery = Mathf.Max(0f, currentBattery);
            UpdateBatteryUI();

            // Saat habis, matikan & mulai coroutine recharge
            if (currentBattery <= 0f && !isRecharging)
            {
                flashlight.Toggle(false);
                StartCoroutine(RechargeBatteryAfterDelay());
            }
        }
    }

    private IEnumerator RechargeBatteryAfterDelay()
    {
        isRecharging = true;
        yield return new WaitForSeconds(rechargeDelay);

        currentBattery = maxBattery;
        UpdateBatteryUI();
        isRecharging = false;
    }

    private void UpdateBatteryUI()
    {
        if (batteryImage != null)
            batteryImage.fillAmount = currentBattery / maxBattery;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualisasi jarak interaksi
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        // Visualisasi jarak foto
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, captureRange);
    }
}