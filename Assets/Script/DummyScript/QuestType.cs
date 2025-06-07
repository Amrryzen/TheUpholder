using UnityEngine;

[System.Flags]
public enum QuestType
{
    None         = 0,
    Photograph   = 1 << 0,
    CollectItem  = 1 << 1,
    // Tambah lainnya jika perlu
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest Definition")]
public class QuestDefinition : ScriptableObject
{
    [Header("Identitas Quest")]
    public string questId;
    public string questName;
    [TextArea(3, 5)]
    public string description;

    [Header("Tipe & Target")]
    public QuestType questTypes;     // ← multiple types allowed
    public string[] targetTags;      // ← array of tags
    public int requiredCount;
}

