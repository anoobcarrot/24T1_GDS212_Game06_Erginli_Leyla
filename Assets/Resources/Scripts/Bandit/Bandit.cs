using UnityEngine;

public class Bandit : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    private Renderer renderer;
    private Color originalColor;

    private GameObject player; // Reference to the player object
    [SerializeField] private float moveSpeed = 1f; // Adjust the speed at which the bandit moves towards the player

    private BanditDialogue banditDialogue;

    // Timer variables
    private float attackTimer = 0f;
    private float attackInterval = 1f; // 1 second interval

    void Start()
    {
        banditDialogue = FindObjectOfType<BanditDialogue>();
        currentHealth = maxHealth;
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;

        // Find the player object at the start
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in the scene!");
        }
    }

    void Update()
    {
        // Update the attack timer
        attackTimer += Time.deltaTime;

        if (banditDialogue != null && banditDialogue.CanAttack())
        {
            // Move towards the player
            MoveTowardsPlayer();

            // Check if the attack interval has elapsed
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f; // Reset the timer
                AttackPlayer();
            }
        }

        // Check bandit's health
        if (currentHealth <= 10)
        {
            banditDialogue.SetCanAttack(false);
        }
    }

    public void AttackPlayer()
    {
        // Check if the player is within attack range
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= 2f)
        {
            Debug.Log("Attack Player!");
            // Apply damage to the player
            player.GetComponent<PlayerHealth>().TakeDamage(5);

            // Move towards the player
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        // Calculate direction to the player
        Vector3 direction = (player.transform.position - transform.position).normalized;

        // Move towards the player
        transform.position += direction * moveSpeed * Time.deltaTime;
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






