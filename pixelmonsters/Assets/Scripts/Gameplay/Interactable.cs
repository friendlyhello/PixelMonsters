using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IInteractable // : MonoBehaviour - This is an interface!
{
   // You can only define functions (no implementation!) in an interface
   void Interact();
}
