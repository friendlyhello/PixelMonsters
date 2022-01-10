using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  (!) Because RecoveryItems inherits from ItemBase (ScriptableObject), 
//      RecoveryItem class is also a ScriptableObject!

//  Create instances of RecoveryItem (now also a ScriptableObject!) with attribute:
[CreateAssetMenu(menuName = "Items/Create new Recovery Item")]

public class RecoveryItem : ItemBase
{
    //  (!) New fields added to this class appear in Recovery Item ScriptableObject
    //      Fields for Recovery Item behaviours: Heal HP, PP, Fainted Monsters, etc.

    [Header("HP")]
    [SerializeField] private int HPAmount;
    [SerializeField] private bool restoreMaxHP;
    
    [Header("PP")]
    [SerializeField] private int PPAmount;
    [SerializeField] private bool restoreMaxPP;

    // TODO: Finish implementing Conditions
    [Header("Status Conditions")]
    [SerializeField] private ConditionID status;
    [SerializeField] private bool recoverAllStatus;
    
    [Header("Revive")]
    [SerializeField] private bool revive;
    [SerializeField] private bool maxRevive;
}
