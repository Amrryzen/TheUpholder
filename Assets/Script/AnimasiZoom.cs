using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimasiZoom : MonoBehaviour
{
    public RectTransform panelToAnimate;
    public RectTransform startPoint; // Titik asal animasi (biasanya tombol)
    public float animationDuration = 0.5f;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private bool isAnimating = false;

    void Start()
    {
        originalScale = panelToAnimate.localScale;
        originalPosition = panelToAnimate.position;

        panelToAnimate.localScale = Vector3.zero;
        panelToAnimate.gameObject.SetActive(false);
    }

    public void OpenPanel()
    {
        if (isAnimating || panelToAnimate.gameObject.activeSelf) return;

        panelToAnimate.gameObject.SetActive(true);
        panelToAnimate.localScale = Vector3.zero;
        panelToAnimate.position = startPoint.position;

        StartCoroutine(ZoomPanel(startPoint.position, originalPosition, Vector3.zero, originalScale, true));
    }

    public void ClosePanel()
    {
        if (isAnimating || !panelToAnimate.gameObject.activeSelf) return;

        StartCoroutine(ZoomPanel(originalPosition, startPoint.position, originalScale, Vector3.zero, false));
    }

    IEnumerator ZoomPanel(Vector3 fromPos, Vector3 toPos, Vector3 fromScale, Vector3 toScale, bool opening)
    {
        isAnimating = true;

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            float t = elapsed / animationDuration;
            panelToAnimate.position = Vector3.Lerp(fromPos, toPos, t);
            panelToAnimate.localScale = Vector3.Lerp(fromScale, toScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panelToAnimate.position = toPos;
        panelToAnimate.localScale = toScale;

        if (!opening)
            panelToAnimate.gameObject.SetActive(false);

        isAnimating = false;
    }
}
