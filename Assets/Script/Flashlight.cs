using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public GameObject flashlightObject; // Object senter yang mau dinyalakan/dimatikan
    private bool isOn = false;

    [Header("Flashlight Follow")]
    private Vector2 lastDirection = Vector2.right;
    private SpriteRenderer spriteRenderer; // Optional for visual flipping
    public Vector3 rightHandPosition = new Vector3(0.242f, -0.242f, 0);
    public Vector3 leftHandPosition = new Vector3(-0.242f, -0.242f, 0);
    private bool isFacingRight;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleFlashlight();
        }

        SameDirectionFlash();
        
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;

        if (flashlightObject != null)
        {
            flashlightObject.SetActive(isOn);
        }
    }

    void SameDirectionFlash()
    {
        // Get input and only update direction if there is input
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (input != Vector2.zero)
        {
            lastDirection = input;

            // Only check for direction change if moving horizontally
            if (Mathf.Abs(input.x) > 0.1f)
            {
                bool wasFacingRight = isFacingRight;
                isFacingRight = input.x > 0;

                // Only update position if direction actually changed
                if (wasFacingRight != isFacingRight)
                {
                    transform.localPosition = isFacingRight ? rightHandPosition : leftHandPosition;
                }
            }

            // Optional sprite flipping (alternative to rotation)
            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = !isFacingRight;
            }
        }

        // Calculate and apply rotation
        float targetAngle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

}
