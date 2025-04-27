using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector3 moveInput;

    void Update()
    {
        // Ambil input player
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        transform.position += moveInput * moveSpeed * Time.deltaTime;

        // Kirim parameter ke Animator
        if (moveInput != Vector3.zero)
        {
            animator.SetBool("WalkRight", true);
        }
        else animator.SetBool("WalkRight", false);

        if (moveInput == Vector3.left){
            transform.rotation = Quaternion.Euler(0, 180, 0);
        } else if (moveInput == Vector3.right){
        transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

}

