using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster
{
    [SerializeField] private MonsterBase _base;
    [SerializeField] private int level;

    // (!) C# Monster Class calculates all the values specific to a Monster's Level

    // (!)  Reference to Monster Base Class in order to access all base data
    //      These two values can be used to calculate all the base levels for the Monsters

    // These were private but then changed to properties so they can be accessed outside of the class.
    public MonsterBase Base { 
        get {
            return _base;
        }
    }

    public int Level {
        get {
            return level; 
        }
    }

    // Current monster HP
    public int HP { get; set; }

    // These are the moves the monsters actually have, not the moves they learn
    public List<Move> Moves { get; set; }

    // Monster constructor
    public void Init()
    {
        HP = MaxHp;

        // (!) Generate monster moves based on level

        // Initially set moves to empty list
        Moves = new List<Move>();

        // Loop through Learnable Moves, then add it to the list, based on the level
        foreach (var move in Base.LearnableMoves)
        {
            // Check if level at which move can be learned is <= to the level of
            // the monster
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));

            // Monsters can only have four moves, if there are more than four moves, 
            // don't add anymore moves and exit loop
            if (Moves.Count >= 4)
                break;
        }
    }

    // Properties to get stats from Monster Base Class
    public int Attack
    {
        // This is the actual formula used in the Pokemón game
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100.0f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100.0f) + 5; }
    }

    public int spAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100.0f) + 5; }
    }

    public int spDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100.0f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100.0f) + 5; }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100.0f) + 10; }
    }

    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.IsSpecial) ? attacker.spAttack : attacker.Attack;
        float defense = (move.Base.IsSpecial) ? spDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float) attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
