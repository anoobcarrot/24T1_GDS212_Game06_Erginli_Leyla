using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float speed = 5f; // Speed of the magic projectile
    public int damage = 20; // Damage amount of the magic projectile

    private Vector3 targetPosition; // Target position for the magic projectile to move towards

    // Update is called once per frame
    void Update()
    {
        // Move the magic projectile towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the magic projectile has reached the target position
        if (transform.position == targetPosition)
        {
            // Destroy the magic projectile if it reaches the target position without hitting anything
            Destroy(gameObject);
        }
    }

    // Method to set the target position for the magic projectile
    public void SetTargetPosition(Vector3 target)
    {
        targetPosition = target;
    }

    // Method to handle collision with other objects
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the magic projectile collides with a bandit
        if (other.CompareTag("Bandit"))
        {
            // Deal damage to the bandit
            Bandit banditScript = other.GetComponent<Bandit>();
            if (banditScript != null)
            {
                banditScript.TakeDamage(damage);
            }

            // Destroy the magic projectile upon collision with the bandit
            Destroy(gameObject);
        }
    }
}

