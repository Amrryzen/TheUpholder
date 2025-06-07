using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questId;
    public string questName;
    public string description;
    public QuestType questTypes;     // ← support multiple types
    public string[] targetTags;
    public int requiredCount;

    [HideInInspector] public int progress;
    [HideInInspector] public bool isActive;
    [HideInInspector] public bool isCompleted;

    public Quest(QuestDefinition def)
    {
        questId       = def.questId;
        questName     = def.questName;
        description   = def.description;
        questTypes    = def.questTypes;
        targetTags    = def.targetTags;
        requiredCount = def.requiredCount;
        progress      = 0;
        isActive      = false;
        isCompleted   = false;
    }

    public void AddProgressForTag(string taggedObject)
    {
        if (isCompleted) return;
        if (System.Array.IndexOf(targetTags, taggedObject) < 0)
            return;

        progress = Mathf.Min(progress + 1, requiredCount);
        if (progress >= requiredCount)
            isCompleted = true;

        Debug.Log($"[{questName}] Progress: {progress}/{requiredCount}");
    }

    public string GetDescriptionWithProgress()
    {
        return $"{questName} ({progress}/{requiredCount})\n{description}";
    }

    public bool HasQuestType(QuestType type)
    {
        return (questTypes & type) == type;
    }
}
