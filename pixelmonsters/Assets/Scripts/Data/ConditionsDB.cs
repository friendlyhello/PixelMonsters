using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class stores instances of Condition class
public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }
    
    // Store list of conditions, static so no need to create instances of ConditionsDB class
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() 
    {
        // Define status conditions:
        {
            ConditionID.psn,
            new Condition() 
            {
                Name = "Poison",
                StartMessage = "has been Poisoned!",
                
                // Lamda function to define function while assigning it 
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHP(monster.MaxHp / 8);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} hurt itself due to POISON");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition() 
            {
                Name = "Burn",
                StartMessage = "has been Burned!",
                
                // Lamda function to define function while assigning it 
                OnAfterTurn = (Monster monster) =>
                {
                    monster.UpdateHP(monster.MaxHp / 16);
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} hurt itself due to BURN");
                }
            }
        },
        {
            ConditionID.par,
            new Condition() 
            {
                Name = "Paralyzed",
                StartMessage = "has been Paralyzed!",
                OnBeforeMove = (Monster monster) =>
                {
                    // 1 out of 4 times monster wont be able to perform paralyze
                    if (Random.Range(1, 5) == 1)
                    {
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is PARALYZED and cant's move!");
                        return false;
                    }
                    
                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition() 
            {
                Name = "Freeze",
                StartMessage = "has been Frozen!",
                OnBeforeMove = (Monster monster) =>
                {
                    // 1 out of 4 times monster wont be able to perform paralyze
                    if (Random.Range(1, 5) == 1)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} is not FROZEN anymore");
                        return true;
                    }
                    
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition() 
            {
                Name = "Sleep",
                StartMessage = "has fallen asleep!",
                
                OnStart = (Monster monster) =>
                { 
                    // Sleep for 1 - 3 turns
                    monster.StatusTime = Random.Range(1, 4);
                    Debug.Log($"Will sleep for {monster.StatusTime} moves");
                },
                
                OnBeforeMove = (Monster monster) =>
                {
                    if (monster.StatusTime <= 0)
                    {
                        monster.CureStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} woke up!");
                        return true;
                    }
                    
                    monster.StatusTime--;
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} is sleeping!");
                    return false;
                }
            }
        },
        {
            ConditionID.confusion,
            new Condition() 
            {
                Name = "Confusion",
                StartMessage = "is confused!",
                
                OnStart = (Monster monster) =>
                { 
                    // Sleep for 1 - 3 turns
                    monster.VolatileStatusTime = Random.Range(1, 4);
                    Debug.Log($"Will be confused for {monster.VolatileStatusTime} moves");
                },
                
                OnBeforeMove = (Monster monster) =>
                {
                    if (monster.VolatileStatusTime <= 0)
                    {
                        monster.CureVolatileStatus();
                        monster.StatusChanges.Enqueue($"{monster.Base.Name} kicked out of confusion!");
                        return true;
                    }

                    monster.VolatileStatusTime--;

                    if (Random.Range(1, 3) == 1)
                        return true;
                    
                    monster.StatusChanges.Enqueue($"{monster.Base.Name} is confused!");
                    monster.UpdateHP(monster.HP / 8);
                    monster.StatusChanges.Enqueue($"Hurt itself due to confusion");
                    return false;
                }
            }
        }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz, confusion
}
