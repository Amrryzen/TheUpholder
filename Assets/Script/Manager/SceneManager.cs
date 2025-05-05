using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagement : MonoBehaviour
{
    [Header("Transition Settings")]
    public Image transitionImage;           // UI Image yang menutupi layar (pastikan full screen)
    public float transitionDuration = 1f;     // Durasi animasi transisi
    [SerializeField] private string sceneToLoad = "DestinationScene"; // Nama scene tujuan

    [Header("Objects to Keep Visible")]
    public GameObject[] objectsToKeepVisible; // Objek yang tidak akan diubah opacity-nya

    // Method untuk dipanggil guna pindah scene dengan transisi morph
    public void LoadScene()
    {
        // Ubah opacity semua root objek kecuali objek yang ditentukan menjadi 0
        SetOpacityForOtherObjects();
        StartCoroutine(Transition());
    }

    public void QuitGame()
{
    Debug.Log("Game is quitting...");
    Application.Quit();
}

    // Mengubah opacity semua root objek (selain objek yang ditentukan) menjadi 0
    void SetOpacityForOtherObjects()
    {
        GameObject[] roots = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in roots)
        {
            if (obj == this.gameObject || obj == transitionImage.gameObject || IsInKeepVisibleList(obj))
                continue;

            SetOpacityRecursive(obj, 0f);
        }
    }

    // Mengecek apakah obj termasuk dalam daftar objek yang harus tetap terlihat
    bool IsInKeepVisibleList(GameObject obj)
    {
        foreach (GameObject keep in objectsToKeepVisible)
        {
            if (keep == obj)
                return true;
        }
        return false;
    }

    // Fungsi rekursif untuk mengubah opacity pada objek dan semua komponen terkait di bawahnya
    void SetOpacityRecursive(GameObject obj, float opacity)
    {
        // Jika memiliki CanvasGroup, set alpha-nya
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = opacity;
        }

        // Ubah opacity untuk semua komponen UI (Graphic) yang ada pada objek dan anak-anaknya
        Graphic[] graphics = obj.GetComponentsInChildren<Graphic>(true);
        foreach (Graphic g in graphics)
        {
            Color c = g.color;
            c.a = opacity;
            g.color = c;
        }

        // Ubah opacity untuk SpriteRenderer (jika ada)
        SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            Color c = sr.color;
            c.a = opacity;
            sr.color = c;
        }
    }

    IEnumerator Transition()
    {
        // Animasi masuk: transitionImage dari scale 1, alpha 0 ke scale 1.2, alpha 1
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / transitionDuration;
            // Ubah alpha
            Color c = transitionImage.color;
            c.a = Mathf.Lerp(0f, 1f, progress);
            transitionImage.color = c;
            // Ubah scale
            transitionImage.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.2f, 1.2f, 1.2f), progress);
            yield return null;
        }
        // Pastikan kondisi akhir animasi masuk
        {
            Color c = transitionImage.color;
            c.a = 1f;
            transitionImage.color = c;
            transitionImage.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }

        // Load scene secara asinkron menggunakan sceneToLoad
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Animasi keluar: transitionImage dari scale 1.2, alpha 1 ke scale 1, alpha 0
        elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / transitionDuration;
            Color c = transitionImage.color;
            c.a = Mathf.Lerp(1f, 0f, progress);
            transitionImage.color = c;
            transitionImage.transform.localScale = Vector3.Lerp(new Vector3(1.2f, 1.2f, 1.2f), Vector3.one, progress);
            yield return null;
        }
        // Kondisi akhir animasi keluar
        {
            Color c = transitionImage.color;
            c.a = 0f;
            transitionImage.color = c;
            transitionImage.transform.localScale = Vector3.one;
        }
    }
}
