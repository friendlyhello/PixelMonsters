using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
   public void HandleUpdate(Action OnBack)
   {
      if (Input.GetKeyDown(KeyCode.Escape))
      OnBack?.Invoke();
   }
}
