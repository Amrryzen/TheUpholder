using UnityEngine;
using UnityEngine.UI;

public class LobbyStoryLockChecker : MonoBehaviour
{
    public Button story2Button;

    void Start()
    {
        int unlocked = PlayerPrefs.GetInt("Mode1Completed", 0);
        story2Button.interactable = (unlocked == 1);
    }
}
