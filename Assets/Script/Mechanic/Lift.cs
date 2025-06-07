using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Lift : MonoBehaviour
{
    [Header("Transition Settings")]
    public Image transitionImage;          // UI Image yang menutupi layar
    public float transitionDuration = 1f;  // Durasi transisi (dalam detik)
    public GameObject uiPanel;             // Panel tombol pilihan scene

    private void Start()
    {
        // Sembunyikan panel pilihan awalnya
        if (uiPanel != null)
            uiPanel.SetActive(false);

        // Pastikan transitionImage awalnya transparan dan tidak aktif
        if (transitionImage != null)
        {
            transitionImage.gameObject.SetActive(false);
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

    // Dipanggil oleh tombol di UI panel (misalnya via OnClick)
    public void LoadScene(string sceneName)
    {
        if (uiPanel != null)
            uiPanel.SetActive(false); // Tutup panel pilihan saat tombol diklik

        if (transitionImage != null)
        {
            transitionImage.gameObject.SetActive(true);
            StartCoroutine(TransitionAndLoad(sceneName));
        }
        else
        {
            // Jika transitionImage belum di-assign, load langsung tanpa transisi
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator TransitionAndLoad(string sceneName)
    {
        // 1) Fade‐in (alpha 0 → 1) sekaligus scale (1 → 1.2)
        float elapsed = 0f;
        Color c = transitionImage.color;
        Vector3 initialScale = Vector3.one;
        Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / transitionDuration);

            // Lerp alpha
            c.a = Mathf.Lerp(0f, 1f, progress);
            transitionImage.color = c;

            // Lerp scale
            transitionImage.transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);

            yield return null;
        }

        // Pastikan alpha = 1 dan scale = target
        c.a = 1f;
        transitionImage.color = c;
        transitionImage.transform.localScale = targetScale;

        // 2) Langsung load scene
        SceneManager.LoadScene(sceneName);
    }
}
