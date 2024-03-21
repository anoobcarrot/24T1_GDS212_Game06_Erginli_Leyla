using System.Collections;
using UnityEngine;
using SaveLoadSystem;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void LoadGameScene()
    {
        // Load the scene
        StartCoroutine(LoadSceneThenLoadSave());
    }

    IEnumerator LoadSceneThenLoadSave()
    {
        yield return new WaitForSeconds(1f);
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Wait for one second before loading the save
        yield return new WaitForSeconds(1f);
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // If the player object exists, destroy it
        if (player != null)
        {
            Destroy(player);
        }
        yield return new WaitForSeconds(0.0001f);
        // Load the save
        SaveLoadSystem.SaveLoadSystem.Load();
    }
}
