using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Apps : MonoBehaviour
{
    [System.Serializable]
    public class ButtonPanelPair
    {
        public Button triggerButton;
        public GameObject targetPanel;
    }

    public float scaleDuration = 0.3f;
    public Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);
    public ButtonPanelPair[] buttonPanelPairs;

    void Start()
    {
        foreach (var pair in buttonPanelPairs)
        {
            if (pair.triggerButton != null && pair.targetPanel != null)
            {
                pair.targetPanel.transform.localScale = Vector3.zero;
                pair.targetPanel.SetActive(false);
                pair.triggerButton.onClick.AddListener(() =>
                {
                    StartCoroutine(AnimatePanel(pair.targetPanel));
                });
            }
        }
    }

    IEnumerator AnimatePanel(GameObject panel)
    {
        panel.SetActive(true);
        Transform t = panel.transform;
        t.localScale = startScale;

        float timer = 0f;
        while (timer < scaleDuration)
        {
            timer += Time.unscaledDeltaTime;
            float tVal = Mathf.Clamp01(timer / scaleDuration);
            float scale = EaseOutBack(tVal); // Optional ease
            t.localScale = Vector3.one * scale;
            yield return null;
        }
        t.localScale = Vector3.one;
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }
}
