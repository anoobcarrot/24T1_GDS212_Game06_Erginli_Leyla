using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PersonalityTrait
{
    public string name;
    public int value;

    public PersonalityTrait(string _name, int _value)
    {
        name = _name;
        value = _value;
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

    // Method to add a new companion
    public void AddCompanion(string name, GameObject gameObject, PersonalityTrait[] personalityTraits, List<Dialogue> dialogues)
    {
        // Create a new companion object
        Companion newCompanion = new Companion(name, gameObject, personalityTraits, dialogues);

        // Add the companion to the list
        companions.Add(newCompanion);
    }

    // Method to modify morality for a specific companion
    public void ModifyMorality(string companionName, int amount)
    {
        Companion companion = companions.Find(comp => comp.name == companionName);

        if (companion != null)
        {
            int moralityChange = CalculateTraitValue(companion.personalityTraits) * amount;
            Debug.Log("Morality change for " + companionName + ": " + moralityChange);
        }
        else
        {
            Debug.LogError("Companion not found: " + companionName);
        }
    }

    // Method to modify friendship for a specific companion
    public void ModifyFriendship(string companionName, int amount)
    {
        Companion companion = companions.Find(comp => comp.name == companionName);

        if (companion != null)
        {
            int friendshipChange = CalculateTraitValue(companion.personalityTraits) * amount;
            Debug.Log("Friendship change for " + companionName + ": " + friendshipChange);
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




