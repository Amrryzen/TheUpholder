using UnityEngine;

[System.Serializable]
public class Quest
{
    public string    questId;
    public string    questName;
    public string    description;
    public QuestType questType;
    public string[]  targetTags;      // ← multiple tags
    public int       requiredCount;

    [HideInInspector] public int  progress;
    [HideInInspector] public bool isActive;
    [HideInInspector] public bool isCompleted;

    public Quest(QuestDefinition def)
    {
        questId       = def.questId;
        questName     = def.questName;
        description   = def.description;
        questType     = def.questType;
        targetTags    = def.targetTags;
        requiredCount = def.requiredCount;
        progress      = 0;
        isActive      = false;
        isCompleted   = false;
    }

    public void AddProgressForTag(string taggedObject)
    {
        // hanya jika tag cocok salah satu targetTags
        if (isCompleted) return;
        if ( System.Array.IndexOf(targetTags, taggedObject) < 0 ) 
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
}
