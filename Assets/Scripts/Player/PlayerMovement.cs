using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Combat")]
    [SerializeField] private float attackRadius = 2f;

    private Rigidbody2D rb;
    private BanditDialogue banditDialogue;
    private Animator animator;

    private Vector2 movement;
    private Vector2 lastMoveDirection = Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        banditDialogue = FindObjectOfType<BanditDialogue>();
    }

    private void Update()
    {
        // Get immediate input without smoothing.
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        movement = new Vector2(horizontalInput, verticalInput).normalized;

        // Update character animations.
        UpdateAnimatorParameters(movement);

        // Check for bandit attacks.
        if (banditDialogue != null && banditDialogue.CanAttack())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttackBandit();
            }
        }
    }

    private void FixedUpdate()
    {
        // Apply movement during physics updates.
        rb.velocity = movement * moveSpeed;
    }

    private void UpdateAnimatorParameters(Vector2 movement)
    {
        if (movement.sqrMagnitude > 0)
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
            else if (movement.y < 0)
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
        else
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
            Collider2D[] colliders =
                Physics2D.OverlapCircleAll(transform.position, attackRadius);

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Bandit"))
                {
                    Bandit bandit = collider.GetComponent<Bandit>();

                    if (bandit != null)
                    {
                        if (bandit.currentHealth > 10)
                        {
                            bandit.TakeDamage(10);

                            Debug.Log(
                                "Attacked Bandit: " + bandit.gameObject.name
                            );
                        }
                        else
                        {
                            Debug.Log(
                                "Bandit's health is too low to attack: " +
                                collider.gameObject.name
                            );
                        }
                    }
                    else
                    {
                        Debug.LogWarning(
                            "Bandit component not found on object: " +
                            collider.gameObject.name
                        );
                    }
                }
            }
        }
    }
}




