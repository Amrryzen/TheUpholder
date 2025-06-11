using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Sprite yang Diaktifkan Saat Quest Selesai")]
    public GameObject spriteToActivateOnCompletion;

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
        if (spriteToActivateOnCompletion != null) spriteToActivateOnCompletion.SetActive(false);
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
                    // Trigger quest complete event
                    QuestManager2.Instance.OnQuestCompleted.Invoke(q);

                    // Pindah scene jika ada
                    if (unlockManager != null)
                        unlockManager.LoadMenuScene(menuSceneName);

                    // Tampilkan reward & efek fade
                    StartCoroutine(ShowRewardAndFadeOut());

                    // Hapus quest setelah semua proses
                    QuestManager2.Instance.RemoveQuest(questDef.questId);
                }));
            }
        }
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

        // ✅ Aktifkan sprite 2D target jika ada
        if (spriteToActivateOnCompletion != null)
            spriteToActivateOnCompletion.SetActive(true);

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
