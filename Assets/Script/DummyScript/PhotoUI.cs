using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhotoUI : MonoBehaviour
{
    public RectTransform imageTransform;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;
    public float riseDistance = 100f;

    private Vector2 startPos;

    void Start()
    {
        if (imageTransform != null)
        {
            startPos = imageTransform.anchoredPosition;
            canvasGroup.alpha = 0f;
        }
    }

    public void ShowPhoto()
    {
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        Vector2 endPos = startPos + Vector2.up * riseDistance;
        float elapsed = 0f;
        canvasGroup.alpha = 0f;
        imageTransform.anchoredPosition = startPos;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            imageTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        imageTransform.anchoredPosition = endPos;
    }
}
