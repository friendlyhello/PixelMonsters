using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class stores instances of Condition class
public class ConditionsDB
{
    // Store list of conditions, static so no need to create instances of ConditionsDB class
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>() 
    {
        {
            // Define status conditions:
            ConditionID.psn,
            new Condition() 
            {
                Name = "Poison",
                StartMessage = "has been Poisoned!"
            }
        }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz
}
