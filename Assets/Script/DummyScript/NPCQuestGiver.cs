using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class NPCQuestGiver : MonoBehaviour, Interacable
{
    [Header("Quest Data")]
    public QuestDefinition questDef;

    [Header("Dialog Lines")]
    [TextArea(2,4)] public string[] introDialogLines;
    [TextArea(2,4)] public string[] postCompletionDialogLines;

    [Header("UI & SFX")]
    public GameObject       dialogPanel;
    public TextMeshProUGUI  dialogText;
    public GameObject       nextPanel;
    public GameObject       rewardImage;
    public AudioSource      audioSource;
    public AudioClip        dialogSFX;
    public AudioClip        rewardSFX;
    public float            typingSpeed       = 0.02f;
    public float            delayBetweenLines = 1f;

    [Header("Fade Out After Completion")]
    public float fadeOutDuration = 1f; // durasi fade
    public bool  destroyAfterFade = true;

    private bool hasGivenQuest = false;
    private bool isTyping      = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (dialogPanel  != null) dialogPanel .SetActive(false);
        if (nextPanel    != null) nextPanel   .SetActive(false);
        if (rewardImage  != null) rewardImage .SetActive(false);
    }

    public bool canInteract() => true;

    public void Interact()
    {
        if (isTyping) return;
        if (!hasGivenQuest)
        {
            // Give quest
            StartCoroutine(PlayDialogSequence(introDialogLines, () =>
            {
                QuestManager2.Instance.ActivateQuest(questDef);
                hasGivenQuest = true;
                if (nextPanel) nextPanel.SetActive(true);
            }));
        }
        else
        {
            // Check completion
            Quest q = QuestManager2.Instance.GetActiveQuest(questDef.questId);
            if (q != null && q.isCompleted)
            {
                StartCoroutine(PlayDialogSequence(postCompletionDialogLines, () =>
                {
                    // Complete quest
                    QuestManager2.Instance.OnQuestCompleted.Invoke(q);
                    // Show reward, then fade out NPC
                    StartCoroutine(ShowRewardAndFadeOut());
                }));
            }
        }
    }

    private IEnumerator ShowRewardAndFadeOut()
    {
        // 1) show reward image
        if (rewardImage != null)
        {
            rewardImage.SetActive(true);
            if (rewardSFX != null && audioSource != null)
                audioSource.PlayOneShot(rewardSFX);
            yield return new WaitForSecondsRealtime(delayBetweenLines);
            rewardImage.SetActive(false);
        }
        // 2) fade out self
        yield return StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        // collect all SpriteRenderers under this GameObject
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        float timer = 0f;

        // cache original colors
        Color[][] originals = new Color[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
            originals[i] = new Color[] { renderers[i].color };

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            for (int i = 0; i < renderers.Length; i++)
            {
                Color c = renderers[i].color;
                c.a = alpha;
                renderers[i].color = c;
            }
            yield return null;
        }

        // ensure fully transparent
        foreach (var sr in renderers)
        {
            var c = sr.color;
            c.a = 0f;
            sr.color = c;
        }

        // destroy or disable
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
