using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal; // Import yang benar untuk Light2D

public class PhotographySystem : MonoBehaviour
{
    [Header("Camera Settings")]
    public KeyCode photographKey = KeyCode.Space;
    public float photographCooldown = 1.0f;
    public float cameraRange = 5.0f;
    
    [Header("Visual Effects")]
    public GameObject cameraFlashPrefab;
    public float flashDuration = 0.2f;
    public AudioClip cameraSound;
    
    [Header("References")]
    public Transform cameraOrigin;
    public Light2D cameraLight;
    
    // Event yang dipanggil saat foto diambil
    public UnityEvent<string> OnPhotoTaken;
    
    private bool canTakePhoto = true;
    private float cooldownTimer = 0f;
    private QuestManager2 questManager;
    private AudioSource audioSource;
    
    private void Start()
    {
        questManager = QuestManager2.Instance;
        
        if (questManager == null)
        {
            Debug.LogError("QuestManager tidak ditemukan! Pastikan QuestManager sudah ada di scene.");
        }
        
        // Setup camera light
        if (cameraLight == null)
        {
            cameraLight = GetComponentInChildren<Light2D>();
        }
        
        // Matikan light pada awal permainan
        if (cameraLight != null)
        {
            cameraLight.enabled = true;
        }
        
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Initialize events
        if (OnPhotoTaken == null)
            OnPhotoTaken = new UnityEvent<string>();
    }
    
    private void Update()
    {
        // Update cooldown timer
        if (!canTakePhoto)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canTakePhoto = true;
            }
        }
        
        // Cek input untuk memotret
        if (Input.GetKeyDown(photographKey) && canTakePhoto)
        {
            // Cek apakah player memiliki quest fotografi yang aktif
            if (questManager != null && questManager.HasActivePhotoQuest())
            {
                TakePhoto();
            }
            else
            {
                Debug.Log("Tidak ada quest fotografi yang aktif!");
            }
        }
    }
    
    private void TakePhoto()
    {
        // Set cooldown
        canTakePhoto = false;
        cooldownTimer = photographCooldown;
        
        // Mainkan efek suara
        if (cameraSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(cameraSound);
        }
        
        // Buat efek flash dengan light
        if (cameraLight != null)
        {
            StartCoroutine(FlashLight());
        }
        
        // Tampilkan efek flash prefab jika tersedia
        if (cameraFlashPrefab != null)
        {
            GameObject flashEffect = Instantiate(cameraFlashPrefab, cameraOrigin.position, Quaternion.identity);
            Destroy(flashEffect, flashDuration);
        }
        
        // Cek objek di depan kamera
        DetectObjects();
    }
    
    private IEnumerator FlashLight()
    {
        // Aktifkan light
        cameraLight.enabled = true;
        
        // Tunggu durasi flash
        yield return new WaitForSeconds(flashDuration);
        
        // Matikan light
        cameraLight.enabled = false;
    }
    
    private void DetectObjects()
    {
        if (cameraOrigin == null)
        {
            Debug.LogError("Camera origin belum diatur pada PhotographySystem!");
            return;
        }
        
        // Raycast atau overlap circle untuk mendeteksi objek di sekitar
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(cameraOrigin.position, cameraRange);
        
        bool photoProcessed = false;
        
        foreach (Collider2D collider in hitColliders)
        {
            if (collider == null) continue;
            
            // Jika objek memiliki tag yang sesuai dengan quest
            string objectTag = collider.tag;
            Debug.Log($"Mendeteksi objek dengan tag: {objectTag}");
            
            // Informasikan QuestManager tentang objek yang terdeteksi
            if (questManager != null)
            {
                questManager.ProcessPhoto(objectTag);
                photoProcessed = true;
                OnPhotoTaken.Invoke(objectTag);
            }
        }
        
        if (!photoProcessed)
        {
            Debug.Log("Tidak ada objek quest yang terdeteksi dalam foto.");
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Visualisasi jarak kamera
        if (cameraOrigin != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cameraOrigin.position, cameraRange);
        }
    }
}