using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    
    [Header("References")]
    public Animator animator;
    public PlayerController playerController; // Untuk disable gerak saat mati

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Cek kalau lupa assign
        if (animator == null)
            animator = GetComponent<Animator>();

        if (playerController == null)
            playerController = GetComponent<PlayerController>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Jangan bisa kena damage lagi setelah mati

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Biar ga minus

        if (currentHealth == 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Disable movement
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // (Opsional) Habis animasi mati, bisa Destroy(gameObject) atau respawn
        // Destroy(gameObject, 2f); // Misal auto hapus setelah 2 detik
    }
}
