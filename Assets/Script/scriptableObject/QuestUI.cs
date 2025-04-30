using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    [Header("UI Container & Prefab")]
    public Transform contentParent;     // Container yang punya Vertical Layout Group
    public GameObject entryPrefab;      // Prefab quest entry, isinya TMP_Text saja

    void Start()
    {
        RefreshQuestList();
    }

    /// <summary>
    /// Refresh daftar quest yang sedang aktif.
    /// </summary>
    public void RefreshQuestList()
    {
        Debug.Log("[QuestUI] RefreshQuestList dipanggil");

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            Debug.Log($"[QuestUI] Menambahkan quest: {quest.questTitle}");
            GameObject entry = Instantiate(entryPrefab, contentParent);
            TMP_Text questText = entry.GetComponentInChildren<TMP_Text>();
            if (questText != null)
            {
                questText.text = $"• {quest.questTitle}";
            }
        }
    }

}
