using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Daftar Semua Quest (assign di Inspector)")]
    public List<QuestData> allQuests;

    [HideInInspector]
    public List<QuestData> activeQuests = new List<QuestData>();

    private Dictionary<string, QuestData> lookup;
    public event Action<string, object> OnQuestEvent;

    [Header("Debug Settings (Inspector)")]
    [Tooltip("Quest ID to start via inspector")] public string questToStartID;
    [Tooltip("Quest ID to complete via inspector")] public string questToCompleteID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Build lookup
            lookup = new Dictionary<string, QuestData>();
            foreach (var q in allQuests)
            {
                if (string.IsNullOrEmpty(q.questID))
                {
                    Debug.LogWarning($"[QuestManager] Quest '{q.questTitle}' memiliki questID kosong.");
                    continue;
                }
                if (lookup.ContainsKey(q.questID))
                {
                    Debug.LogWarning($"[QuestManager] Duplicate questID: {q.questID}");
                    continue;
                }
                lookup.Add(q.questID, q);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Debug via Inspector
        if (!string.IsNullOrEmpty(questToStartID)) StartQuest(questToStartID);
        if (!string.IsNullOrEmpty(questToCompleteID)) CompleteQuest(questToCompleteID);
    }

    /// <summary>
    /// Mulai quest berdasarkan ID
    /// </summary>
    public void StartQuest(string questID)
    {
        if (!lookup.TryGetValue(questID, out var quest))
        {
            Debug.LogWarning($"[QuestManager] QuestID '{questID}' tidak ditemukan.");
            return;
        }
        if (activeQuests.Contains(quest))
        {
            Debug.LogWarning($"[QuestManager] Quest '{quest.questTitle}' sudah aktif.");
            return;
        }

        activeQuests.Add(quest);
        foreach (var obj in quest.objectives)
            obj.Initialize(this);

        Debug.Log($"[QuestManager] Quest started: {quest.questTitle} (ID: {questID})");
        OnQuestEvent?.Invoke("QuestStarted", questID);
    }

    /// <summary>
    /// Kirim event ke objectives
    /// </summary>
    public void PublishEvent(string eventName, object data = null)
    {
        OnQuestEvent?.Invoke(eventName, data);
    }

    /// <summary>
    /// Cek apakah quest aktif
    /// </summary>
    public bool IsQuestActive(string questID)
    {
        return activeQuests.Exists(q => q.questID == questID);
    }

    /// <summary>
    /// Selesaikan quest dan bersihkan
    /// </summary>
    public void CompleteQuest(string questID)
    {
        if (!lookup.TryGetValue(questID, out var quest))
        {
            Debug.LogWarning($"[QuestManager] QuestID '{questID}' tidak ditemukan.");
            return;
        }
        if (!activeQuests.Contains(quest))
        {
            Debug.LogWarning($"[QuestManager] Quest '{quest.questTitle}' belum aktif atau sudah selesai.");
            return;
        }

        foreach (var obj in quest.objectives)
            obj.Cleanup(this);
        activeQuests.Remove(quest);

        Debug.Log($"[QuestManager] Quest complete: {quest.questTitle} (ID: {questID})");
        OnQuestEvent?.Invoke("QuestCompleted", questID);
    }

    private void OnDestroy()
    {
        // Unsubscribe all to avoid leaks
        OnQuestEvent = null;
    }
}
