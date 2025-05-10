using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Lift : MonoBehaviour
{
    [Header("Transition Settings")]
    public Image transitionImage;             // UI Image yang menutupi layar
    public float transitionDuration = 1f;     // Durasi transisi
    public GameObject uiPanel;                // Panel tombol pilihan scene

    private void Start()
    {
        if (uiPanel != null)
            uiPanel.SetActive(false);
        
        // Pastikan transition image awalnya transparan
        if (transitionImage != null)
        {
            Color c = transitionImage.color;
            c.a = 0f;
            transitionImage.color = c;
            transitionImage.transform.localScale = Vector3.one;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiPanel != null)
                uiPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiPanel != null)
                uiPanel.SetActive(false);
        }
    }

    public void LoadScene(string sceneName)
{
    if (uiPanel != null)
        uiPanel.SetActive(false); // Panel ditutup saat tombol diklik

    StartCoroutine(TransitionAndLoad(sceneName));
}


    private IEnumerator TransitionAndLoad(string sceneName)
{
    // Animasi masuk (fade in + scale up)
    float elapsed = 0f;
    while (elapsed < transitionDuration)
    {
        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / transitionDuration);

        Color c = transitionImage.color;
        c.a = Mathf.Lerp(0f, 1f, progress);
        transitionImage.color = c;

        transitionImage.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.2f, 1.2f, 1.2f), progress);

        yield return null;
    }

    // Pastikan alpha 1 dan scale akhir
    Color final = transitionImage.color;
    final.a = 1f;
    transitionImage.color = final;
    transitionImage.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

    // Load scene setelah transisi selesai
    SceneManager.LoadScene(sceneName);
}

}
