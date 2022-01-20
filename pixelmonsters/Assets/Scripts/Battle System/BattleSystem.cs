using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Control the Game States
public enum BattleState {Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver}
public class BattleSystem : MonoBehaviour
{
   // (!) References for Player HUD and Player Unit scripts
   [SerializeField] private BattleUnit playerUnit;

   // (!) References for Enemy HUD and Enemy Unit scripts
   [SerializeField] private BattleUnit enemyUnit;

   // (!) Reference to BattleDialogBox class
   [SerializeField] private BattleDialogBox dialogBox;
   
   // (!) Reference to Party Screen
   [SerializeField] private PartyScreen partyScreen;

   public event Action<bool> OnBattleOver;
   
   private BattleState state;
   private int currentAction;
   private int currentMove;
   private int currentMember;

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

      // Setup the Enemy Unit
      enemyUnit.Setup(wildMonster);

      // Setup the Party Screen
      partyScreen.Init();
      
      // Set and display monster move names
      dialogBox.SetMoveNames(playerUnit.Monster.Moves);
      
      // Set the dialogue in the dialogue box UI
      yield return dialogBox.TypeDialog($"A wild {enemyUnit.Monster.Base.Name} appeared!");
      yield return new WaitForSeconds(1f);

      ChooseFirstTurn();
   }

   void ChooseFirstTurn()
   {
      if (playerUnit.Monster.Speed >= enemyUnit.Monster.Speed)
         ActionSelection();
      else
         StartCoroutine(EnemyMove());
   }

   void BattleOver(bool won)
   {
      state = BattleState.BattleOver;
      playerParty.Monsters.ForEach(m => m.OnBattleOver()); // shorthand for each using Linq
      OnBattleOver(won);
   }
   
   void ActionSelection()
   {
      state = BattleState.ActionSelection;
      dialogBox.SetDialog("Choose an action.");
      dialogBox.EnableActionSelector(true);
   }

   void OpenPartyScreen()
   {
      state = BattleState.PartyScreen;
      partyScreen.SetPartyData(playerParty.Monsters);
      partyScreen.gameObject.SetActive(true);
   }

   void MoveSelection()
   {
      state = BattleState.MoveSelection;
      dialogBox.EnableActionSelector(false);
      dialogBox.EnableDialogText(false);
      dialogBox.EnableMoveSelector(true);
   }

   IEnumerator PlayerMove()
   {
      state = BattleState.PerformMove;

      var move = playerUnit.Monster.Moves[currentMove];
      yield return RunMove(playerUnit, enemyUnit, move);
      
      // If battle state was not changed to RunMove(), go to next step 
      if(state == BattleState.PerformMove)
         StartCoroutine(EnemyMove());
   }

   IEnumerator EnemyMove()
   {
      state = BattleState.PerformMove;

      var move = enemyUnit.Monster.GetRandomMove();
      yield return RunMove(enemyUnit, playerUnit, move);
      
      // If battle state was not changed to RunMove(), go to next step
      if(state == BattleState.PerformMove)
         ActionSelection();
   }

   IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
   {
      move.PP--;
      yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} used {move.Base.Name}");

      sourceUnit.PlayAttackAnimation();
      yield return new WaitForSeconds(1f);
      targetUnit.PlayHitAnimation();

      if (move.Base.Category == MoveCategory.Status)
      {
         yield return (RunMoveEffects(move, sourceUnit.Monster, targetUnit.Monster));
      }

      else
      {
         var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
         yield return targetUnit.Hud.UpdateHP();
         yield return ShowDamageDetails(damageDetails);
      }

      if (targetUnit.Monster.HP <= 0)
      {
         yield return dialogBox.TypeDialog($"{targetUnit.Monster.Base.Name} Fainted");
         targetUnit.PlayFaintAnimation();
         yield return new WaitForSeconds(2f);
         
         CheckForBattleOver(targetUnit);
      }
      
      // Status effects like brn or psn will hurt the monster after the turn
      sourceUnit.Monster.OnAfterTurn();
      yield return ShowStatusChanges(sourceUnit.Monster);
      yield return sourceUnit.Hud.UpdateHP();
      if (sourceUnit.Monster.HP <= 0)
      {
         yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} Fainted");
         sourceUnit.PlayFaintAnimation();
         yield return new WaitForSeconds(2f);
         
         CheckForBattleOver(sourceUnit);
      }
   }
   

   IEnumerator RunMoveEffects(Move move, Monster source, Monster target)
   {
      var effects = move.Base.Effects;
      if (effects.Boosts != null)
      {
         if(move.Base.Target == MoveTarget.Self)
            source.ApplyBoosts(effects.Boosts);
         else
            target.ApplyBoosts(effects.Boosts);
      }

      if (effects.Status != ConditionID.none)
      {
         target.SetStatus(effects.Status);
      }
      
      yield return ShowStatusChanges(source);
      yield return ShowStatusChanges(target);
   }
   
   IEnumerator ShowStatusChanges(Monster monster)
   {
      while (monster.StatusChanges.Count > 0)
      {
         var message = monster.StatusChanges.Dequeue();
         yield return dialogBox.TypeDialog(message);
      }  
   }
   

   void CheckForBattleOver(BattleUnit faintedUnit)
   {
      if (faintedUnit.IsPlayerUnit)
      {
         var nextMonster = playerParty.GetHealthyMonster();
         if (nextMonster != null)
            OpenPartyScreen();
         else
            BattleOver(false);
      }
      else
         BattleOver(true);
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
      if (state == BattleState.ActionSelection)
      {
         HandleActionSelection();
      }
      else if (state == BattleState.MoveSelection)
      {
         HandleMoveSelection();
      }
      else if (state == BattleState.PartyScreen)
      {
         HandlePartyScreenSelection();
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
            MoveSelection();
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
         StartCoroutine(PlayerMove());
      }

      else if(Input.GetKeyDown(KeyCode.X))
      {
         dialogBox.EnableMoveSelector(false);
         dialogBox.EnableDialogText(true);
         ActionSelection();
      }
   }

   private void HandlePartyScreenSelection()
   {
      if (Input.GetKeyDown(KeyCode.RightArrow))
         ++currentMember;
      else if (Input.GetKeyDown(KeyCode.LeftArrow))
         --currentMember;
      else if (Input.GetKeyDown(KeyCode.DownArrow))
         currentMember += 2;
      else if (Input.GetKeyDown(KeyCode.UpArrow))
         currentMember -= 2;
      
      currentMember = Mathf.Clamp(currentMember, 0, playerParty.Monsters.Count - 1);
      
      partyScreen.UpdateMemberSelection(currentMember);

      if (Input.GetKeyDown(KeyCode.Return))
      {
         var selectedMember = playerParty.Monsters[currentMember];
         if (selectedMember.HP <= 0)
         {
            partyScreen.SetMessageText("You can't send out FAINTED monsters!");
            return;
         }

         if (selectedMember == playerUnit.Monster)
         {
            partyScreen.SetMessageText("You can't switch with the SAME monster!");
            return;
         }
         
         partyScreen.gameObject.SetActive(false);
         state = BattleState.Busy;
         StartCoroutine(SwitchMonster(selectedMember));
      }
      
      else if (Input.GetKeyDown(KeyCode.Escape))
      {
         partyScreen.gameObject.SetActive(false);
         ActionSelection();
      }
   }

   IEnumerator SwitchMonster(Monster newMonster)
   {
      bool currentMonsterFainted = true;
      
      if (playerUnit.Monster.HP > 0)
      {
         currentMonsterFainted = false;
         yield return dialogBox.TypeDialog($"Come back {playerUnit.Monster.Base.Name}!");
         playerUnit.PlayFaintAnimation();
         yield return new WaitForSeconds(2.0f);
      }
      
      playerUnit.Setup(newMonster);
      dialogBox.SetMoveNames(newMonster.Moves);
      yield return dialogBox.TypeDialog($"Go {newMonster.Base.Name}!");

      if(currentMonsterFainted)
         ChooseFirstTurn();
      else
         StartCoroutine(EnemyMove());
   }
}




