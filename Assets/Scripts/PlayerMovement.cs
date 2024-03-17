using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust this value to control the player's movement speed
    public float attackRadius = 2f; // Radius within which the player can attack bandits

    private Rigidbody2D rb;
    private BanditDialogue banditDialogue;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        banditDialogue = FindObjectOfType<BanditDialogue>();
    }

    private void Update()
    {
        banditDialogue = FindObjectOfType<BanditDialogue>();
        // Get input from the player
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;

        // Move the player
        rb.velocity = movement * moveSpeed;

        // Check if the player presses spacebar to attack bandits
        if (banditDialogue != null && banditDialogue.CanAttack())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttackBandit();
            }
        }
    }

    public void AttackBandit()
    {
        if (banditDialogue != null && banditDialogue.CanAttack())
        {
            // Get all colliders within the attack radius
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRadius);

            // Loop through all colliders and attack bandits
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Bandit"))
                {
                    Bandit bandit = collider.GetComponent<Bandit>();
                    if (bandit != null)
                    {
                        // Check bandit's health before attacking
                        if (bandit.currentHealth > 10)
                        {
                            bandit.TakeDamage(10);
                            Debug.Log("Attacked Bandit: " + bandit.gameObject.name);
                        }
                        else
                        {
                            Debug.Log("Bandit's health is too low to attack: " + bandit.gameObject.name);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Bandit component not found on object: " + collider.gameObject.name);
                    }
                }
            }
        }
    }
}






