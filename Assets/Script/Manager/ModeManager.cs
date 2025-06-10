using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    private string selectedModeScene = "";

    [Header("Highlight Indicator")]
    public GameObject mode1Highlight;
    public GameObject mode2Highlight;

    [Header("Start Button")]
    public Button startButton;

    [Header("Mode 2 Button & Overlay")]
    public Button mode2Button;
    public GameObject mode2LockedOverlay;

    [Header("Nama Scene untuk Setiap Mode")]
    public string sceneMode1 = "Hall";
    public string sceneMode2 = "SceneMode2";

    [Header("Debug")]
    public bool forceDebugMode = false; // Untuk testing

    private void Start()
    {
        // ❗ UNCOMMENT SATU KALI BUAT RESET TES DARI AWAL
        // PlayerPrefs.DeleteKey("Mode1Completed");
        // PlayerPrefs.Save();

        CheckMode2Availability();
        InitializeUI();
    }

    private void CheckMode2Availability()
    {
        // Ambil data dari PlayerPrefs dengan lebih detail debug
        int mode1Completed = PlayerPrefs.GetInt("Mode1Completed", 0);
        
        // Debug lebih detail
        Debug.Log($"[ModeManager] === DEBUG INFO ===");
        Debug.Log($"[ModeManager] PlayerPrefs 'Mode1Completed' = {mode1Completed}");
        Debug.Log($"[ModeManager] HasKey 'Mode1Completed' = {PlayerPrefs.HasKey("Mode1Completed")}");
        Debug.Log($"[ModeManager] ForceDebugMode = {forceDebugMode}");
        
        // Logika unlock Mode 2
        bool isMode2Unlocked = (mode1Completed == 1) || forceDebugMode;
        
        Debug.Log($"[ModeManager] Mode 2 Unlocked = {isMode2Unlocked}");

        // Atur tombol mode 2
        if (mode2Button != null)
        {
            mode2Button.interactable = isMode2Unlocked;
            Debug.Log($"[ModeManager] Mode2Button.interactable = {mode2Button.interactable}");
        }
        else
        {
            Debug.LogWarning("[ModeManager] mode2Button belum di-assign di Inspector!");
        }

        // Atur overlay kunci mode 2
        if (mode2LockedOverlay != null)
        {
            mode2LockedOverlay.SetActive(!isMode2Unlocked);
            Debug.Log($"[ModeManager] mode2LockedOverlay aktif = {mode2LockedOverlay.activeSelf}");
        }
        else
        {
            Debug.LogWarning("[ModeManager] mode2LockedOverlay belum di-assign!");
        }
    }

    private void InitializeUI()
    {
        // Nonaktifkan tombol start
        if (startButton != null)
        {
            startButton.interactable = false;
        }

        // Nonaktifkan highlight di awal
        if (mode1Highlight != null) mode1Highlight.SetActive(false);
        if (mode2Highlight != null) mode2Highlight.SetActive(false);
    }

    public void SelectMode1()
    {
        selectedModeScene = sceneMode1;

        if (mode1Highlight != null) mode1Highlight.SetActive(true);
        if (mode2Highlight != null) mode2Highlight.SetActive(false);
        if (startButton != null) startButton.interactable = true;

        Debug.Log("[ModeManager] Mode 1 dipilih. Scene: " + sceneMode1);
    }

    public void SelectMode2()
    {
        // Double check apakah Mode 2 benar-benar unlocked
        int mode1Completed = PlayerPrefs.GetInt("Mode1Completed", 0);
        bool isMode2Unlocked = (mode1Completed == 1) || forceDebugMode;
        
        if (!isMode2Unlocked)
        {
            Debug.LogWarning($"[ModeManager] Mode 2 masih terkunci! Mode1Completed = {mode1Completed}");
            return;
        }

        selectedModeScene = sceneMode2;

        if (mode1Highlight != null) mode1Highlight.SetActive(false);
        if (mode2Highlight != null) mode2Highlight.SetActive(true);
        if (startButton != null) startButton.interactable = true;

        Debug.Log("[ModeManager] Mode 2 dipilih. Scene: " + sceneMode2);
    }

    public void StartGame()
    {
        if (!string.IsNullOrEmpty(selectedModeScene))
        {
            Debug.Log("[ModeManager] Memuat scene: " + selectedModeScene);
            SceneManager.LoadScene(selectedModeScene);
        }
        else
        {
            Debug.LogWarning("[ModeManager] Belum ada mode yang dipilih!");
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("Mode1Completed");
        PlayerPrefs.Save();
        Debug.Log("[ModeManager] Progress Mode 1 berhasil di-reset (Mode1Completed dihapus).");
        
        // Refresh UI setelah reset
        CheckMode2Availability();
    }

    // Method untuk debugging manual
    [ContextMenu("Force Check Mode2")]
    public void ForceCheckMode2()
    {
        CheckMode2Availability();
    }

    // Method untuk testing
    [ContextMenu("Simulate Mode1 Complete")]
    public void SimulateMode1Complete()
    {
        PlayerPrefs.SetInt("Mode1Completed", 1);
        PlayerPrefs.Save();
        CheckMode2Availability();
        Debug.Log("[ModeManager] SIMULATED: Mode 1 completed!");
    }
}