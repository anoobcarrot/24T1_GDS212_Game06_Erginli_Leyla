using UnityEngine;

public class NPCController : MonoBehaviour
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

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        UpdateIndicator();
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        isPlayerNearby = distance <= interactRadius;

        if (skipInitialDialogueIfCompanionExists &&
            !hasTalked &&
            GameObject.FindGameObjectWithTag("Companion") != null)
        {
            hasTalked = true;
            UpdateIndicator();
            ToggleActiveObject();
        }

        if (!isPlayerNearby)
        {
            if (DialogueManager.Instance.IsDialogueActive &&
                initialDialogueStarted)
            {
                DialogueManager.Instance.CancelDialogue();
                initialDialogueStarted = false;
            }

            return;
        }

        if (Input.GetKeyDown(DialogueManager.Instance.interactKey))
        {
            if (DialogueManager.Instance.IsDialogueActive)
                return;

            Interact();
        }
    }

    private void Interact()
    {
        DialogueData dialogueToPlay = null;

        if (!hasTalked)
        {
            dialogueToPlay = initialDialogue;
        }
        else if (followUpDialogues.Length > 0)
        {
            dialogueToPlay = followUpDialogues[
                Random.Range(0, followUpDialogues.Length)
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
            hasTalked = true;

            UpdateIndicator();
            ToggleActiveObject();
        }
    }

    private void UpdateIndicator()
    {
        if (exclamationMark != null)
            exclamationMark.SetActive(!hasTalked);
    }

    private void ToggleActiveObject()
    {
        if (activeObject != null)
            activeObject.SetActive(!activeObject.activeSelf);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            interactRadius
        );
    }
}