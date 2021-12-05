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
   
   // (!) Reference to BattleDialogBox class
   [SerializeField] private BattleDialogBox dialogBox;
   
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
      playerHud.SetData(playerUnit.Monster); // Passes in to monster parameter in SetData() in BattleHud class
      
      // Setup the Enemy Unit
      enemyUnit.Setup();
      
      // Setup the Enemy HUD
      enemyHud.SetData(enemyUnit.Monster);
      
      // Set the dialogue in the dialogue box UI
      StartCoroutine(dialogBox.TypeDialog($"A wild {enemyUnit.Monster.Base.Name} appeared!"));
      // Using '$' allows for value of different variables 
   }
}



