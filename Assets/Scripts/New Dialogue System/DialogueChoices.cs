using UnityEngine;

[CreateAssetMenu(
    fileName = "New Dialogue Choices",
    menuName = "Celestria/Dialogue/Choices"
)]
public class DialogueChoices : ScriptableObject
{
    public CelestriaDialogueOption[] options;
}

[System.Serializable]
public class CelestriaDialogueOption
{
    public string optionText;

    public DialogueData nextDialogue;
}