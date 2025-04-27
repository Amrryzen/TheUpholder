using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public GameObject flashlightObject; // Object senter yang mau dinyalakan/dimatikan
    private bool isOn = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;

        if (flashlightObject != null)
        {
            flashlightObject.SetActive(isOn);
        }
    }
}
