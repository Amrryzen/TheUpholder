using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public GameObject flashlightObject;    // Child dengan Light2D atau SpotLight

    private bool isOn = false;

    [Header("Flashlight Follow")]
    private Vector2 lastDirection = Vector2.right;
    private SpriteRenderer spriteRenderer;
    public Vector3 rightHandPosition = new Vector3(0.242f, -0.242f, 0);
    public Vector3 leftHandPosition  = new Vector3(-0.242f, -0.242f, 0);
    private bool isFacingRight = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (flashlightObject != null)
            flashlightObject.SetActive(isOn);
    }

    void Update()
    {
        UpdateFollowDirection();
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

    private void UpdateFollowDirection()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (input != Vector2.zero)
        {
            lastDirection = input;

            if (Mathf.Abs(input.x) > 0.1f)
            {
                bool wasFacing = isFacingRight;
                isFacingRight = input.x > 0;
                if (wasFacing != isFacingRight)
                    transform.localPosition = isFacingRight
                        ? rightHandPosition
                        : leftHandPosition;
            }

            if (spriteRenderer != null)
                spriteRenderer.flipY = !isFacingRight;
        }

        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}


