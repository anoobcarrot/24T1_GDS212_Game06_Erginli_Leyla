using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    public string playerTag = "Player"; // Tag of the player GameObject
    public float followDistance = 2f; // Distance behind the player that the companion follows

    private GameObject player; // Reference to the player GameObject

    private void Start()
    {
        // Find the player GameObject tagged with the specified tag
        player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogError("Player GameObject not found. Make sure the player GameObject is tagged with the specified tag.");
        }
    }

    private void Update()
    {
        // Make sure we have a valid player reference
        if (player != null)
        {
            // Calculate the target position for the companion to follow
            Vector3 targetPosition = player.transform.position - player.transform.forward * followDistance;

            // Move the companion towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }
    }
}

