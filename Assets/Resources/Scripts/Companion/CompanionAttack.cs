using UnityEngine;

public class CompanionAttack : MonoBehaviour
{
    public bool hasChargeAbility = false; // Public variable to determine if the companion has the charge ability
    public bool hasMagicAbility = false; // Public variable to determine if the companion has the magic ability
    public float chargeSpeedMultiplier = 2f; // Speed multiplier during charge ability
    public GameObject magicProjectilePrefab; // Reference to the magic projectile prefab
    public int magicDamage = 20; // Damage amount for magic ability

    private GameObject[] bandits; // Array to store references to all bandit GameObjects
    private int normalDamage = 10; // Normal damage amount
    private int chargeDamage = 20; // Damage amount for charge ability

    private void Start()
    {
        // Find all GameObjects with the "Bandit" tag and store references to them
        bandits = GameObject.FindGameObjectsWithTag("Bandit");

        // Subscribe to the DamageTakenEvent
        FindObjectOfType<PlayerHealth>().DamageTakenEvent += OnDamageTaken;
    }

    private void OnDamageTaken()
    {
        // Attack bandits when the player takes damage
        AttackBandits();
    }

    public void AttackBandits()
    {
        // Check if there are bandits in the scene
        if (bandits != null && bandits.Length > 0)
        {
            // Iterate through all bandits and attack them
            foreach (GameObject bandit in bandits)
            {
                // Perform attack on the bandit
                Attack(bandit);
            }
        }
    }

    private void Attack(GameObject bandit)
    {
        // Check if the bandit has a health component
        Bandit banditScript = bandit.GetComponent<Bandit>();
        if (banditScript != null)
        {
            // Determine damage amount based on companion abilities
            int damageAmount = hasChargeAbility ? chargeDamage : (hasMagicAbility ? magicDamage : normalDamage);

            // Deal damage to the bandit
            banditScript.TakeDamage(damageAmount);

            // If charge ability is enabled, move companion towards the bandit with increased speed
            if (hasChargeAbility)
            {
                // Calculate direction to the bandit
                Vector3 direction = (bandit.transform.position - transform.position).normalized;

                // Move towards the bandit with increased speed
                GetComponent<Rigidbody2D>().velocity = direction * chargeSpeedMultiplier;
            }

            // If magic ability is enabled, throw magic projectile
            if (hasMagicAbility)
            {
                ThrowMagicProjectile(bandit.transform.position);
            }
        }
    }

    private void ThrowMagicProjectile(Vector3 targetPosition)
    {
        if (magicProjectilePrefab != null)
        {
            // Instantiate magic projectile
            GameObject magicProjectile = Instantiate(magicProjectilePrefab, transform.position, Quaternion.identity);

            // Set the target position for the magic projectile to aim towards
            MagicProjectile magicScript = magicProjectile.GetComponent<MagicProjectile>();
            if (magicScript != null)
            {
                magicScript.SetTargetPosition(targetPosition);
            }
        }
    }
}









