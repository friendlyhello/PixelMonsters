using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  (!) This script holds all of the Item ScriptableObjects

public class Inventory : MonoBehaviour
{
    // In order to store all types of items, use ItemBase class (ScriptableObject base),
    // which will include inherited classes such as RecoveryItem, etc...
    [SerializeField] private List<ItemSlot> slots;

}

// This class will behave like a slot, which will hold one type of item and its item count

[Serializable]
public class ItemSlot
{
    // Reference to item
    [SerializeField] private ItemBase item;
    
    // Item count
    [SerializeField] private int count;
    
    // Properties to expose fields
    public ItemBase Item => item;
    public int Count => count;
}

