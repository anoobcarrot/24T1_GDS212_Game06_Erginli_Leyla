using UnityEngine;

[CreateAssetMenu(
    fileName = "New Dialogue",
    menuName = "Celestria/Dialogue/Conversation"
)]
public class DialogueData : ScriptableObject
{
    public DialogueLine[] lines;
}

[System.Serializable]
public class DialogueLine
{
    public string speakerName;

    [TextArea(3, 6)]
    public string text;

    public DialogueChoices choices;
}