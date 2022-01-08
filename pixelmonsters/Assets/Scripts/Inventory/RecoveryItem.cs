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

    [SerializeField] private int hpAmount;
    [SerializeField] private bool restoreMaxHP;
    
    [SerializeField] private int ppAmount;
    [SerializeField] private bool restoreMaxPP;

    [SerializeField] private ConditionID status;
    [SerializeField] private bool recoverAllStatus;
    
    [SerializeField] private bool revive;
    [SerializeField] private bool maxRevive;
}
