using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersonalityTrait
{
    public string name;
    public int value;
    public bool affectsMorality;
    public bool affectsFriendship;

    public PersonalityTrait(string _name, int _value, bool _affectsMorality, bool _affectsFriendship)
    {
        name = _name;
        value = _value;
        affectsMorality = _affectsMorality;
        affectsFriendship = _affectsFriendship;
    }
}

[System.Serializable]
public class Companion
{
    public string name;
    public int morality; // Add morality attribute
    public int friendship; // Add friendship attribute
    public GameObject gameObject; // Reference to the companion's GameObject
    public PersonalityTrait[] personalityTraits;
    public List<Dialogue> dialogues; // List of dialogues available for the companion

    public Companion(string _name, GameObject _gameObject, PersonalityTrait[] _personalityTraits, List<Dialogue> _dialogues)
    {
        name = _name;
        gameObject = _gameObject;
        personalityTraits = _personalityTraits;
        dialogues = _dialogues;
    }
}

public class MoralityFriendshipManager : MonoBehaviour
{
    public static MoralityFriendshipManager instance;

    // List to store companions
    public List<Companion> companions = new List<Companion>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Loop through companions and log their morality and friendship values
        foreach (Companion companion in companions)
        {
            // Debug.Log("Current morality value for " + companion.name + ": " + companion.morality);
            // Debug.Log("Current friendship value for " + companion.name + ": " + companion.friendship);
        }
    }

    // Method to add a new companion
    public void AddCompanion(string name, GameObject gameObject, PersonalityTrait[] personalityTraits, List<Dialogue> dialogues)
    {
        // Create a new companion object
        Companion newCompanion = new Companion(name, gameObject, personalityTraits, dialogues);

        // Add the companion to the list
        companions.Add(newCompanion);
    }

    // Method to modify morality for a specific companion with consideration of the trait name
    public void ModifyMorality(string companionName, int amount, string traitName)
    {
        Companion companion = companions.Find(comp => comp.name == companionName);

        if (companion != null)
        {
            PersonalityTrait trait = Array.Find(companion.personalityTraits, t => t.name == traitName);
            if (trait != null && trait.affectsMorality)
            {
                int traitValue = trait.value;
                companion.morality = Mathf.Clamp(companion.morality + amount + traitValue, 0, 100);
                Debug.Log("Morality change for " + companionName + " with trait " + traitName + ": " + amount);
                Debug.Log("New morality value for " + companionName + ": " + companion.morality);
            }
            else
            {
                Debug.LogError("Trait not found or does not affect morality: " + traitName);
            }
        }
        else
        {
            Debug.LogError("Companion not found: " + companionName);
        }
    }

    // Method to modify friendship for a specific companion with consideration of the trait name
    public void ModifyFriendship(string companionName, int amount, string traitName)
    {
        Companion companion = companions.Find(comp => comp.name == companionName);

        if (companion != null)
        {
            PersonalityTrait trait = Array.Find(companion.personalityTraits, t => t.name == traitName);
            if (trait != null && trait.affectsFriendship)
            {
                int traitValue = trait.value;
                companion.friendship = Mathf.Clamp(companion.friendship + amount + traitValue, 0, 100);
                Debug.Log("Friendship change for " + companionName + " with trait " + traitName + ": " + amount);
                Debug.Log("New friendship value for " + companionName + ": " + companion.friendship);
            }
            else
            {
                Debug.LogError("Trait not found or does not affect friendship: " + traitName);
            }
        }
        else
        {
            Debug.LogError("Companion not found: " + companionName);
        }
    }

    // Calculate the overall value of personality traits
    private int CalculateTraitValue(PersonalityTrait[] traits)
    {
        int totalValue = 0;
        foreach (PersonalityTrait trait in traits)
        {
            totalValue += trait.value;
        }
        return totalValue;
    }
}




