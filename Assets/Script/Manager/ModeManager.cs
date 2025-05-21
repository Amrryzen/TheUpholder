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

    [Header("Mode Buttons")]
    public Button mode2Button;
    public GameObject mode2LockedOverlay; // Gambar atau teks untuk tampilkan kunci di mode 2

    void Start()
    {
        // Cek apakah Mode 1 sudah diselesaikan sebelumnya
        bool mode1Completed = PlayerPrefs.GetInt("Mode1Completed", 0) == 1;

        // Atur interaksi tombol mode 2 berdasarkan progres
        if (mode2Button != null)
            mode2Button.interactable = mode1Completed;

        if (mode2LockedOverlay != null)
            mode2LockedOverlay.SetActive(!mode1Completed);

        // Nonaktifkan tombol Start di awal
        if (startButton != null)
            startButton.interactable = false;

        // Nonaktifkan highlight di awal
        if (mode1Highlight != null)
            mode1Highlight.SetActive(false);

        if (mode2Highlight != null)
            mode2Highlight.SetActive(false);
    }

    // Tombol Mode 1
    public void SelectMode1()
    {
        selectedModeScene = "Hall"; // Ganti dengan nama scene sebenarnya
        if (mode1Highlight != null)
            mode1Highlight.SetActive(true);

        if (mode2Highlight != null)
            mode2Highlight.SetActive(false);

        if (startButton != null)
            startButton.interactable = true;
    }

    // Tombol Mode 2
    public void SelectMode2()
    {
        // Cek dulu apakah mode ini sudah terbuka
        if (PlayerPrefs.GetInt("Mode1Completed", 0) != 1)
        {
            Debug.Log("Mode 2 masih terkunci!");
            return;
        }

        selectedModeScene = "SceneMode2"; // Ganti dengan nama scene sebenarnya
        if (mode1Highlight != null)
            mode1Highlight.SetActive(false);

        if (mode2Highlight != null)
            mode2Highlight.SetActive(true);

        if (startButton != null)
            startButton.interactable = true;
    }

    // Tombol Start
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

    // 🔁 Untuk testing: reset progress (bisa panggil lewat tombol UI sementara)
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("Mode1Completed");
        PlayerPrefs.Save();
        Debug.Log("Progress berhasil di-reset!");
    }
}
