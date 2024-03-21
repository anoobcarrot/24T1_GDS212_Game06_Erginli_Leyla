using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    public GameObject playerPrefab; // Reference to the player prefab

    private GameObject playerInstance; // Reference to the instantiated player object
    private GameObject companionInstance; // Reference to the instantiated companion object

    private Vector3 playerSpawnPosition;
    private Vector3 companionSpawnPosition;

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializePlayerAndCompanion(Vector3 playerPosition, Vector3 companionPosition)
    {
        playerSpawnPosition = playerPosition;
        companionSpawnPosition = companionPosition;

        if (playerInstance == null)
        {
            playerInstance = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        }

        // Find the companion object with the "Companion" tag
        GameObject companion = GameObject.FindGameObjectWithTag("Companion");
        if (companion != null)
        {
            companionInstance = Instantiate(companion, companionSpawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Companion object with tag 'Companion' not found.");
        }
    }

    // Method to respawn player and companion at specified positions
    public void RespawnPlayerAndCompanion(Vector3 playerPosition, Vector3 companionPosition)
    {
        playerSpawnPosition = playerPosition;
        companionSpawnPosition = companionPosition;

        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }
        if (companionInstance != null)
        {
            Destroy(companionInstance);
        }

        playerInstance = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);

        // Find the companion object with the "Companion" tag
        GameObject companion = GameObject.FindGameObjectWithTag("Companion");
        if (companion != null)
        {
            companionInstance = Instantiate(companion, companionSpawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Companion object with tag 'Companion' not found.");
        }
    }

    // Getters for spawn positions
    public Vector3 GetPlayerSpawnPosition()
    {
        return playerSpawnPosition;
    }

    public Vector3 GetCompanionSpawnPosition()
    {
        return companionSpawnPosition;
    }
}


