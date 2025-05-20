using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Untuk Light2D

public class FlashEffects : MonoBehaviour
{
    public Light2D flashLight;
    public float flashDuration = 0.2f;
    public float maxIntensity = 1.0f;

    public FlashEffects flashEffects;

    public Transform playerTransform; // Drag objek Player ke sini via Inspector
    public Vector3 offset = Vector3.zero; // Jika ingin posisi flash tidak persis di tengah

    private void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + offset;
        }
    }



    private void Start()
    {
        if (flashLight == null)
        {
            flashLight = GetComponent<Light2D>();
        }

        if (flashLight != null)
        {
            flashLight.enabled = false;
        }
    }

    public void PlayFlash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        if (flashLight == null) yield break;

        flashLight.enabled = true;
        flashLight.intensity = maxIntensity;

        // Durasi tetap terang
        yield return new WaitForSeconds(flashDuration * 0.2f);

        // Fade out
        float timer = 0;
        float fadeDuration = flashDuration * 0.8f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float normalizedTime = timer / fadeDuration;
            flashLight.intensity = Mathf.Lerp(maxIntensity, 0, normalizedTime);
            yield return null;
        }

        flashLight.enabled = false;
    }

}