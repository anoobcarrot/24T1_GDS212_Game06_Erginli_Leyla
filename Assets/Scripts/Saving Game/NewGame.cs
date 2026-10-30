using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGame : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        // Clear all PlayerPrefs data
        PlayerPrefs.DeleteAll();
    }
}
