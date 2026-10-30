using System.Collections;
using UnityEngine;
using SaveLoadSystem;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    private bool isLoading = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("[LoadGame] LoadGame object initialized.");
    }

    public void LoadGameScene()
    {
        if (isLoading)
            return;

        StartCoroutine(LoadSceneThenLoadSave());
    }

    private IEnumerator LoadSceneThenLoadSave()
    {
        isLoading = true;

        Debug.Log("[LoadGame] Starting game load...");

        yield return new WaitForSeconds(1f);

        // Load the Game scene.
        Debug.Log("[LoadGame] Loading Game scene...");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(
            "Game",
            LoadSceneMode.Single
        );

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("[LoadGame] Game scene fully loaded!");

        // Disable all music controllers.
        SetMusicEnabled(false);

        // Disable all AudioSources in the Game scene.
        SetAudioSourcesEnabled(false);

        yield return new WaitForSeconds(1f);

        // Find the original player.
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Debug.Log(
                "[LoadGame] Original player found at: " +
                player.transform.position
            );

            // Destroy the original player.
            Destroy(player);

            Debug.Log("[LoadGame] Original player marked for destruction.");
        }
        else
        {
            Debug.LogWarning("[LoadGame] Original player not found!");
        }

        // Wait for the original player to be destroyed.
        yield return null;

        Debug.Log("[LoadGame] Original player destruction completed.");

        // Load saved game.
        Debug.Log("[LoadGame] Loading saved game...");

        SaveLoadSystem.SaveLoadSystem.Load();

        Debug.Log("[LoadGame] SaveLoadSystem.Load() returned.");

        // Wait for the saved player to be positioned.
        yield return new WaitForFixedUpdate();

        GameObject savedPlayer = GameObject.FindGameObjectWithTag("Player");

        if (savedPlayer != null)
        {
            Debug.Log(
                "[LoadGame] Saved player found at position: " +
                savedPlayer.transform.position
            );
        }
        else
        {
            Debug.LogWarning("[LoadGame] Saved player not found!");
        }

        // Re-enable AudioSources.
        Debug.Log("[LoadGame] Enabling all AudioSources...");
        SetAudioSourcesEnabled(true);

        // Re-enable music controllers.
        Debug.Log("[LoadGame] Enabling music controllers...");
        SetMusicEnabled(true);

        Debug.Log("[LoadGame] Loading complete! Audio is enabled.");

        isLoading = false;
    }

    private void SetMusicEnabled(bool enable)
    {
        TownMusicController[] townControllers =
            FindObjectsOfType<TownMusicController>(true);

        ForestMusicController[] forestControllers =
            FindObjectsOfType<ForestMusicController>(true);

        InsideMusicController[] insideControllers =
            FindObjectsOfType<InsideMusicController>(true);

        Debug.Log(
            "[LoadGame] Found music controllers: " +
            townControllers.Length + " Town, " +
            forestControllers.Length + " Forest, " +
            insideControllers.Length + " Inside."
        );

        foreach (TownMusicController controller in townControllers)
        {
            controller.enabled = enable;

            if (!enable && controller.audioSource != null)
                controller.audioSource.Stop();

            Debug.Log(
                "[LoadGame] Town music controller " +
                (enable ? "ENABLED" : "DISABLED")
            );
        }

        foreach (ForestMusicController controller in forestControllers)
        {
            controller.enabled = enable;

            if (!enable && controller.audioSource != null)
                controller.audioSource.Stop();

            Debug.Log(
                "[LoadGame] Forest music controller " +
                (enable ? "ENABLED" : "DISABLED")
            );
        }

        foreach (InsideMusicController controller in insideControllers)
        {
            controller.enabled = enable;

            if (!enable && controller.audioSource != null)
                controller.audioSource.Stop();

            Debug.Log(
                "[LoadGame] Inside music controller " +
                (enable ? "ENABLED" : "DISABLED")
            );
        }
    }

    private void SetAudioSourcesEnabled(bool enable)
    {
        AudioSource[] audioSources =
            FindObjectsOfType<AudioSource>(true);

        Debug.Log(
            "[LoadGame] Found " +
            audioSources.Length +
            " AudioSources."
        );

        foreach (AudioSource source in audioSources)
        {
            // Only affect AudioSources belonging to the Game scene.
            if (source.gameObject.scene.name != "Game")
                continue;

            if (!enable)
            {
                source.Stop();
            }

            source.enabled = enable;

            Debug.Log(
                "[LoadGame] AudioSource '" +
                source.gameObject.name +
                "' " +
                (enable ? "ENABLED" : "DISABLED")
            );
        }
    }
}