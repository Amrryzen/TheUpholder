using UnityEngine;

public enum QuestType
{
    Photograph,
    CollectItem,
    // dll.
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest Definition")]
public class QuestDefinition : ScriptableObject
{
    [Header("Identitas Quest")]
    public string questId;
    public string questName;
    [TextArea(3,5)]
    public string description;

    [Header("Tipe & Target")]
    public QuestType       questType;
    public string[]        targetTags;     // ← array of tags
    public int             requiredCount;
}
