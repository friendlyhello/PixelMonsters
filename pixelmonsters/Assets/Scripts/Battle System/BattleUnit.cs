using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    // Reference to MonsterBase and Level
    [SerializeField] private MonsterBase _base;
    [SerializeField] private int level;
    
    // Bool to determine if it is a Player Unit or a Monster Unit
    [SerializeField] private bool isPlayerUnit;
    
    // Monster class reference
    public Monster Monster { get; set; }
    
    // Function that will create a Monster with _base and level 
    public void Setup()
    {
        // Set Monster (reference created above) to Monster constructor 
        Monster = new Monster(_base, level);
        
        // If it's a Player Unit, set image sprite of Player Unit (The back-facing one) with a monster sprite
        if (isPlayerUnit)
        {
            GetComponent<Image>().sprite = Monster.Base.BackSprite;
        }
        else
        {
            GetComponent<Image>().sprite = Monster.Base.FrontSprite;
        }
    }
}

