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

    private void Update()
    {
        // Find the player if the reference is missing.
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
                return;
        }

        if (banditDialogue == null)
            return;

        // Defeated bandits cannot move or attack.
        if (currentHealth <= 10)
            return;

        // Only attack when combat is enabled.
        if (banditDialogue.CanAttack())
        {
            MoveTowardsPlayer();

            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                AttackPlayer();
            }
        }
    }

    public void AttackPlayer()
    {
        if (player == null)
            return;

        if (Vector3.Distance(transform.position, player.transform.position) <= 2f)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.TakeDamage(5);
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        // Don't move if the player has been destroyed.
        if (player == null)
            return;

        Vector3 direction =
            (player.transform.position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        // Don't allow health to fall below 10.
        currentHealth = Mathf.Max(10, currentHealth - damage);

        // Flash red when attacked.
        FlashRed();

        // Stop attacking when defeated.
        if (currentHealth <= 10)
        {
            Debug.Log(gameObject.name + " has been defeated!");
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






