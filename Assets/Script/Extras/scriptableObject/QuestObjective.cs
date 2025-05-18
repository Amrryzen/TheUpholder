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

    /// <summary>
    /// Subscribe listener ke QuestManager Events
    /// </summary>
    public abstract void Initialize(QuestManager manager);

    /// <summary>
    /// Unsubscribe listener
    /// </summary>
    public abstract void Cleanup(QuestManager manager);
}