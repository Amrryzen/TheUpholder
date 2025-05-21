using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoUI2 : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject  photoPanel;
    public RawImage    photoImage;
    public CanvasGroup photoCanvasGroup;

    [Header("Timing")]
    public float fadeInDuration  = 0.5f;
    public float displayDuration = 1f;
    public float fadeOutDuration = 0.5f;

    [Header("Audio")]
    public AudioClip takeSound;
    public AudioClip displaySound;

    private AudioSource audioSrc;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        photoPanel?.SetActive(false);
        if (photoCanvasGroup != null) photoCanvasGroup.alpha = 0f;
    }

    public void ShowPhoto()
    {
        StartCoroutine(ShowSequence());
    }

    private IEnumerator ShowSequence()
    {
        audioSrc.PlayOneShot(takeSound);
        yield return new WaitForEndOfFrame();

        // capture screen
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0,0,Screen.width,Screen.height), 0,0);
        tex.Apply();

        photoImage.texture = tex;
        photoPanel.SetActive(true);

        // fade in
        yield return Fade(0f, 1f, fadeInDuration);
        audioSrc.PlayOneShot(displaySound);
        // display
        yield return new WaitForSeconds(displayDuration);
        // fade out
        yield return Fade(1f, 0f, fadeOutDuration);

        photoPanel.SetActive(false);
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (photoCanvasGroup == null) yield break;
        float start = Time.time;
        while (Time.time < start + duration)
        {
            photoCanvasGroup.alpha = Mathf.Lerp(from, to, (Time.time - start)/duration);
            yield return null;
        }
        photoCanvasGroup.alpha = to;
    }
}
