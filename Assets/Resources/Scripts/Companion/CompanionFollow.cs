using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    public string playerTag = "Player"; // Tag of the player GameObject
    public float followDistance = 2f; // Distance behind the player that the companion follows
    public float stopRadius = 0.5f; // Stop radius from the player
    public float stopThreshold = 0.01f; // Threshold for considering the companion as stopped

    private GameObject player; // Reference to the player GameObject
    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.right; // Default last move direction is right

    public float moveSpeed = 1f; // Move speed of the companion
    private float originalMoveSpeed; // Original move speed of the companion

    private void Start()
    {
        // Find the player GameObject tagged with the specified tag
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogError("Player GameObject not found. Make sure the player GameObject is tagged with the specified tag.");
        }

        animator = GetComponent<Animator>();

        // Store the original move speed
        originalMoveSpeed = moveSpeed;
    }

    private void Update()
    {
        // Make sure we have a valid player reference
        if (player != null)
        {
            // Calculate movement direction
            Vector2 movement = (player.transform.position - transform.position).normalized;

            // Update animator parameters based on movement direction
            UpdateAnimatorParameters(movement);

            // Calculate the target position for the companion to follow
            Vector3 targetPosition = player.transform.position - player.transform.forward * followDistance;

            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, targetPosition);

            // Move the companion towards the target position only if it's beyond the stop radius
            if (distanceToPlayer > stopRadius)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            }
        }
    }

    private void UpdateAnimatorParameters(Vector2 movement)
    {
        if (movement.magnitude > stopThreshold) // If moving
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y)) // Moving horizontally
            {
                if (movement.x > 0)
                {
                    lastMoveDirection = Vector2.right;
                    animator.Play("WalkRight");
                }
                else
                {
                    lastMoveDirection = Vector2.left;
                    animator.Play("WalkLeft");
                }
            }
            else // Moving vertically
            {
                if (movement.y > 0)
                {
                    lastMoveDirection = Vector2.up;
                    animator.Play("WalkBack");
                }
                else
                {
                    lastMoveDirection = Vector2.down;
                    animator.Play("WalkFront");
                }
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
            else if (lastMoveDirection.y > 0)
            {
                animator.Play("IdleBack");
            }
            else if (lastMoveDirection.y < 0)
            {
                animator.Play("IdleFront");
            }
        }
    }

    // Method to apply speed multiplier to the companion
    public void ApplySpeedMultiplier(float multiplier)
    {
        moveSpeed *= multiplier;
    }

    // Method to reset the companion's speed to its original value
    public void ResetSpeed()
    {
        moveSpeed = originalMoveSpeed;
    }
}






