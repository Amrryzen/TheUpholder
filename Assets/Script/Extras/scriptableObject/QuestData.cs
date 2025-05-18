using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Data", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [Tooltip("Unique ID untuk memanggil quest ini")]
    public string questID;

    [Tooltip("Judul quest yang ditampilkan di UI")]
    public string questTitle;

    [TextArea(3, 10)]
    [Tooltip("Deskripsi singkat tujuan quest")]
    public string description;

    [Tooltip("List of objectives for this quest")]
    public QuestObjective[] objectives;
}