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

    public void ActivateQuest(QuestDefinition def)
    {
        if (activeQuests.ContainsKey(def.questId)) return;

        Quest q = new Quest(def);
        q.isActive = true;
        activeQuests.Add(def.questId, q);
        OnQuestActivated.Invoke(q);
        Debug.Log($"Quest activated: {q.questName}");
    }

    public void ProcessAction(string taggedObject)
    {
        foreach (var kv in activeQuests)
        {
            Quest q = kv.Value;
            if (q.isCompleted) continue;

            q.AddProgressForTag(taggedObject);
            OnQuestUpdated.Invoke(q);

            if (q.isCompleted)
            {
                OnQuestCompleted.Invoke(q);
                Debug.Log($"Quest completed: {q.questName}");
            }
        }
    }

    public bool HasActivePhotoQuest()
    {
        return activeQuests.Values.Any(q => q.HasQuestType(QuestType.Photograph) && !q.isCompleted);
    }

    public Quest GetActiveQuest(string questId)
    {
        return activeQuests.TryGetValue(questId, out var q) ? q : null;
    }

    public List<string> GetActiveQuestDescriptions()
    {
        return activeQuests.Values.Select(q => q.GetDescriptionWithProgress()).ToList();
    }
}
