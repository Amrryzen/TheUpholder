using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhotoUI2 : MonoBehaviour
{
    [Header("UI References")]
    public GameObject photoPanel;
    public RawImage photoImage; // Foto besar
    public CanvasGroup photoCanvasGroup;

    [Header("Photo Settings")]
    public float fadeInDuration = 0.5f;
    public float displayDuration = 1.0f;
    public float fadeOutDuration = 0.5f;
    public KeyCode takePictureKey = KeyCode.Space;

    [Header("Audio")]
    public AudioClip photoDisplaySound;
    public AudioClip photoTakeSound;

    private AudioSource audioSource;
    private bool isPhotoMode = false;
    private QuestManager2 questManager;

    private void Start()
    {
        questManager = QuestManager2.Instance;

        if (photoImage != null && photoImage.GetComponent<Shadow>() == null)
        {
            Shadow shadow = photoImage.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, 0.5f);
            shadow.effectDistance = new Vector2(5, -5);
            shadow.useGraphicAlpha = true;
        }

        if (photoPanel != null)
            photoPanel.SetActive(false);

        if (photoCanvasGroup == null && photoPanel != null)
        {
            photoCanvasGroup = photoPanel.GetComponent<CanvasGroup>();
            if (photoCanvasGroup == null)
                photoCanvasGroup = photoPanel.AddComponent<CanvasGroup>();
        }

        if (photoCanvasGroup != null)
            photoCanvasGroup.alpha = 0;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(takePictureKey) && !isPhotoMode)
        {
            if (CanTakePhoto())
            {
                isPhotoMode = true;
                StartCoroutine(TakePhotoSequence());
            }
            else
            {
                Debug.Log("Belum ada quest fotografi aktif.");
            }
        }
    }

    public void ShowPhoto()
    {
        StartCoroutine(TakePhotoSequence());
    }

    private bool CanTakePhoto()
    {
        if (questManager == null) return false;

        foreach (Quest quest in questManager.activeQuests)
        {
            if (quest.requiresPhotography && !quest.isCompleted)
                return true;
        }

        return false;
    }

    private IEnumerator TakePhotoSequence()
    {
        if (audioSource != null && photoTakeSound != null)
            audioSource.PlayOneShot(photoTakeSound);

        yield return new WaitForEndOfFrame();

        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();

        photoPanel.SetActive(true);
        photoImage.texture = screenTexture;
        photoImage.rectTransform.localScale = Vector3.one;
        photoImage.rectTransform.anchoredPosition = Vector2.zero;

        if (photoCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(photoCanvasGroup, 0, 1, fadeInDuration));

            if (audioSource != null && photoDisplaySound != null)
                audioSource.PlayOneShot(photoDisplaySound);

            yield return new WaitForSeconds(displayDuration);

            yield return StartCoroutine(FadeCanvasGroup(photoCanvasGroup, 1, 0, fadeOutDuration));
        }

        photoPanel.SetActive(false);
        isPhotoMode = false;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvas, float from, float to, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            canvas.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        canvas.alpha = to;
    }
}
