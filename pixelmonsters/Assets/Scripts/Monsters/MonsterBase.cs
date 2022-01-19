using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Create New Monster")]

public class MonsterBase : ScriptableObject
{
    // (!) Contains all base info for a Monster: Name, Type, Base Stats, etc

    // Variables to store Monster Data
    [SerializeField] private new string name;
    
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
    
    // List of Learnable Moves, LearnableMove class within the <>
    [SerializeField] private List<LearnableMove> learnableMoves;

    // Properties to expose variables outside of this scriptable object
    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }
    
    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }
    
    public Sprite BackSprite
    {
        get { return backSprite; }
    }
    
    public MonsterType Type1
    {
        get { return type1; }
    }
    
    public MonsterType Type2
    {
        get { return type2; }
    }
    
    public int MaxHp
    {
        get { return maxHp; }
    }
    
    public int Attack
    {
        get { return attack; }
    }
    
    public int Defense
    {
        get { return defense; }
    }
    
    public int SpAttack
    {
        get { return spAttack; }
    }
    
    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }
    
    // Property for learnableMove List
    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
}

// (!) This class needs to appear in the inspector, so use System.Serializable

[System.Serializable]
public class LearnableMove
{
    // Reference MoveBase ScriptableObject
    [SerializeField] private MoveBase moveBase;
    
    // Level at which move will be learned
    [SerializeField] private int level;
    
    // Properties to expose moveBase and level
    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
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
    Dragon
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed
}

public class TypeChart
{
    static readonly float[][] chart =
    {
        //                  NOR   FIR   WAT   ELE  GRA  ICE  FIG  POI               
        /*NOR*/ new float[] { 1f,  1f,   1f,  1f,  1f,  1f,  1f,  1f },
        /*FIR*/ new float[] { 1f, 0.5f, 0.5f, 1f,  2f,  2f,  1f,  1f },
        /*WAT*/ new float[] { 1f,  2f,  0.5f, 2f, 0.5f, 1f,  1f,  1f },
        /*ELE*/ new float[] { 1f,  1f,  2f,  0.5f,0.5f, 2f,  1f,  1f },
        /*GRS*/ new float[] { 1f, 0.5f, 2f,   2f, 0.5f, 1f,  1f, 0.5f },
        /*POI*/ new float[] { 1f,  1f,   1f,  1f,  2f,  1f,  1f,  1f }
    };

    public static float GetEffectiveness(MonsterType attackType, MonsterType defenseType)
    {
        if (attackType == MonsterType.None || defenseType == MonsterType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}
