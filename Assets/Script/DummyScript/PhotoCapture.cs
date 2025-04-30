using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCapture : MonoBehaviour
{
    public float photoRange = 10f;
    public LayerMask photoLayer;
    public KeyCode photoKey = KeyCode.Space;
    public Camera mainCamera;

    public AudioClip shutterSound;
    private AudioSource audioSource;

    private FlashEffect flashEffect;
    private PhotoUI photoUI;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        flashEffect = FindObjectOfType<FlashEffect>();
        photoUI = FindObjectOfType<PhotoUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(photoKey))
        {
            TakePhoto();
        }
    }

    void TakePhoto()
    {
        // Play shutter sound
        if (shutterSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shutterSound);
        }

        // Play flash
        if (flashEffect != null)
        {
            flashEffect.PlayFlash();
        }

        // Show UI image
        if (photoUI != null)
        {
            photoUI.ShowPhoto();
        }

        // Raycast to detect photographed object
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, photoRange, photoLayer))
        {
            Debug.Log("Object photographed: " + hit.collider.name);
            hit.collider.SendMessage("OnPhotographed", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.Log("No object in view.");
        }
    }
}
