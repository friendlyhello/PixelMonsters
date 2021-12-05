using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

// Control the Game States
public enum BattleState {Start, PlayerAction, PlayerMove, EnemyMove, Busy}
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

   private BattleState state;
   private int currentAction;
   
   private void Start()
   {
      StartCoroutine(SetupBattle());
   }
   
   // Setup Battle method
   public IEnumerator SetupBattle()
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
      yield return dialogBox.TypeDialog($"A wild {enemyUnit.Monster.Base.Name} appeared!");
      yield return new WaitForSeconds(1f);

      PlayerAction();
   }

   private void Update()
   {
      if (state == BattleState.PlayerAction)
      {
         HandleActionSelection();
      }
   }
   
   void PlayerAction()
   {
      state = BattleState.PlayerAction;
      StartCoroutine(dialogBox.TypeDialog("Choose an action."));
      dialogBox.EnableActionSelector(true);
   }

   void HandleActionSelection()
   {
      if (Input.GetKeyDown(KeyCode.DownArrow))
      {
         if (currentAction < 1)
            ++currentAction;
         
         else if (Input.GetKeyDown(KeyCode.UpArrow))
            if (currentAction > 0)
               --currentAction;
      }
      
      dialogBox.UpdateActionSelection(currentAction);
   }
}



