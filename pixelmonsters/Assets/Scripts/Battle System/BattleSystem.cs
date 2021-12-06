using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
   private int currentMove;
   
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
      
      // Set and display monster move names
      dialogBox.SetMoveNames(playerUnit.Monster.Moves);
      
      // Set the dialogue in the dialogue box UI
      yield return dialogBox.TypeDialog($"A wild {enemyUnit.Monster.Base.Name} appeared!");
      yield return new WaitForSeconds(1f);

      PlayerAction();
   }

   void PlayerAction()
   {
      state = BattleState.PlayerAction;
      StartCoroutine(dialogBox.TypeDialog("Choose an action."));
      dialogBox.EnableActionSelector(true);
   }

   void PlayerMove()
   {
      state = BattleState.PlayerMove;
      dialogBox.EnableActionSelector(false);
      dialogBox.EnableDialogText(false);
      dialogBox.EnableMoveSelector(true);
   }

   IEnumerator PerformPlayerMove()
   {
      state = BattleState.Busy;
      
      Move move = playerUnit.Monster.Moves[currentMove];
      yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} used {move.Base.Name}");
      
      yield return new WaitForSeconds(1f);

      bool isFainted = enemyUnit.Monster.TakeDamage(move, playerUnit.Monster);
      yield return enemyHud.UpdateHP();
      
      if (isFainted)
      {
         yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} Fainted");
      }
      else
      {
         StartCoroutine(EnemyMove());
      }
   }

   IEnumerator EnemyMove()
   {
      state = BattleState.EnemyMove;

      Move move = enemyUnit.Monster.GetRandomMove();
      yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} used {move.Base.Name}");
      
      yield return new WaitForSeconds(1f);

      bool isFainted = playerUnit.Monster.TakeDamage(move, playerUnit.Monster);
      yield return playerHud.UpdateHP();
      
      if (isFainted)
      {
         yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} Fainted");
      }
      else
      {
         PlayerAction();
      }
   }
   
   private void Update()
   {
      if (state == BattleState.PlayerAction)
      {
         HandleActionSelection();
      }
      else if (state == BattleState.PlayerMove)
      {
         HandleMoveSelection();
      }
   }
   
   void HandleActionSelection()
   {
      if (Input.GetKeyDown(KeyCode.DownArrow))
      {
         if (currentAction < 1)
            ++currentAction;
      }
      else if (Input.GetKeyDown(KeyCode.UpArrow))
      {
         if (currentAction > 0)
            --currentAction;
      }

      dialogBox.UpdateActionSelection(currentAction);

      if (Input.GetKeyDown(KeyCode.Return))
      {
         if (currentAction == 0)
         {
            // Fight
            PlayerMove();
         }
         else if (currentAction == 1)
         {
            // Run
         }
      }
   }

   private void HandleMoveSelection()
   {
      if (Input.GetKeyDown(KeyCode.RightArrow))
      {
         if (currentMove < playerUnit.Monster.Moves.Count - 1)
            ++currentMove;
      }
      else if (Input.GetKeyDown(KeyCode.LeftArrow))
      {
         if (currentMove > 0)
            --currentMove;
      }
      else if (Input.GetKeyDown(KeyCode.DownArrow))
      {
         if (currentMove < playerUnit.Monster.Moves.Count - 2)
            currentMove += 2;
      }
      else if (Input.GetKeyDown(KeyCode.UpArrow))
      {
         if (currentMove > 1)
            currentMove -= 2;
      }
      
      dialogBox.UpdateMoveSelection(currentMove, playerUnit.Monster.Moves[currentMove]);

      if (Input.GetKeyDown(KeyCode.Return))
      {
         dialogBox.EnableMoveSelector(false);
         dialogBox.EnableDialogText(true);
         StartCoroutine(PerformPlayerMove());
      }
   }
}




