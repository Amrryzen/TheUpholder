using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    private bool isGamePaused = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(this.gameObject); // Jika ingin tetap ada di semua scene
    }
    public void SetPause(bool pause)
    {
        isGamePaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }
    public bool IsGamePaused()
{
    return isGamePaused;
}


}
