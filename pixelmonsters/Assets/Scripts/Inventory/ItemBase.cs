using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBase : ScriptableObject
{
    // Common properties all Item types will have
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon;
    
    // Properties to expose fields
    public string Name => name;
    public string Description => description;
    public Sprite Icon => icon;
}
