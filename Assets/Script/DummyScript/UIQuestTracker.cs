using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIQuestTracker : MonoBehaviour
{
    [Header("UI references")]
    public TextMeshProUGUI questText;
    public GameObject questPanel;

    private QuestManager2 questManager;

    // cache agar tidak perlu mem‑set teks setiap frame
    private string lastDisplayedText = "";

    private void Start()
    {
        questManager = QuestManager2.Instance;

        if (questManager == null)
        {
            Debug.LogError("QuestManager tidak ditemukan!");
            enabled = false;
            return;
        }

        // tetap subscribe – kalau ada quest baru / selesai, teks langsung refresh
        questManager.OnQuestActivated.AddListener(UpdateQuestDisplay);
        questManager.OnQuestCompleted.AddListener(UpdateQuestDisplay);

        UpdateQuestDisplay();
    }

    private void Update()
    {
        // cek setiap frame untuk mendeteksi perubahan progres (mis. 1/3 → 2/3)
        UpdateQuestDisplay();
    }

    /// <summary>Refresh panel jika teks berubah.</summary>
    private void UpdateQuestDisplay(Quest q = null)
    {
        if (questManager == null) return;

        List<string> lines = questManager.GetActiveQuestDescriptions(); // harus sudah mengandung progres

        string combined = string.Join("\n\n", lines);

        if (combined == lastDisplayedText) return;      // tidak berubah, abaikan

        lastDisplayedText = combined;

        if (lines.Count > 0)
        {
            questPanel.SetActive(true);
            questText.text = combined;
        }
        else
        {
            questPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (questManager == null) return;
        questManager.OnQuestActivated.RemoveListener(UpdateQuestDisplay);
        questManager.OnQuestCompleted.RemoveListener(UpdateQuestDisplay);
    }
}
