using System.Collections.Generic;
using UnityEngine;

    [CreateAssetMenu(fileName = "Move", menuName = "Monster/Create New Move")]
    
    public class MoveBase : ScriptableObject
{
    [SerializeField] private new string name;

    [TextArea] [SerializeField] private string description;

    [SerializeField] private MonsterType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;

    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveEffects effects;
    [SerializeField] private MoveTarget target;
    // (!) Properties that return the values entered in the scriptable object
    public string Name
    {
        get { return name; }
    }
    
    public string Description
    {
        get { return description; }
    }

    public MonsterType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int PP
    {
        get { return pp; }
    }
    
    // Expose category property
    public MoveCategory Category
    {
        get { return category; }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }
}

[System.Serializable] // Expose in Inspector
public class MoveEffects
{
    [SerializeField] private List<StatBoost> boosts;

    public List<StatBoost> Boosts
    {
        get { return boosts; }
    }
}

[System.Serializable] // Expose in Inspector
public class StatBoost
{
    public Stat stat;
    public int boost;
}

public enum MoveCategory
{
    Physical,
    Special,
    Status
}

public enum MoveTarget
{
    Foe,
    Self
}