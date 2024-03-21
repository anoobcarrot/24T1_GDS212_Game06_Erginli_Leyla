using System.Collections;
using UnityEngine;
using TMPro;

public class SaveGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI saveText;
    [SerializeField] private float displayDuration = 1f;

    public void SaveGameScene()
    {
        StartCoroutine(ShowAndHide());
    }

    private IEnumerator ShowAndHide()
    {
        saveText.text = "Game Saved";
        SaveLoadSystem.SaveLoadSystem.SaveNew();
        saveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        saveText.gameObject.SetActive(false);
    }
}

