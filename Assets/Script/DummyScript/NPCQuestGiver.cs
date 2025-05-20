using System.Collections;
using UnityEngine;
using TMPro;

public class NPCQuestGiver : MonoBehaviour, Interacable
{
    [Header("Additional UI")]
    public GameObject nextPanel;
    [Header("Quest Settings")]
    public string questId;
    public float interactionDistance = 2f;

    [Header("Dialog Settings")]
    public GameObject dialogPanel;
    public TextMeshProUGUI dialogText;
    public float typingSpeed = 0.02f;
    public float delayBetweenLines = 1f;

    [Header("NPC Info")]
    public string npcName = "Pak Fotografi";

    [Header("Dialog Lines")]
    [TextArea(2, 4)]
    public string[] introDialogLines = {
        "Hai! Namaku Pak Fotografi. Aku fotografer profesional.",
        "Aku melihat kamera di tanganmu... kamu tampaknya punya bakat!",
        "Aku punya tugas penting: ambil beberapa foto anomali di sekitar desa."
    };

    [TextArea(2, 4)]
    public string[] postCompletionDialogLines = {
        "Terima kasih! Foto-foto ini akan sangat berguna untuk penelitianku.",
        "Kau benar-benar punya mata seorang fotografer.",
        "Kalau sempat, mampirlah lagi. Mungkin aku punya tugas lain."
    };

    private QuestManager2 questManager;
    private bool hasGivenQuest = false;
    private bool questCompleted = false;
    private bool isTyping = false;

    private void Start()
    {
        questManager = QuestManager2.Instance;

        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        if (nextPanel != null)
            nextPanel.SetActive(false);
    }

    public bool canInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (isTyping) return;

        if (!hasGivenQuest)
        {
            StartCoroutine(PlayDialogSequence(introDialogLines, () =>
{
    if (nextPanel != null)
        nextPanel.SetActive(true); // Tampilkan panel setelah dialog bagian 1

    GiveQuest(); // Tetap aktifkan quest setelah panel muncul
}));
        }
        else if (IsQuestComplete())
        {
            StartCoroutine(PlayDialogSequence(postCompletionDialogLines, () =>
            {
                CompleteQuest();
            }));
        }
    }

    private void GiveQuest()
    {
        questManager?.ActivateQuest(questId);
        hasGivenQuest = true;
    }

    private bool IsQuestComplete()
    {
        Quest quest = questManager?.activeQuests.Find(q => q.questId == questId);
        if (quest != null && quest.requiresPhotography)
        {
            return quest.photosTaken >= quest.photosRequired;
        }
        return false;
    }

    private void CompleteQuest()
    {
        Quest quest = questManager?.activeQuests.Find(q => q.questId == questId);
        if (quest != null)
        {
            questManager?.CompleteQuest(quest);
            questCompleted = true;
        }
    }

    private IEnumerator PlayDialogSequence(string[] lines, System.Action onComplete = null)
    {
        isTyping = true;
        dialogPanel.SetActive(true);

        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSecondsRealtime(delayBetweenLines);
        }

        dialogPanel.SetActive(false);
        isTyping = false;

        onComplete?.Invoke();
    }

    private IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
