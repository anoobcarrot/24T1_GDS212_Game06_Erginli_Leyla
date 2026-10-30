using SaveLoadSystem;
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

public class MoralityFriendshipManager : MonoBehaviour, ISaveable
{
    public static MoralityFriendshipManager instance;

    public List<Companion> companions = new List<Companion>();

    [Serializable]
    private class CompanionStatsSaveData
    {
        public string name;
        public int morality;
        public int friendship;
    }

    [Serializable]
    private class MoralityFriendshipSaveData
    {
        public List<CompanionStatsSaveData> companions =
            new List<CompanionStatsSaveData>();
    }

    private Dictionary<string, CompanionStatsSaveData> pendingLoadedStats;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCompanion(
        string name,
        GameObject companionObject,
        PersonalityTrait[] personalityTraits,
        List<Dialogue> dialogues)
    {
        // Prevent duplicate companions after scene loading.
        Companion existing = companions.Find(c => c.name == name);

        if (existing != null)
        {
            existing.gameObject = companionObject;
            existing.personalityTraits = personalityTraits;
            existing.dialogues = dialogues;

            ApplyLoadedStats(existing);
            UpdateCompanionDisplay(existing);
            return;
        }

        Companion newCompanion = new Companion(
            name,
            companionObject,
            personalityTraits,
            dialogues
        );

        companions.Add(newCompanion);

        ApplyLoadedStats(newCompanion);
        UpdateCompanionDisplay(newCompanion);
    }

    public void ModifyMorality(
        string companionName,
        int amount,
        string traitName)
    {
        Companion companion = companions.Find(
            c => c.name == companionName
        );

        if (companion == null)
        {
            Debug.LogError("Companion not found: " + companionName);
            return;
        }

        PersonalityTrait trait = Array.Find(
            companion.personalityTraits,
            t => t.name == traitName
        );

        if (trait == null || !trait.affectsMorality)
        {
            Debug.LogError(
                "Trait not found or does not affect morality: " + traitName
            );
            return;
        }

        companion.morality = Mathf.Clamp(
            companion.morality + amount + trait.value,
            0,
            100
        );

        Debug.Log(
            companionName + " morality: " + companion.morality
        );

        UpdateCompanionDisplay(companion);
    }

    public void ModifyFriendship(
        string companionName,
        int amount,
        string traitName)
    {
        Companion companion = companions.Find(
            c => c.name == companionName
        );

        if (companion == null)
        {
            Debug.LogError("Companion not found: " + companionName);
            return;
        }

        PersonalityTrait trait = Array.Find(
            companion.personalityTraits,
            t => t.name == traitName
        );

        if (trait == null || !trait.affectsFriendship)
        {
            Debug.LogError(
                "Trait not found or does not affect friendship: " + traitName
            );
            return;
        }

        companion.friendship = Mathf.Clamp(
            companion.friendship + amount + trait.value,
            0,
            100
        );

        Debug.Log(
            companionName + " friendship: " + companion.friendship
        );

        UpdateCompanionDisplay(companion);
    }

    public (int, int) GetMoralityAndFriendship(string companionName)
    {
        Companion companion = companions.Find(
            c => c.name == companionName
        );

        if (companion != null)
        {
            return (companion.morality, companion.friendship);
        }

        Debug.LogError("Companion not found: " + companionName);
        return (0, 0);
    }

    private void UpdateCompanionDisplay(Companion companion)
    {
        if (companion.gameObject == null)
            return;

        CompanionDialogue dialogue =
            companion.gameObject.GetComponent<CompanionDialogue>();

        if (dialogue != null)
        {
            dialogue.UpdateMoralityFriendship(
                companion.morality,
                companion.friendship
            );
        }
    }

    private void ApplyLoadedStats(Companion companion)
    {
        if (pendingLoadedStats == null)
            return;

        if (pendingLoadedStats.TryGetValue(
            companion.name,
            out CompanionStatsSaveData stats))
        {
            companion.morality = stats.morality;
            companion.friendship = stats.friendship;

            UpdateCompanionDisplay(companion);
        }
    }

    // ======================================
    // SAVE SYSTEM
    // ======================================

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
        MoralityFriendshipSaveData data =
            new MoralityFriendshipSaveData();

        foreach (Companion companion in companions)
        {
            data.companions.Add(new CompanionStatsSaveData
            {
                name = companion.name,
                morality = companion.morality,
                friendship = companion.friendship
            });
        }

        Debug.Log("Morality and friendship saved!");

        return data;
    }

    public void LoadState(object state)
    {
        MoralityFriendshipSaveData data =
            state as MoralityFriendshipSaveData;

        if (data == null)
            return;

        pendingLoadedStats =
            new Dictionary<string, CompanionStatsSaveData>();

        foreach (CompanionStatsSaveData stats in data.companions)
        {
            pendingLoadedStats[stats.name] = stats;
        }

        foreach (Companion companion in companions)
        {
            ApplyLoadedStats(companion);
        }

        Debug.Log("Morality and friendship loaded!");
    }

    public void PostInstantiation(object state)
    {
        foreach (Companion companion in companions)
        {
            ApplyLoadedStats(companion);
        }
    }

    public void GotAddedAsChild(
        GameObject obj,
        GameObject hisParent)
    {
    }
}