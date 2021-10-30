using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    // (!) C# Monster Class calculates all the values specific to a Monster's Level
    
    // Reference to Monster Base Class in order to access all base data
    
    // These two values can be used to calculate all the base levels for the Monsters
    private MonsterBase _base;
    private int level;
    
    // Monster constructor
    public Monster(MonsterBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
    }
    
    // Properties to get stats from Monster Base Class
    public int Attack
    {
        // This is the actual formula used in the Pokemón game
        get { return Mathf.FloorToInt((_base.Attack * level) / 100.0f) + 5; }
    }
    
    public int Defense
    {
        get { return Mathf.FloorToInt((_base.Defense * level) / 100.0f) + 5; }
    }
    
    public int spAttack
    {
        get { return Mathf.FloorToInt((_base.SpAttack * level) / 100.0f) + 5; }
    }
    
    public int spDefense
    {
        get { return Mathf.FloorToInt((_base.SpDefense * level) / 100.0f) + 5; }
    }
    
    public int Speed
    {
        get { return Mathf.FloorToInt((_base.Speed * level) / 100.0f) + 5; }
    }
    
    public int MaxHp
    {
        get { return Mathf.FloorToInt((_base.MaxHp * level) / 100.0f) + 10; }
    }
}
