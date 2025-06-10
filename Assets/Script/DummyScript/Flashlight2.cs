using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight2 : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public GameObject flashlightObject;  // Child yang punya Light2D, arah default ke kanan (0°)
    private bool isOn = false;

    [Header("Offset Settings")]
    public float distanceFromPlayer = 0.06f; // jarak senter dari pusat player
    public Vector2 offset = Vector2.zero; // Tambahkan offset tambahan jika diperlukan

    private Vector2 lastDirection = Vector2.right;

    void Start()
    {
        if (flashlightObject != null)
            flashlightObject.SetActive(isOn);
    }

    void Update()
    {
        UpdateDirection();
    }

    public void Toggle(bool state)
    {
        isOn = state;
        if (flashlightObject != null)
            flashlightObject.SetActive(isOn);
    }

    public bool IsOn()
    {
        return isOn;
    }

    private void UpdateDirection()
    {
        // 1) Ambil input, jika ada simpan sebagai lastDirection
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (input != Vector2.zero)
            lastDirection = input;

        if (flashlightObject == null)
            return;

        // 2) Geser child flashlightObject dengan memperhitungkan offset
        Vector2 baseOffset = lastDirection * distanceFromPlayer;
        flashlightObject.transform.localPosition = (Vector3)(baseOffset + offset);

        // 3) Putar child agar beam menghadap arah yang benar
        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        flashlightObject.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}