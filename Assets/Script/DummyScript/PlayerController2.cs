using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;                  // Untuk battery (Image)
using TMPro;
using UnityEngine.Rendering.Universal; // Untuk Light2D di flashEffect
using UnityEngine.SceneManagement;     // Untuk LoadScene

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
    public GameObject interactionUI;
    public Vector3 uiWorldOffset = new Vector3(0, 1.5f, 0);

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Photo Capture")]
    public FlashEffects flashEffect;
    public PhotoUI2 photoUI;
    public AudioSource shutterSound;
    public KeyCode captureKey = KeyCode.Space;
    public float captureRange = 5f;
    public LayerMask captureLayer;

    [Header("Flashlight Settings")]
    public Flashlight2 flashlight;
    public Image batteryImage;
    public float maxBattery = 100f;
    public float batteryDrainRate = 5f;
    public float rechargeDelay = 3f;

    [Header("Enemy Tags")]
    public string[] enemyTags;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public string nextSceneName;

    // internal
    private Vector2 moveInput;
    private float currentSpeed;
    private bool isRunning;
    private Interacable currentTarget;
    private Transform currentTargetTransform;
    private bool isFacingRight = true;
    private float currentBattery;
    private bool isRecharging = false;

    private QuestManager2 questManager;
    private float photoTimer = 0f;
    private float photoCooldown = 1f;

    private bool isDead = false;
    private HashSet<string> capturedTags = new HashSet<string>();

    void Start()
    {
        rb = rb ?? GetComponent<Rigidbody2D>();
        animator = animator ?? GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        interactionUI?.SetActive(false);
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        currentBattery = maxBattery;
        UpdateBatteryUI();

        questManager = QuestManager2.Instance;
        if (questManager == null)
            Debug.LogWarning("QuestManager2 tidak ditemukan di scene!");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isDead || PauseController.Instance != null && PauseController.Instance.IsGamePaused())
            return;

        if (photoTimer > 0f)
            photoTimer -= Time.deltaTime;

        HandleMovementInput();
        HandleAnimationAndFlip();

        CheckForInteractable();
        if (currentTarget != null && Input.GetKeyDown(KeyCode.F))
            currentTarget.Interact();

        if (Input.GetKeyDown(captureKey) && photoTimer <= 0f)
        {
            if (questManager != null && questManager.HasActivePhotoQuest())
            {
                DoCapture();
                photoTimer = photoCooldown;
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
        if (isDead || PauseController.Instance != null && PauseController.Instance.IsGamePaused())
            return;

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

        if (moveInput.x < -0.1f && isFacingRight)
            FlipPlayer(false);
        else if (moveInput.x > 0.1f && !isFacingRight)
            FlipPlayer(true);
    }

    private void FlipPlayer(bool faceRight)
    {
        isFacingRight = faceRight;
        if (spriteRenderer != null)
            spriteRenderer.flipX = !isFacingRight;

        if (flashEffect != null)
        {
            Vector3 scale = flashEffect.transform.localScale;
            scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            flashEffect.transform.localScale = scale;
        }
    }

    private void CheckForInteractable()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactionRange, interactableLayer);
        if (hit != null)
        {
            var ia = hit.GetComponent<Interacable>();
            if (ia != null && ia.canInteract())
            {
                currentTarget = ia;
                currentTargetTransform = hit.transform;
                interactionUI.SetActive(true);
                PositionUIAboveTarget();
                return;
            }
        }

        currentTarget = null;
        interactionUI.SetActive(false);
    }

    private void PositionUIAboveTarget()
    {
        if (currentTargetTransform == null) return;
        Vector3 worldPos = currentTargetTransform.position + uiWorldOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        interactionUI.GetComponent<RectTransform>().position = screenPos;
    }

    private void DoCapture()
    {
        flashEffect?.PlayFlash();
        shutterSound?.Play();
        photoUI?.ShowPhoto();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, captureRange, captureLayer);
        bool any = false;

        foreach (var c in hits)
        {
            string tag = c.tag;
            if (!capturedTags.Contains(tag))
            {
                questManager.ProcessAction(tag);
                capturedTags.Add(tag);
                any = true;
            }
        }

        if (!any)
            Debug.Log("Tidak ada objek quest terdeteksi atau semua sudah dipotret.");
    }

    private void HandleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.E) && flashlight != null && currentBattery > 0f)
            flashlight.Toggle(!flashlight.IsOn());

        if (flashlight != null && flashlight.IsOn())
        {
            currentBattery = Mathf.Max(0f, currentBattery - batteryDrainRate * Time.deltaTime);
            UpdateBatteryUI();

            if (currentBattery <= 0f && !isRecharging)
            {
                flashlight.Toggle(false);
                StartCoroutine(RechargeBattery());
            }
        }
    }

    private IEnumerator RechargeBattery()
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        foreach (var tag in enemyTags)
        {
            if (other.CompareTag(tag))
            {
                isDead = true;
                StartCoroutine(GameOverRoutine());
                break;
            }
        }
    }

    private IEnumerator GameOverRoutine()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        yield return new WaitForSeconds(5f);

        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, captureRange);
    }
}
