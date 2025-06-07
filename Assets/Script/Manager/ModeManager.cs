using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    private string selectedModeScene = "";

    [Header("Highlight Indicator")]
    [Tooltip("Objek UI yang akan diaktifkan untuk men‐highlight Mode 1 bila dipilih.")]
    public GameObject mode1Highlight;

    [Tooltip("Objek UI yang akan diaktifkan untuk men‐highlight Mode 2 bila dipilih.")]
    public GameObject mode2Highlight;

    [Header("Start Button")]
    [Tooltip("Tombol untuk memulai scene setelah mode dipilih.")]
    public Button startButton;

    [Header("Mode 2 Button & Overlay")]
    [Tooltip("Tombol UI untuk memilih Mode 2.")]
    public Button mode2Button;

    [Tooltip("Objek UI (misalnya gembok) yang tampilkan bahwa Mode 2 terkunci.")]
    public GameObject mode2LockedOverlay;

    [Header("Nama Scene untuk Setiap Mode")]
    [Tooltip("Nama scene Mode 1 (misalnya \"Hall\").")]
    public string sceneMode1 = "Hall";

    [Tooltip("Nama scene Mode 2 (misalnya \"SceneMode2\").")]
    public string sceneMode2 = "SceneMode2";

    private void Start()
    {
        // 1) Cek apakah Mode 1 sudah selesai (PlayerPrefs["Mode1Completed"] == 1)
        bool mode1Completed = PlayerPrefs.GetInt("Mode1Completed", 0) == 1;

        // 2) Atur interaktif/tampilan Mode 2 sesuai progres
        if (mode2Button != null)
            mode2Button.interactable = mode1Completed;

        if (mode2LockedOverlay != null)
            mode2LockedOverlay.SetActive(!mode1Completed);

        // 3) Nonaktifkan tombol Start di awal
        if (startButton != null)
            startButton.interactable = false;

        // 4) Nonaktifkan highlight pada kedua mode di awal
        if (mode1Highlight != null)
            mode1Highlight.SetActive(false);
        if (mode2Highlight != null)
            mode2Highlight.SetActive(false);
    }

    /// <summary>
    /// Dipanggil saat tombol atau UI Mode 1 diklik.
    /// </summary>
    public void SelectMode1()
    {
        selectedModeScene = sceneMode1;
        if (mode1Highlight != null)
            mode1Highlight.SetActive(true);
        if (mode2Highlight != null)
            mode2Highlight.SetActive(false);
        if (startButton != null)
            startButton.interactable = true;
    }

    /// <summary>
    /// Dipanggil saat tombol atau UI Mode 2 diklik.
    /// Mode 2 hanya bisa dipilih kalau PlayerPrefs["Mode1Completed"] == 1.
    /// </summary>
    public void SelectMode2()
    {
        // Pastikan Mode 1 sudah selesai
        if (PlayerPrefs.GetInt("Mode1Completed", 0) != 1)
        {
            Debug.Log("Mode 2 masih terkunci!");
            return;
        }

        selectedModeScene = sceneMode2;
        if (mode1Highlight != null)
            mode1Highlight.SetActive(false);
        if (mode2Highlight != null)
            mode2Highlight.SetActive(true);
        if (startButton != null)
            startButton.interactable = true;
    }

    /// <summary>
    /// Dipanggil saat tombol Start diklik; akan memuat scene yang sudah di‐set di selectedModeScene.
    /// </summary>
    public void StartGame()
    {
        if (!string.IsNullOrEmpty(selectedModeScene))
        {
            SceneManager.LoadScene(selectedModeScene);
        }
        else
        {
            Debug.LogWarning("Pilih mode terlebih dahulu!");
        }
    }

    /// <summary>
    /// Kalau perlu, panggil fungsi ini untuk reset progress Mode 1 (misalnya lewat tombol debug).
    /// </summary>
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("Mode1Completed");
        PlayerPrefs.Save();
        Debug.Log("Progress Mode 1 berhasil di-reset!");
    }
}
