using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    // Define a delegate type for the damage taken event
    public delegate void DamageTakenEventHandler();
    // Define the event using the delegate type
    public event DamageTakenEventHandler DamageTakenEvent;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Ensure that the current health doesn't go below 0
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Invoke the damage taken event
            DamageTakenEvent?.Invoke();
        }
    }

    private void Die()
    {
        // Implement death behavior here, such as game over or respawn
        Debug.Log("Player has died!");
    }
}



