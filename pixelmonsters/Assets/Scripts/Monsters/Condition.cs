using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }

    public Action<Monster> OnStart { get; set; }
    
    // Func is an Action that returns a value and has a return type (bool). Action returns no value.
    public Func<Monster, bool> OnBeforeMove { get; set; }
    public Action<Monster> OnAfterTurn { get; set; }
}
