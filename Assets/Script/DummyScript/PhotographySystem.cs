using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class PhotographySystem : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode photographKey = KeyCode.Space;
    public float   cooldown       = 1f;
    public float   cameraRange    = 5f;

    [Header("Effects")]
    public GameObject cameraFlashPrefab;
    public float      flashDuration = 0.2f;
    public AudioClip  cameraSound;          // pastikan di-assign

    [Header("References")]
    public Transform cameraOrigin;
    public Light2D   cameraLight;

    public UnityEvent<string> OnPhotoTaken;

    private bool         canTake   = true;
    private float        timer     = 0f;
    private QuestManager2 qm;
    private AudioSource  audioSrc;

    void Start()
    {
        qm       = QuestManager2.Instance;
        audioSrc = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        // matikan lampu flash awalnya
        if (cameraLight != null)
            cameraLight.enabled = false;

        OnPhotoTaken = OnPhotoTaken ?? new UnityEvent<string>();
    }

    void Update()
    {
        if (!canTake)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f) canTake = true;
        }

        if (Input.GetKeyDown(photographKey) && canTake && qm != null && qm.HasActivePhotoQuest())
            TakePhoto();
    }

    private void TakePhoto()
    {
        canTake = false;
        timer   = cooldown;

        // play sound (safe-check)
        if (cameraSound != null)
            audioSrc.PlayOneShot(cameraSound);

        // flash light
        if (cameraLight != null)
            StartCoroutine(Flash());

        // optional flash prefab
        if (cameraFlashPrefab != null && cameraOrigin != null)
        {
            GameObject go = Instantiate(cameraFlashPrefab, cameraOrigin.position, Quaternion.identity);
            Destroy(go, flashDuration);
        }

        // detect objects
        Collider2D[] hits = Physics2D.OverlapCircleAll(cameraOrigin.position, cameraRange);
        bool any = false;
        foreach (var c in hits)
        {
            if (qm != null)
                qm.ProcessAction(c.tag);
            OnPhotoTaken.Invoke(c.tag);
            any = true;
        }
        if (!any)
            Debug.Log("Tidak ada target quest terdeteksi.");
    }

    private IEnumerator Flash()
    {
        cameraLight.enabled = true;
        yield return new WaitForSeconds(flashDuration);
        cameraLight.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraOrigin == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(cameraOrigin.position, cameraRange);
    }
}
