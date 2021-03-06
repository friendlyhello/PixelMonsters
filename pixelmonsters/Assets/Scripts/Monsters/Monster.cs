using System.Collections.Generic;
using System.Linq;
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
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    
    public Condition Status { get; private set; }
    
    public int StatusTime { get; set; }
    
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }
    
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();

    public bool hpChanged { get; set; }

    public event System.Action OnStatusChanged;

    // Initialize (Init) Monster
    public void Init()
    {
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
        
        CalculateStats();
        HP = MaxHp;
        
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        
        // Calculate and store the value for each stat
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100.0f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100.0f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100.0f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100.0f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100.0f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;
    }

    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defense, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefense, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasion, 0}
        };
    }
    
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];
        
        // Apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f};

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);
        
        return statVal;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
            
            if(boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");    
            
            Debug.Log($"{stat} has been BOOSTED to {StatBoosts[stat]}");
        }
    }

    // Properties to get stats from Monster Base Class
    public int Attack
    {
        // This is the actual formula used in the Pokem??n game
        get { return GetStat(Stat.Attack); }
    }
    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }
    public int spAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }
    public int spDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }
    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }
    public int MaxHp { get; private set; }

    public DamageDetails TakeDamage(Move move, Monster attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) *
                     TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.spAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? spDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float) attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        UpdateHP(damage);

        return damageDetails;
    }

    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage,0, MaxHp);
        hpChanged = true;
    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null) return;
        
        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null; 
        OnStatusChanged?.Invoke();
    }
    
    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus != null) return;
        
        VolatileStatus = ConditionsDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }
    
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }
    
    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();
        
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        
        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }
        
        return canPerformMove;
    }
    
    public void OnAfterTurn()
    {
        // I didn't know you could use ? twice, or on multiple times
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
