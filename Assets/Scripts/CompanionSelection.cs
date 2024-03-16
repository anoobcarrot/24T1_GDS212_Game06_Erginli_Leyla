using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CompanionSelection : MonoBehaviour
{
    public GameObject[] companionPrefabs;
    public GameObject[] companionGameObjects;
    public GameObject confirmationPopup; // Reference to the confirmation popup panel in the scene
    public TextMeshProUGUI confirmationText; // Reference to the text component in the confirmation popup
    public Button okButtonPrefab;
    public Transform buttonParent; // Parent object to hold the confirmation button
    private bool isConfirmed = false;
    public float verticalSpacing = 10f;

    private void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0) && !isConfirmed)
        {
            // Check if the confirmation popup is not active
            if (!confirmationPopup.activeSelf)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null)
                {
                    GameObject clickedObject = hit.collider.gameObject;

                    // Check if the clicked object is one of the companion game objects
                    int index = System.Array.IndexOf(companionGameObjects, clickedObject);
                    if (index != -1)
                    {
                        ShowConfirmationPopup(clickedObject);
                    }
                }
            }
        }
    }

    private void ShowConfirmationPopup(GameObject clickedObject)
    {
        // Activate the confirmation popup panel
        confirmationPopup.SetActive(true);

        // Start animating the text
        StartCoroutine(AnimateText("Are you sure you want to select this companion?", confirmationText));

        // Instantiate the confirmation buttons
        Button okButton = Instantiate(okButtonPrefab, buttonParent);
        Button cancelButton = Instantiate(okButtonPrefab, buttonParent); // Instantiate the "No" button
        cancelButton.GetComponentInChildren<TextMeshProUGUI>().text = "No"; // Change the text of the "No" button
        okButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ok"; // Change the text of the "Ok" button

        // Calculate the position for the "Ok" button
        Vector3 okButtonPosition = buttonParent.position;
        okButton.transform.position = okButtonPosition;

        // Calculate the position for the "No" button with vertical spacing
        Vector3 cancelButtonPosition = okButtonPosition - new Vector3(0, verticalSpacing, 0);
        cancelButton.transform.position = cancelButtonPosition;

        okButton.onClick.AddListener(() => ConfirmSelection(clickedObject));
        cancelButton.onClick.AddListener(() => CancelSelection());

        // Disable other game objects while the confirmation pop-up is active
        DisableOtherGameObjects(clickedObject);
    }

    private void CancelSelection()
    {
        // Deactivate the confirmation popup panel
        confirmationPopup.SetActive(false);
        isConfirmed = false;

        // Enable colliders of other game objects
        foreach (GameObject obj in companionGameObjects)
        {
            obj.GetComponent<Collider2D>().enabled = true;
        }
    }

    private void ConfirmSelection(GameObject selectedObject)
    {
        isConfirmed = true;

        int index = System.Array.IndexOf(companionGameObjects, selectedObject);
        if (index != -1 && index < companionPrefabs.Length)
        {
            GameObject selectedPrefab = companionPrefabs[index];
            if (selectedPrefab != null)
            {
                Vector3 spawnPosition = selectedObject.transform.position + new Vector3(1, 0, 0);
                Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

                // Destroy the selected game object
                Destroy(selectedObject);
            }
            else
            {
                Debug.LogWarning("Selected companion prefab is not assigned.");
            }
        }
        else
        {
            Debug.LogWarning("Game object not found in the companion prefabs array.");
        }

        // Deactivate the confirmation popup panel
        confirmationPopup.SetActive(false);
    }

    private void DisableOtherGameObjects(GameObject selectedObject)
    {
        foreach (GameObject obj in companionGameObjects)
        {
            if (obj != selectedObject)
            {
                obj.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    // Coroutine for animating text
    IEnumerator AnimateText(string text, TextMeshProUGUI textMeshProUGUI)
    {
        textMeshProUGUI.text = "";
        foreach (char c in text)
        {
            textMeshProUGUI.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }
}






