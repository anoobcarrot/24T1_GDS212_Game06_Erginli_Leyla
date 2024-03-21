using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Adjust this value to control the player's movement speed
    [SerializeField] private float attackRadius = 2f; // Radius within which the player can attack bandits

    private Rigidbody2D rb;
    private BanditDialogue banditDialogue;
    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.right; // Default last move direction is right

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        banditDialogue = FindObjectOfType<BanditDialogue>();
        animator = GetComponent<Animator>();
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

        // Update animator parameters based on movement direction
        UpdateAnimatorParameters(movement);

        // Check if the player presses spacebar to attack bandits
        if (banditDialogue != null && banditDialogue.CanAttack())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttackBandit();
            }
        }
    }

    private void UpdateAnimatorParameters(Vector2 movement)
    {
        if (movement.magnitude > 0) // If moving
        {
            if (movement.x > 0)
            {
                lastMoveDirection = Vector2.right;
                animator.Play("WalkRight");
            }
            else if (movement.x < 0)
            {
                lastMoveDirection = Vector2.left;
                animator.Play("WalkLeft");
            }
            else if (movement.y < 0) // Check if moving downward
            {
                lastMoveDirection = Vector2.down;
                animator.Play("WalkFront");
            }
            else if (movement.y > 0)
            {
                lastMoveDirection = Vector2.up;
                animator.Play("WalkBack");
            }
        }
        else // If not moving
        {
            if (lastMoveDirection.x > 0)
            {
                animator.Play("IdleRight");
            }
            else if (lastMoveDirection.x < 0)
            {
                animator.Play("IdleLeft");
            }
            else if (lastMoveDirection.y < 0)
            {
                animator.Play("IdleFront");
            }
            else if (lastMoveDirection.y > 0)
            {
                animator.Play("IdleBack");
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








