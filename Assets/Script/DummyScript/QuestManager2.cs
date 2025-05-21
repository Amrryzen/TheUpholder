using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager2 : MonoBehaviour
{
    public static QuestManager2 Instance { get; private set; }

    [Header("Events")]
    public UnityEvent<Quest> OnQuestActivated;
    public UnityEvent<Quest> OnQuestUpdated;
    public UnityEvent<Quest> OnQuestCompleted;

    // Semua quest aktif, keyed by questId
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            OnQuestActivated = OnQuestActivated ?? new UnityEvent<Quest>();
            OnQuestUpdated   = OnQuestUpdated   ?? new UnityEvent<Quest>();
            OnQuestCompleted = OnQuestCompleted ?? new UnityEvent<Quest>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>Aktifkan quest baru berdasarkan definition.</summary>
    public void ActivateQuest(QuestDefinition def)
    {
        if (activeQuests.ContainsKey(def.questId)) return;

        Quest q = new Quest(def);
        q.isActive = true;
        activeQuests.Add(def.questId, q);
        OnQuestActivated.Invoke(q);
        Debug.Log($"Quest activated: {q.questName}");
    }

    /// <summary>Generic process: dipanggil saat foto diambil atau item di-pickup.</summary>
    public void ProcessAction(string taggedObject)
{
    foreach (var kv in activeQuests)
    {
        Quest q = kv.Value;
        if (q.isCompleted) continue;

        q.AddProgressForTag(taggedObject);    // tarik kemajuan berdasarkan tag
        OnQuestUpdated.Invoke(q);

        if (q.isCompleted)
        {
            OnQuestCompleted.Invoke(q);
            Debug.Log($"Quest completed: {q.questName}");
        }
    }
}


    /// <summary>Apakah ada quest fotografi aktif dan belum selesai?</summary>
    public bool HasActivePhotoQuest()
    {
        return activeQuests.Values.Any(q => q.questType == QuestType.Photograph && !q.isCompleted);
    }

    /// <summary>Ambil instance quest aktif menurut ID, atau null.</summary>
    public Quest GetActiveQuest(string questId)
    {
        return activeQuests.TryGetValue(questId, out var q) ? q : null;
    }

    /// <summary>Kembalikan semua deskripsi quest aktif (dengan progress).</summary>
    public List<string> GetActiveQuestDescriptions()
    {
        var list = new List<string>();
        foreach (var kv in activeQuests)
            list.Add(kv.Value.GetDescriptionWithProgress());
        return list;
    }
}
