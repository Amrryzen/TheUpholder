using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumManager : MonoBehaviour
{
    public static AlbumManager Instance;

    [Header("UI References")]
    public GameObject albumPanel;
    public Transform photoGridContent;
    public GameObject photoItemPrefab; // prefab with RawImage

    private List<Texture2D> savedPhotos = new List<Texture2D>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (albumPanel != null)
            albumPanel.SetActive(false);
    }

    public void AddPhoto(Texture2D tex)
{
    Debug.Log("AddPhoto() called");

    GameObject photoGO = Instantiate(photoItemPrefab, photoGridContent);
    RawImage img = photoGO.GetComponent<RawImage>();
    img.texture = tex;
}


    public void ToggleAlbum()
    {
        if (albumPanel != null)
            albumPanel.SetActive(!albumPanel.activeSelf);
    }
}
