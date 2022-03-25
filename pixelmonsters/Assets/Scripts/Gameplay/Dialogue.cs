using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Appears in inspector
public class Dialogue {
    
    // List of dialogues
    [SerializeField] private List<string> lines;

    public List<string> Lines
    {
        get { return lines; }
    }
}
