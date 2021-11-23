using UnityEngine;

// (!) Just plain C# class
public class Move 
{
    // Reference to MoveBase class
    
    // Short-hand auto-property, creates a private variable "behind the scenes"
    public MoveBase Base { get; set; }
    public int PP { get; set; }
    
    // Constructor
    public Move(MoveBase pBase, int pp)
    {
        Base = pBase;
        PP = pp;
    }
}
