using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class StoryUnlockManager : MonoBehaviour
{
    [Header("Quest ID untuk quest terakhir Mode 1")]
    public string finalQuest1Id = "QuestA_Final";

    [Header("UI Fade Image (tanpa CanvasGroup)")]
    public Image fadeImage; // Drag Image UI ke sini dari Inspector

    private void Start()
    {
        if (QuestManager2.Instance != null)
        {
            QuestManager2.Instance.OnQuestCompleted.AddListener(OnQuestCompleted);
        }
        else
        {
            Debug.LogError("StoryUnlockManager: QuestManager2.Instance == null!");
        }
    }

    private void OnDestroy()
    {
        if (QuestManager2.Instance != null)
            QuestManager2.Instance.OnQuestCompleted.RemoveListener(OnQuestCompleted);
    }

    private void OnQuestCompleted(Quest q)
    {
        if (q.questId == finalQuest1Id)
        {
            Debug.Log($"[StoryUnlockManager] Quest terakhir Mode 1 ({finalQuest1Id}) selesai. Men‐set Mode1Completed = 1 (unlock Mode 2).");
            PlayerPrefs.SetInt("Mode1Completed", 1);
            PlayerPrefs.Save();
        }
    }

    public void LoadMenuScene(string menuSceneName)
    {
        if (!string.IsNullOrEmpty(menuSceneName))
            StartCoroutine(LoadSceneWithFade(menuSceneName));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        yield return new WaitForSeconds(3f); // Delay sebelum fade

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);

            // Fade in (alpha 0 → 1)
            yield return StartCoroutine(FadeImage(0f, 1f, 1f));

            yield return new WaitForSeconds(1f); // Tunggu sebentar

            // Fade out (alpha 1 → 0)
            yield return StartCoroutine(FadeImage(1f, 0f, 1f));

            fadeImage.gameObject.SetActive(false);
        }

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeImage(float fromAlpha, float toAlpha, float duration)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, toAlpha);
    }
}
