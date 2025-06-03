using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PhonePauseEffect : MonoBehaviour
{
    public Button pauseButton;
    public Button resumeButton;

    public Image blackOverlay;
    public Image flashImage;
    public GameObject pausePanel;

    public float fadeDuration = 0.3f;
    public float buttonScaleDuration = 0.2f;
    public float buttonDelay = 0.1f;

    private bool isPaused = false;

    void Start()
    {
        pauseButton.onClick.AddListener(OnPause);
         resumeButton.onClick.AddListener(ResumeGame); 

        SetAlpha(blackOverlay, 0f);
        SetAlpha(flashImage, 0f);
        flashImage.gameObject.SetActive(false);
        blackOverlay.gameObject.SetActive(false);
        pausePanel.SetActive(false);
    }

    void SetAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    void OnPause()
    {
        if (isPaused) return;
        isPaused = true;
        StartCoroutine(PauseSequence());
    }

    IEnumerator PauseSequence()
    {
        // 1. Fade in black overlay
        blackOverlay.gameObject.SetActive(true);
        yield return StartCoroutine(FadeImage(blackOverlay, 0f, 1f));

        // 2. Flash white
        flashImage.gameObject.SetActive(true);
        yield return StartCoroutine(FadeImage(flashImage, 0f, 1f));
        yield return StartCoroutine(FadeImage(flashImage, 1f, 0f));
        flashImage.gameObject.SetActive(false);

        // 3. Show pause panel
        pausePanel.SetActive(true);

        // 4. Hide all button elements first
        Transform[] children = pausePanel.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child != pausePanel.transform)
            {
                child.localScale = Vector3.zero;
                child.gameObject.SetActive(true);
            }
        }

        // 5. Scale up buttons one by one
        // 5. Scale up buttons one by one
        foreach (Transform child in children)
        {
            if (child != pausePanel.transform)
            {
                StartCoroutine(ScaleUpButton(child, buttonScaleDuration));
                yield return new WaitForSecondsRealtime(buttonDelay);
            }
        }

        // 6. Baru pause game setelah semua animasi selesai
        yield return new WaitForSecondsRealtime(0.1f); // optional buffer
        Time.timeScale = 0f;

        // 6. Pause game
        Time.timeScale = 0f;

    }

    IEnumerator FadeImage(Image img, float from, float to)
    {
        float timer = 0f;
        Color c = img.color;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, timer / fadeDuration);
            c.a = a;
            img.color = c;
            yield return null;
        }

        c.a = to;
        img.color = c;
    }

    IEnumerator ScaleUpButton(Transform target, float duration)
    {
        float timer = 0f;
        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.one;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            target.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        target.localScale = end;
    }

    public void ResumeGame()
{
    if (!isPaused) return;

    isPaused = false;

    // Matikan pause panel dan overlay
    pausePanel.SetActive(false);
    flashImage.gameObject.SetActive(false);
    blackOverlay.gameObject.SetActive(false);

    Time.timeScale = 1f;

    Debug.Log("Resume dari PhonePauseEffect");
}

}
