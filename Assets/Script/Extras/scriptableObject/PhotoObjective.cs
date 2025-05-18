using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Objectives/Photo Objective")]
public class PhotoObjective : QuestObjective
{
    [Tooltip("ID quest yang akan di-track oleh objective ini")]
    public string questID;

    [Tooltip("Event name yang diterbitkan saat capture foto")]
    public string captureEventName = "OnPhotoCaptured";

    [Tooltip("Jumlah foto yang dibutuhkan")]
    public int requiredCount = 3;

    // Internal counter untuk progress
    private int currentCount = 0;

    // Expose progress ke UI dan sistem lain
    public int CurrentCount => currentCount;
    public int RequiredCount => requiredCount;

    /// <summary>
    /// Subscribe ke QuestManager.OnQuestEvent
    /// </summary>
    public override void Initialize(QuestManager manager)
    {
        // Subscribe ke event yang benar
        manager.OnQuestEvent += HandleEvent;
    }

    /// <summary>
    /// Unsubscribe dari QuestManager.OnQuestEvent
    /// </summary>
    public override void Cleanup(QuestManager manager)
    {
        manager.OnQuestEvent -= HandleEvent;
    }

    /// <summary>
    /// Dijalankan setiap ada event dari QuestManager
    /// </summary>
    private void HandleEvent(string eventName, object data)
    {
        // Pastikan ini event foto, dan untuk quest yang benar
        if (eventName != captureEventName) return;
        if (!(data is string firedQuestID) || firedQuestID != questID) return;

        if (IsComplete) 
            return;

        // Increment dan clamp
        currentCount = Mathf.Clamp(currentCount + 1, 0, requiredCount);
        Debug.Log($"[PhotoObjective] ({questID}) Foto diambil: ({currentCount}/{requiredCount})");

        // Tandai selesai jika sudah mencukupi
        if (currentCount >= requiredCount)
        {
            IsComplete = true;

            // Opsional: notify QuestManager bahwa objective ini selesai
            // manager.PublishEvent("ObjectiveCompleted", questID);
        }
    }
}