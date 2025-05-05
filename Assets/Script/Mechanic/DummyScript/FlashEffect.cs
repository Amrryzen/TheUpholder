using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
    public Light2D flashLight;
    public float flashDuration = 0.2f;
    public float maxIntensity = 1.5f;

    private float originalIntensity;

    void Start()
    {
        if (flashLight != null)
        {
            originalIntensity = flashLight.intensity;
            flashLight.intensity = 0f;
        }
    }

    public void PlayFlash()
    {
        if (flashLight != null)
            StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        flashLight.intensity = maxIntensity;
        yield return new WaitForSeconds(flashDuration);
        flashLight.intensity = 0f;
    }
}
