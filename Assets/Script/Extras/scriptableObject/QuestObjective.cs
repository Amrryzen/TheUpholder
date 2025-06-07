using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class QuestObjective : ScriptableObject
{
    [Tooltip("Unique ID untuk objective ini")]
    public string objectiveID;
    [TextArea]
    public string description;

    public bool IsComplete { get; protected set; } = false;

    public abstract void Initialize(QuestManager manager);
    public abstract void Cleanup(QuestManager manager);
}