using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight2 : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public GameObject flashlightObject; // Child yang punya Light2D, arah default ke kanan (0°)
    private bool isOn = false;

    [Header("Offset Settings")]
    public float distanceFromPlayer = 0.06f; // Jarak senter dari pusat player
    public Vector2 offset = Vector2.zero;    // Tambahkan offset tambahan jika diperlukan
    private Vector2 lastDirection = Vector2.right;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip flashlightOnClip;
    public AudioClip flashlightOffClip;

    void Start()
    {
        if (flashlightObject != null)
            flashlightObject.SetActive(isOn);

        if (audioSource == null)
            Debug.LogWarning("⚠️ AudioSource belum di-assign!");
    }

    void Update()
    {
        UpdateDirection();
    }

    public void Toggle(bool state)
    {
        if (isOn == state) return; // biar gak play audio dua kali

        isOn = state;

        if (flashlightObject != null)
            flashlightObject.SetActive(isOn);

        PlaySFX(state);
    }

    public bool IsOn()
    {
        return isOn;
    }

    private void UpdateDirection()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (input != Vector2.zero)
            lastDirection = input;

        if (flashlightObject == null) return;

        Vector2 baseOffset = lastDirection * distanceFromPlayer;
        flashlightObject.transform.localPosition = (Vector3)(baseOffset + offset);

        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        flashlightObject.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void PlaySFX(bool turnOn)
    {
        if (audioSource == null) return;

        AudioClip clipToPlay = turnOn ? flashlightOnClip : flashlightOffClip;
        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
        else
        {
            Debug.LogWarning("⚠️ AudioClip untuk " + (turnOn ? "nyala" : "mati") + " belum di-assign!");
        }
    }
}
