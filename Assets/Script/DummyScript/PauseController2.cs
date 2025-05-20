using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController2 : MonoBehaviour
{
     public static bool IsGamePaused { get; private set; }
    public static bool IsDialogActive { get; private set; } = false; // NEW

    public GameObject pauseMenuUI;

    private void Start()
    {
        IsGamePaused = false;
        IsDialogActive = false;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsDialogActive)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        IsGamePaused = !IsGamePaused;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(IsGamePaused);
        }

        Time.timeScale = IsGamePaused ? 0f : 1f;
    }

    public void ResumeGame()
    {
        IsGamePaused = false;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Jangan resume jika dialog sedang aktif
        if (!IsDialogActive)
        {
            Time.timeScale = 1f;
        }
    }

    // Call this from dialog controller
    public static void SetDialogActive(bool isActive)
    {
        IsDialogActive = isActive;
        Time.timeScale = isActive ? 0f : (IsGamePaused ? 0f : 1f);
    }
}