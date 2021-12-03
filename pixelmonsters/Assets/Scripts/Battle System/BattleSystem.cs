using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
   // (!) References for BattleHud and BattleUnit scripts
   [SerializeField] private BattleUnit playerUnit;
   [SerializeField] private BattleHud playerHud;

   private void Start()
   {
      SetupBattle();
   }
   
   // Setup Battle method
   public void SetupBattle()
   {
      // Setup the Player Unit
      playerUnit.Setup();
      
      // Send data to the Player HUD
      playerHud.SetData(playerUnit.Monster);
   }
}
