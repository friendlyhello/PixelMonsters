using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    // Since this class implements the IInteractable interface, the function is defined here:
    public void Interact()
    {
        Debug.Log("Interacting with NPC");
    }
}
