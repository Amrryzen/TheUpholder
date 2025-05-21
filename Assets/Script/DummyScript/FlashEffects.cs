using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Untuk Light2D

[RequireComponent(typeof(Light2D))]
public class FlashEffects : MonoBehaviour
{
    [Header("Flash Settings")]
    public Light2D flashLight;
    public float flashDuration = 0.2f;
    public float maxIntensity = 1.0f;

    [Header("Position Settings")]
    public Transform playerTransform;    // Drag objek Player ke sini via Inspector
    public Vector3 offset = Vector3.zero;

    private void Awake()
    {
        // Jika belum assign di Inspector, ambil dari komponen GameObject ini
        if (flashLight == null)
            flashLight = GetComponent<Light2D>();
    }

    private void Start()
    {
        // Pastikan light selalu aktif, tapi tidak menyala (= intensity 0)
        if (flashLight != null)
        {
            flashLight.enabled = true;
            flashLight.intensity = 0f;
        }
    }

    private void Update()
    {
        // Ikuti posisi player
        if (playerTransform != null)
            transform.position = playerTransform.position + offset;
    }

    /// <summary>
    /// Panggil ini saat player memotret
    /// </summary>
    public void PlayFlash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        if (flashLight == null)
            yield break;

        // Tampilkan sekejap flash
        flashLight.intensity = maxIntensity;

        // Durasi tetap terang
        yield return new WaitForSeconds(flashDuration * 0.2f);

        // Fade out
        float timer = 0f;
        float fadeDuration = flashDuration * 0.8f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            flashLight.intensity = Mathf.Lerp(maxIntensity, 0f, t);
            yield return null;
        }

        // Pastikan benar-benar mati dengan intensity = 0
        flashLight.intensity = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
