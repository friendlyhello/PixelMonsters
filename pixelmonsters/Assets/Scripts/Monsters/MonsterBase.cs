using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create New Monster")]
public class MonsterBase : ScriptableObject
{
    // (!) Contains all base info for a Monster: Name, Type, Base Stats, etc

    // Variables to store Monster Data
    [SerializeField] private string name;
    
    [TextArea] 
    [SerializeField] private string description;
    
    // Monster Sprites for front and back views
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private MonsterType type1;
    [SerializeField] private MonsterType type2;
    
    // Base Stats
    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;
}

public enum MonsterType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
}
