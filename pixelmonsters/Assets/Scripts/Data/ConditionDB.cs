using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Finish implementing status effects!

// This class stores instances of Condition class
public class ConditionDB
{
    // Store list of conditions, static so no need to create instances of ConditionsDB class
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } 
        = new Dictionary<ConditionID, Condition>()
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
    psn, brn, slp, par, frz
}
