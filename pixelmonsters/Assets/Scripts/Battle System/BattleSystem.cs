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
   
   // (!) Reference to Party Screen
   [SerializeField] private PartyScreen partyScreen;

   public event Action<bool> OnBattleOver;
   
   private BattleState state;
   private int currentAction;
   private int currentMove;

   private MonsterParty playerParty;
   private Monster wildMonster;
   
   public void StartBattle(MonsterParty playerParty, Monster wildMonster)
   {
      this.playerParty = playerParty;
      this.wildMonster = wildMonster;
      
      StartCoroutine(SetupBattle());
   }

   // Setup Battle method
   public IEnumerator SetupBattle()
   {
      // Setup the Player Unit
      playerUnit.Setup(playerParty.GetHealthyMonster());
      
      // Send data to the Player HUD
      playerHud.SetData(playerUnit.Monster); // Passes in to monster parameter in SetData() in BattleHud class
      
      // Setup the Enemy Unit
      enemyUnit.Setup(wildMonster);
      
      // Setup the Enemy HUD
      enemyHud.SetData(enemyUnit.Monster);
      
      // Setup the Party Screen
      partyScreen.Init();
      
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
      dialogBox.SetDialog("Choose an action.");
      dialogBox.EnableActionSelector(true);
   }

   void OpenPartyScreen()
   {
      partyScreen.SetPartyData(playerParty.Monsters);
      partyScreen.gameObject.SetActive(true);
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

      var move = playerUnit.Monster.Moves[currentMove];
      move.PP--;
      yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} used {move.Base.Name}");

      playerUnit.PlayAttackAnimation();
      yield return new WaitForSeconds(1f);
      
      enemyUnit.PlayHitAnimation();
      var damageDetails = enemyUnit.Monster.TakeDamage(move, playerUnit.Monster);
      yield return enemyHud.UpdateHP();
      yield return ShowDamageDetails(damageDetails);

      if (damageDetails.Fainted)
      {
         yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} Fainted");
         enemyUnit.PlayFaintAnimation();

         yield return new WaitForSeconds(2f);
         OnBattleOver(true);
      }

      else
      {
         StartCoroutine(EnemyMove());
      }
   }

   IEnumerator EnemyMove()
   {
      state = BattleState.EnemyMove;

      var move = enemyUnit.Monster.GetRandomMove();
      move.PP--;
      yield return dialogBox.TypeDialog($"{enemyUnit.Monster.Base.Name} used {move.Base.Name}");

      enemyUnit.PlayAttackAnimation();
      yield return new WaitForSeconds(1f);
      
      playerUnit.PlayHitAnimation();
      var damageDetails = playerUnit.Monster.TakeDamage(move, playerUnit.Monster);
      yield return playerHud.UpdateHP();
      yield return ShowDamageDetails(damageDetails);

      if (damageDetails.Fainted)
      {
         yield return dialogBox.TypeDialog($"{playerUnit.Monster.Base.Name} Fainted");
         playerUnit.PlayFaintAnimation();

         yield return new WaitForSeconds(2f);

         var nextMonster = playerParty.GetHealthyMonster();
         if (nextMonster != null)
         {
            playerUnit.Setup(nextMonster);
            playerHud.SetData(nextMonster);

            dialogBox.SetMoveNames(nextMonster.Moves);

            yield return dialogBox.TypeDialog($"Go {nextMonster.Base.Name}!");

            PlayerAction();
         }
         else
         {
            OnBattleOver(false);
         }
      }

      else
      { 
         PlayerAction();
      }
   }
   
   IEnumerator ShowDamageDetails(DamageDetails damageDetails)
   {
      if (damageDetails.Critical > 1f)
         yield return dialogBox.TypeDialog("A critical hit!");

      if (damageDetails.TypeEffectiveness > 1f)
         yield return dialogBox.TypeDialog("It's super effective!");
      else if (damageDetails.TypeEffectiveness < 1f)
         yield return dialogBox.TypeDialog("It's not very effective!");
   }
   
   // HandleUpdate wont be called automatically by Unity like Update does
   public void HandleUpdate()
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
      if (Input.GetKeyDown(KeyCode.RightArrow))
         ++currentAction;
      else if (Input.GetKeyDown(KeyCode.LeftArrow))
         --currentAction;
      else if (Input.GetKeyDown(KeyCode.DownArrow))
         currentAction += 2;
      else if (Input.GetKeyDown(KeyCode.UpArrow))
         currentAction -= 2;
      
      currentAction = Mathf.Clamp(currentAction, 0, 3);

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
            // Bag
         }
         else if (currentAction == 2)
         {
            // Monsters - Party screen to switch out Monsters
            OpenPartyScreen();
         }
         else if (currentAction == 3)
         {
            // Run
         }
      }
   }

   private void HandleMoveSelection()
   {
      if (Input.GetKeyDown(KeyCode.RightArrow))
         ++currentMove;
      else if (Input.GetKeyDown(KeyCode.LeftArrow))
         --currentMove;
      else if (Input.GetKeyDown(KeyCode.DownArrow))
         currentMove += 2;
      else if (Input.GetKeyDown(KeyCode.UpArrow))
         currentMove -= 2;
      
      currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Monster.Moves.Count - 1);
      
      dialogBox.UpdateMoveSelection(currentMove, playerUnit.Monster.Moves[currentMove]);

      if (Input.GetKeyDown(KeyCode.Return))
      {
         dialogBox.EnableMoveSelector(false);
         dialogBox.EnableDialogText(true);
         StartCoroutine(PerformPlayerMove());
      }

      else if(Input.GetKeyDown(KeyCode.X))
      {
         dialogBox.EnableMoveSelector(false);
         dialogBox.EnableDialogText(true);
         PlayerAction();
      }
   }
}




