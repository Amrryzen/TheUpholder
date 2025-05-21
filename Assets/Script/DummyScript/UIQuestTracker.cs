using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIQuestTracker : MonoBehaviour
{
    [Header("UI References")]
    public GameObject       questPanel;
    public TextMeshProUGUI  questText;

    private QuestManager2   qm;
    private string          lastText = "";

    private void Start()
    {
        qm = QuestManager2.Instance;
        if (qm == null)
        {
            Debug.LogError("QuestManager2 tidak ditemukan!");
            enabled = false;
            return;
        }

        // Subscribe ke semua event yang merubah progress
        qm.OnQuestActivated.AddListener(_ => RefreshDisplay());
        qm.OnQuestUpdated  .AddListener(_ => RefreshDisplay());
        qm.OnQuestCompleted.AddListener(_ => RefreshDisplay());

        RefreshDisplay();
    }

    private void Update()
    {
        // Cek juga tiap frame kalau progress berubah
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        List<string> lines = qm.GetActiveQuestDescriptions();
        string combined = (lines.Count > 0) ? string.Join("\n\n", lines) : "";

        if (combined == lastText) return;
        lastText = combined;

        questPanel.SetActive(lines.Count > 0);
        questText .text = combined;
    }

    private void OnDestroy()
    {
        if (qm == null) return;
        qm.OnQuestActivated .RemoveListener(_ => RefreshDisplay());
        qm.OnQuestUpdated   .RemoveListener(_ => RefreshDisplay());
        qm.OnQuestCompleted .RemoveListener(_ => RefreshDisplay());
    }
}
