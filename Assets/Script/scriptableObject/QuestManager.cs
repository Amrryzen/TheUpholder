using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Daftar Semua Quest (assign di Inspector)")]
    public List<QuestData> allQuests;

    // Quest yang sudah di-start
    public List<QuestData> activeQuests = new List<QuestData>();

    // Lookup cepat by ID
    private Dictionary<string, QuestData> lookup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Build lookup table
            lookup = new Dictionary<string, QuestData>();
            foreach (var q in allQuests)
                lookup[q.questID] = q;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartQuest(string questID)
    {
        if (lookup.TryGetValue(questID, out var quest) && !activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
            Debug.Log($"[QuestManager] Quest started: {quest.questTitle}");

            // Update UI jika QuestUI aktif di scene
            var questUI = FindObjectOfType<QuestUI>();
            if (questUI != null)
                questUI.RefreshQuestList();
        }
    }

}
