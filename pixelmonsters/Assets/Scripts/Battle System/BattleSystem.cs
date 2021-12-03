using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
   // (!) References for Player HUD and Player Unit scripts
   [SerializeField] private BattleUnit playerUnit;
   [SerializeField] private BattleHud playerHud;
   
   // (!) References for Enemy HUD and Enemy Unit scripts
   [SerializeField] private BattleUnit enemyUnit;
   [SerializeField] private BattleHud enemyHud;
   
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
      
      // Setup the Enemy Unit
      enemyUnit.Setup();
      
      // Setup the Enemy HUD
      enemyHud.SetData(enemyUnit.Monster);
   }
}
