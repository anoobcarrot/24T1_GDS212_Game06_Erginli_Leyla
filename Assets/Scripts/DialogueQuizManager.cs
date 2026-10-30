using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public enum QuizOptionType
{
    PlusOne,
    MinusOne,
    Yes,
    No
}

[System.Serializable]
public class Question
{
    public string questionText;
    public List<QuizOption> options;
}

[System.Serializable]
public class QuizOption
{
    public string optionText;
    public QuizOptionType optionType;
    public string traitName;
}

public class DialogueQuizManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public Image dialogueBoxImage;
    public GameObject optionButtonPrefab;
    public Transform optionButtonParent;

    public List<QuizOption> initialDialogueOptions = new List<QuizOption>();

    private bool hasTriggered = false;

    public List<Question> questions = new List<Question>();
    public List<FollowUpDialogue> followUpDialogues = new List<FollowUpDialogue>();

    private Question currentQuestion;
    private bool isDialogueShowing = false;

    private List<Question> originalQuestions = new List<Question>();

    // Store trait points earned during the quiz
    private Dictionary<string, int> traitPoints = new Dictionary<string, int>();

    private Coroutine currentTextAnimationCoroutine;

    void Start()
    {
        // Store the original questions when the game starts
        originalQuestions.AddRange(questions);
        isDialogueShowing = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            // Check if there is a Companion tagged game object already in the scene
            GameObject companionObject = GameObject.FindGameObjectWithTag("Companion");

            // If there is no Companion object in the scene, proceed
            if (companionObject == null)
            {
                // Display initial dialogue
                ShowInitialDialogue();
                hasTriggered = true;
            }
            else
            {
                Debug.Log("Companion already present in the scene.");
                // Optionally, you can add logic here to inform the player that a companion is already present
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Reset quiz state
            hasTriggered = false;
            ClearTraitPoints();
            ResetQuestions();

            // Stop any ongoing text animations
            StopAllCoroutines();

            // Disable dialogue box
            dialogueBoxImage.gameObject.SetActive(false);
            dialogueText.text = "";
        }
    }

    void ResetQuestions()
    {
        // Clear the existing questions list
        questions.Clear();
        // Add questions back to the list from the originalQuestions list
        questions.AddRange(originalQuestions);
        // Reset the dialogue showing flag
        isDialogueShowing = false;
    }

    void ClearTraitPoints()
    {
        // Clear trait points dictionary
        traitPoints.Clear();
    }

    private void ShowInitialDialogue()
    {
        // Show the initial dialogue with the specified options
        ShowDialogueWithOptions("Are you ready to take the companion quiz to find the right companion for you?", initialDialogueOptions);
    }

    private void ShowDialogueWithOptions(string dialogueText, List<QuizOption> options)
    {
        // Show the dialogue text
        dialogueBoxImage.gameObject.SetActive(true);
        currentTextAnimationCoroutine = StartCoroutine(AnimateText(dialogueText, true));

        // Display the options
        PresentQuestionOptions(options);
    }

    IEnumerator ShowRandomQuestionRoutine()
    {
        while (true)
        {
            if (!isDialogueShowing)
            {
                yield return new WaitForSeconds(30f);
                ShowRandomQuestion();
            }
            yield return null;
        }
    }

    void ShowRandomQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No questions available.");
            return;
        }

        isDialogueShowing = true;
        int randomIndex = Random.Range(0, questions.Count);
        currentQuestion = questions[randomIndex];
        questions.RemoveAt(randomIndex);
        ShowQuestion(currentQuestion, true);
    }

    void ShowQuestion(Question question, bool showOptions)
    {
        dialogueText.text = "";
        dialogueBoxImage.gameObject.SetActive(true);
        if (currentTextAnimationCoroutine != null)
            StopCoroutine(currentTextAnimationCoroutine);
        currentTextAnimationCoroutine = StartCoroutine(AnimateText(question.questionText, showOptions));
        if (showOptions)
        {
            PresentQuestionOptions(question.options);
        }
        // Remove the current question from the list
        questions.Remove(currentQuestion);
    }

    void ShowDialogue(string message)
    {
        // Display dialogue text
        dialogueText.text = "";
        dialogueBoxImage.gameObject.SetActive(true);
        if (currentTextAnimationCoroutine != null)
            StopCoroutine(currentTextAnimationCoroutine);
        currentTextAnimationCoroutine = StartCoroutine(AnimateText(message, false));
    }

    IEnumerator AnimateText(string text, bool showOptions)
    {
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        if (!showOptions)
        {
            // Hide dialogue after delay
            yield return new WaitForSeconds(3f); // Adjust delay as needed
            dialogueBoxImage.gameObject.SetActive(false);
        }
    }

    IEnumerator ShowDialogueAfterDelay(Question question, float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueBoxImage.gameObject.SetActive(true);
        dialogueText.text = question.questionText;
        yield return new WaitForSeconds(1f);
        dialogueBoxImage.gameObject.SetActive(false);
    }

    void PresentQuestionOptions(List<QuizOption> options)
    {
        // Clear previous options
        ClearDialogueOptions();

        float buttonHeight = optionButtonPrefab.GetComponent<RectTransform>().rect.height;
        float verticalSpacing = 10f;

        for (int i = 0; i < options.Count; i++)
        {
            GameObject buttonGO = Instantiate(optionButtonPrefab, optionButtonParent);
            Button button = buttonGO.GetComponent<Button>();
            int index = i;
            button.onClick.AddListener(() => PlayerChoice(options[index]));
            button.GetComponentInChildren<TextMeshProUGUI>().text = options[i].optionText;

            RectTransform buttonRect = button.GetComponent<RectTransform>();
            Vector2 buttonPosition = buttonRect.anchoredPosition;

            // Adjust button position based on vertical positioning number
            buttonPosition.y = -i * (buttonHeight + verticalSpacing);

            // Example: Apply an offset based on the vertical positioning number
            float verticalOffset = i * 15f; // Adjust this value as needed
            buttonPosition.y += verticalOffset;

            buttonRect.anchoredPosition = buttonPosition;
        }
    }


    public void PlayerChoice(QuizOption selectedOption)
    {
        if (selectedOption.optionType == QuizOptionType.PlusOne)
        {
            if (traitPoints.ContainsKey(selectedOption.traitName))
            {
                traitPoints[selectedOption.traitName]++;
            }
            else
            {
                traitPoints[selectedOption.traitName] = 1;
            }
        }
        else if (selectedOption.optionType == QuizOptionType.MinusOne)
        {
            if (traitPoints.ContainsKey(selectedOption.traitName))
            {
                traitPoints[selectedOption.traitName]--;
            }
            else
            {
                traitPoints[selectedOption.traitName] = -1;
            }
        }
        else if (selectedOption.optionType == QuizOptionType.Yes)
        {
            // Trigger start of the quiz
            StartQuiz();
            return; // Add this return statement to exit the method after starting the quiz
        }
        else if (selectedOption.optionType == QuizOptionType.No)
        {
            // Close the dialogue
            isDialogueShowing = false;
            ClearDialogueOptions();
            StartCoroutine(DisableDialogueBoxAfterDelay(1f));
            return; // Add this return statement to exit the method after closing the dialogue
        }

        // Move to the next question
        MoveToNextQuestion();
    }

    void MoveToNextQuestion()
    {
        // Check if there are more questions available
        if (questions.Count > 0)
        {
            // Display the next question
            ShowRandomQuestion();
        }
        else
        {
            // End of quiz
            EndQuiz();
            DisableCollider();
        }
    }

    void EndQuiz()
    {
        // Store the total trait points for each companion
        Dictionary<Companion, int> companionTraitPoints = new Dictionary<Companion, int>();

        // Iterate through each companion
        foreach (Companion companion in MoralityFriendshipManager.instance.companions)
        {
            int totalTraitPoints = 0;

            // Iterate through each personality trait of the companion
            foreach (PersonalityTrait trait in companion.personalityTraits)
            {
                // Check if the trait points dictionary contains the trait name
                if (traitPoints.ContainsKey(trait.name))
                {
                    // Add the trait points to the total for this companion
                    totalTraitPoints += traitPoints[trait.name];
                }
            }

            // Store the total trait points for this companion
            companionTraitPoints.Add(companion, totalTraitPoints);
        }

        // Find the companion with the highest total trait points
        Companion selectedCompanion = null;
        int maxTraitPoints = int.MinValue;
        foreach (var kvp in companionTraitPoints)
        {
            if (kvp.Value > maxTraitPoints)
            {
                selectedCompanion = kvp.Key;
                maxTraitPoints = kvp.Value;
            }
        }

        if (selectedCompanion != null)
        {
            // Spawn the selected companion's GameObject
            Instantiate(selectedCompanion.gameObject, Vector3.zero, Quaternion.identity);
            // Show dialogue
            ShowDialogue("You have received " + selectedCompanion.name + " as your companion");
        }
        else
        {
            Debug.LogError("No companion found with the highest trait points.");
        }

        // Clear the option buttons after the quiz ends
        ClearDialogueOptions();
    }


    void StartQuiz()
    {
        // Check if there are questions available
        if (questions.Count > 0)
        {
            // Display the first question
            ShowQuestionAtIndex(0);
        }
        else
        {
            Debug.LogError("No questions available for the quiz.");
        }
    }

    void ShowQuestionAtIndex(int index)
    {
        if (index < questions.Count)
        {
            // Display the question at the specified index
            currentQuestion = questions[index];
            ShowQuestion(currentQuestion, true);
        }
        else
        {
            Debug.LogError("Invalid question index.");
        }
    }

    IEnumerator DisableDialogueBoxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogueBoxImage.gameObject.SetActive(false);
        isDialogueShowing = false;

        // Calculate highest trait point value
        KeyValuePair<string, int> maxTraitPoints = new KeyValuePair<string, int>("", int.MinValue);
        foreach (var kvp in traitPoints)
        {
            if (kvp.Value > maxTraitPoints.Value)
            {
                maxTraitPoints = kvp;
            }
        }

        // Spawn companion with the highest trait point value
        SpawnCompanion(maxTraitPoints.Key);
        DisableCollider();
    }

    void SpawnCompanion(string traitName)
    {
        // Find companion with the highest trait point
        Companion companion = MoralityFriendshipManager.instance.companions.Find(comp => Array.Exists(comp.personalityTraits, t => t.name == traitName));
        if (companion != null)
        {
            // Spawn the companion's GameObject
            GameObject newObject = Instantiate(companion.gameObject, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Companion not found for trait: " + traitName);
        }
    }

    void DisableCollider()
    {
        // Find all GameObjects with the "Companion Quiz" tag
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Companion Quiz");

        // Loop through each GameObject found
        foreach (GameObject taggedObject in taggedObjects)
        {
            // Get the BoxCollider2D component attached to the GameObject
            BoxCollider2D collider = taggedObject.GetComponent<BoxCollider2D>();

            // Check if the collider exists
            if (collider != null)
            {
                // Disable the collider if found
                collider.enabled = false;
            }
            else
            {
                // Log a warning if the BoxCollider2D component is not found
                Debug.LogWarning("BoxCollider2D component not found on a GameObject with the 'Companion Quiz' tag.");
            }
        }
    }

    void ClearDialogueOptions()
    {
        foreach (Transform child in optionButtonParent)
        {
            Destroy(child.gameObject);
        }
    }
}






