using UnityEngine;
using SaveLoadSystem;
using System;

public class NPCController : MonoBehaviour, ISaveable
{
    [Header("Interaction")]
    public float interactRadius = 1.5f;

    [Header("Dialogue")]
    public DialogueData initialDialogue;
    public DialogueData[] followUpDialogues;

    [Header("NPC Indicator")]
    public GameObject exclamationMark;

    [Header("Optional")]
    public GameObject activeObject;

    [Header("Companion")]
    public bool skipInitialDialogueIfCompanionExists;

    private bool hasTalked;
    private bool isPlayerNearby;
    private bool initialDialogueStarted;

    private Transform player;

    // Data saved by your existing SaveLoadSystem.
    [Serializable]
    private class NPCSaveData
    {
        public bool hasTalked;
        public bool activeObjectState;
    }

    private void Start()
    {
        FindPlayer();
        UpdateIndicator();
    }

    private void Update()
    {
        // Find the player again if they were destroyed
        // and recreated by the load system.
        if (player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        isPlayerNearby = distance <= interactRadius;

        // Skip initial dialogue if a companion already exists.
        if (skipInitialDialogueIfCompanionExists &&
            !hasTalked &&
            GameObject.FindGameObjectWithTag("Companion") != null)
        {
            CompleteInitialDialogue();
        }

        if (!isPlayerNearby)
        {
            if (DialogueManager.Instance != null &&
                DialogueManager.Instance.IsDialogueActive &&
                initialDialogueStarted)
            {
                DialogueManager.Instance.CancelDialogue();
                initialDialogueStarted = false;
            }

            return;
        }

        if (DialogueManager.Instance == null)
            return;

        if (Input.GetKeyDown(DialogueManager.Instance.interactKey))
        {
            if (DialogueManager.Instance.IsDialogueActive)
                return;

            Interact();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void Interact()
    {
        DialogueData dialogueToPlay = null;

        if (!hasTalked)
        {
            dialogueToPlay = initialDialogue;
        }
        else if (followUpDialogues != null &&
                 followUpDialogues.Length > 0)
        {
            dialogueToPlay = followUpDialogues[
                UnityEngine.Random.Range(
                    0,
                    followUpDialogues.Length
                )
            ];
        }

        if (dialogueToPlay == null)
            return;

        initialDialogueStarted = true;

        DialogueManager.Instance.StartDialogue(
            dialogueToPlay,
            this
        );
    }

    public void OnDialogueFinished()
    {
        initialDialogueStarted = false;

        if (!hasTalked)
        {
            CompleteInitialDialogue();
        }
    }

    private void CompleteInitialDialogue()
    {
        hasTalked = true;

        UpdateIndicator();
        ToggleActiveObject();

        Debug.Log(
            "[NPCController] Initial dialogue completed: " +
            gameObject.name
        );
    }

    private void UpdateIndicator()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(!hasTalked);
        }
    }

    private void ToggleActiveObject()
    {
        if (activeObject != null)
        {
            activeObject.SetActive(!activeObject.activeSelf);
        }
    }

    // =====================================
    // SAVE SYSTEM
    // =====================================

    public bool NeedsToBeSaved()
    {
        return true;
    }

    public bool NeedsReinstantiation()
    {
        return false;
    }

    public object SaveState()
    {
        NPCSaveData data = new NPCSaveData
        {
            hasTalked = hasTalked,

            activeObjectState =
                activeObject != null &&
                activeObject.activeSelf
        };

        Debug.Log(
            "[NPCController] Saving " +
            gameObject.name +
            " | Has Talked: " +
            data.hasTalked
        );

        return data;
    }

    public void LoadState(object state)
    {
        NPCSaveData data = state as NPCSaveData;

        if (data == null)
        {
            Debug.LogWarning(
                "[NPCController] Invalid save data for " +
                gameObject.name
            );

            return;
        }

        // Restore dialogue progress.
        hasTalked = data.hasTalked;

        // Reset temporary interaction state.
        initialDialogueStarted = false;
        isPlayerNearby = false;

        // Restore the indicator.
        UpdateIndicator();

        // Restore the optional object's state.
        if (activeObject != null)
        {
            activeObject.SetActive(
                data.activeObjectState
            );
        }

        // Refresh player reference after loading.
        player = null;

        Debug.Log(
            "[NPCController] Loaded " +
            gameObject.name +
            " | Has Talked: " +
            hasTalked
        );
    }

    public void PostInstantiation(object state)
    {
        // Refresh UI after all saved objects are restored.
        UpdateIndicator();
    }

    public void GotAddedAsChild(
        GameObject obj,
        GameObject hisParent
    )
    {
        // Not required for NPC dialogue progress.
    }

    // =====================================
    // GIZMOS
    // =====================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            interactRadius
        );
    }
}