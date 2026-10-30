using System.Collections;
using UnityEngine;
using TMPro;

public class SaveGame : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI saveText;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 1f;

    private Coroutine messageCoroutine;

    public void SaveGameScene()
    {
        SaveLoadSystem.SaveLoadSystem.SaveNew();

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(ShowAndHide());
    }

    private IEnumerator ShowAndHide()
    {
        saveText.text = "Game Saved";
        saveText.gameObject.SetActive(true);

        // Works even when Time.timeScale = 0
        yield return new WaitForSecondsRealtime(displayDuration);

        saveText.gameObject.SetActive(false);
        messageCoroutine = null;
    }
}
