using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class NPCQuestGiver : MonoBehaviour, Interacable
{
    [Header("Quest Data")]
    public QuestDefinition questDef;

    [Header("Collectibles To Activate")]
    public GameObject[] collectiblesToActivate;

    [Header("Dialog Lines")]
    [TextArea(2, 4)] public string[] introDialogLines;
    [TextArea(2, 4)] public string[] postCompletionDialogLines;

    [Header("UI & SFX")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public GameObject nextPanel;
    public GameObject rewardImage;
    public AudioSource audioSource;
    public AudioClip dialogSFX;
    public AudioClip rewardSFX;
    public float typingSpeed = 0.02f;
    public float delayBetweenLines = 1f;

    [Header("Fade Out After Completion")]
    public float fadeOutDuration = 1f;
    public bool destroyAfterFade = true;

    [Header("Nama Scene Menu Pilih Mode (untuk Unlock)")]
    public string menuSceneName = "StorySelectScene";

    [Header("Panel Saat Quest Selesai")]
    public GameObject questCompletePanel;

    private bool hasGivenQuest = false;
    private bool isTyping = false;
    private StoryUnlockManager unlockManager;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        unlockManager = FindObjectOfType<StoryUnlockManager>();
        if (unlockManager == null)
            Debug.LogWarning("NPCQuestGiver: tidak menemukan StoryUnlockManager di scene!");

        if (dialogPanel != null) dialogPanel.SetActive(false);
        if (nextPanel != null) nextPanel.SetActive(false);
        if (rewardImage != null) rewardImage.SetActive(false);
        if (questCompletePanel != null) questCompletePanel.SetActive(false);
    }

    public bool canInteract() => true;

    public void Interact()
    {
        if (isTyping) return;

        if (!hasGivenQuest)
        {
            // Beri quest + dialog
            StartCoroutine(PlayDialogSequence(introDialogLines, () =>
            {
                QuestManager2.Instance.ActivateQuest(questDef);
                hasGivenQuest = true;

                foreach (var obj in collectiblesToActivate)
                {
                    if (obj != null)
                        obj.SetActive(true);
                }

                if (nextPanel) nextPanel.SetActive(true);
            }));
        }
        else
        {
            // Cek apakah quest selesai
            Quest q = QuestManager2.Instance.GetActiveQuest(questDef.questId);
            if (q != null && q.isCompleted)
            {
                StartCoroutine(PlayDialogSequence(postCompletionDialogLines, () =>
                {
                    // Mark quest selesai
                    QuestManager2.Instance.OnQuestCompleted.Invoke(q);

                    // Tampilkan panel quest selesai
                    ShowQuestCompletePanel();

                    // Pindah scene jika diatur
                    if (unlockManager != null)
                        unlockManager.LoadMenuScene(menuSceneName);

                    // Efek reward dan fade
                    StartCoroutine(ShowRewardAndFadeOut());
                }));
            }
        }
    }

    private void ShowQuestCompletePanel()
    {
        if (questCompletePanel != null)
            questCompletePanel.SetActive(true);
    }

    private IEnumerator ShowRewardAndFadeOut()
    {
        if (rewardImage != null)
        {
            rewardImage.SetActive(true);
            if (rewardSFX != null && audioSource != null)
                audioSource.PlayOneShot(rewardSFX);
            yield return new WaitForSecondsRealtime(delayBetweenLines);
            rewardImage.SetActive(false);
        }

        yield return StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            foreach (var sr in renderers)
            {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
            }
            yield return null;
        }

        foreach (var sr in renderers)
        {
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }

        if (destroyAfterFade)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private IEnumerator PlayDialogSequence(string[] lines, Action onComplete)
    {
        isTyping = true;

        if (dialogSFX != null && audioSource != null)
            audioSource.PlayOneShot(dialogSFX);

        dialogPanel.SetActive(true);

        foreach (var line in lines)
        {
            dialogText.text = "";
            foreach (char c in line)
            {
                dialogText.text += c;
                yield return new WaitForSecondsRealtime(typingSpeed);
            }
            yield return new WaitForSecondsRealtime(delayBetweenLines);
        }

        dialogPanel.SetActive(false);
        isTyping = false;
        onComplete?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
