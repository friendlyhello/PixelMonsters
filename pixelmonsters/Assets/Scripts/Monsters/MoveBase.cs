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
}