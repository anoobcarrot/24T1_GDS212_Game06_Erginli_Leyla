using UnityEngine;

public class Bandit : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private Renderer renderer;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            FlashRed();
        }
    }

    void FlashRed()
    {
        renderer.material.color = Color.red;
        Invoke("ResetColor", 0.5f);
    }

    void ResetColor()
    {
        renderer.material.color = originalColor;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}

