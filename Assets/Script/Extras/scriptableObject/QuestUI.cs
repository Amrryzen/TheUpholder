using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject entryPrefab;

    private void OnEnable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestEvent += OnQuestEvent;
    }

    private void OnDisable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestEvent -= OnQuestEvent;
    }

    void Start()
    {
        RefreshQuestList();
    }

    private void OnQuestEvent(string eventName, object data)
    {
        // Refresh UI on relevant quest events
        if (eventName == "QuestStarted" ||
            eventName == "QuestCompleted" ||
            eventName == "OnPhotoCaptured")
        {
            RefreshQuestList();
        }
    }

    public void RefreshQuestList()
    {
        // Clear existing entries
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // Rebuild list for active quests
        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            // Quest title header
            GameObject header = Instantiate(entryPrefab, contentParent);
            TMP_Text headerText = header.GetComponentInChildren<TMP_Text>();
            headerText.text = quest.questTitle;

            // Objectives
            foreach (var obj in quest.objectives)
            {
                GameObject entry = Instantiate(entryPrefab, contentParent);
                TMP_Text text = entry.GetComponentInChildren<TMP_Text>();

                if (obj is PhotoObjective photo)
                {
                    text.text = $"- {photo.description} ({photo.CurrentCount}/{photo.RequiredCount})";
                }
                else
                {
                    int curr = obj.IsComplete ? 1 : 0;
                    text.text = $"- {obj.description} ({curr}/1)";
                }
            }
        }
    }
}
