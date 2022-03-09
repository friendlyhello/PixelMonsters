using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Control the Game States
public enum BattleState {Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver}

public enum BattleAction { Move, SwitchMonster, UseItem, Run }

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
   private BattleState? prevState; // ? is how you make something nullable
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

    ActionSelection();
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

   IEnumerator RunTurns(BattleAction playerAction)
   {
      state = BattleState.RunningTurn;

      if (playerAction == BattleAction.Move)
      {
         playerUnit.Monster.CurrentMove = playerUnit.Monster.Moves[currentMove];
         enemyUnit.Monster.CurrentMove = enemyUnit.Monster.GetRandomMove();

         int playerMovePriority = playerUnit.Monster.CurrentMove.Base.Priority;
         int enemyMovePriority = enemyUnit.Monster.CurrentMove.Base.Priority;
         
         // Check who goes first
         bool playerGoesFirst = true;
         if (enemyMovePriority > playerMovePriority)
            playerGoesFirst = false;
         else if(enemyMovePriority == playerMovePriority)
            playerGoesFirst = playerUnit.Monster.Speed >= enemyUnit.Monster.Speed;
            
         var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
         var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

         var secondMonster = secondUnit.Monster;
         
         // RunMove() for First Turn
         yield return RunMove(firstUnit, secondUnit, firstUnit.Monster.CurrentMove);
         yield return RunAfterTurn(firstUnit);
         if (state == BattleState.BattleOver) yield break;

         if (secondMonster.HP >= 0)
         {
            // RunMove() for Second Turn
            yield return RunMove(secondUnit, firstUnit, secondUnit.Monster.CurrentMove);
            yield return RunAfterTurn(secondUnit);
            if (state == BattleState.BattleOver) yield break;
         }
      }

      else
      {
         if (playerAction == BattleAction.SwitchMonster)
         {
            var selectedMonster = playerParty.Monsters[currentMember];
            state = BattleState.Busy;
            yield return SwitchMonster(selectedMonster);
         }
         
         // Enemy Turn
         var enemyMove = enemyUnit.Monster.GetRandomMove();
         yield return RunMove(enemyUnit,playerUnit,enemyMove);
         yield return RunAfterTurn(enemyUnit);
         if (state == BattleState.BattleOver) yield break;
      }

      if (state != BattleState.BattleOver)
         ActionSelection(); 
   }

   IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
   {
      bool canRunMove = sourceUnit.Monster.OnBeforeMove();
      if (!canRunMove)
      {
         yield return ShowStatusChanges(sourceUnit.Monster);
         yield return sourceUnit.Hud.UpdateHP();
         yield break;
      }

      yield return ShowStatusChanges(sourceUnit.Monster);
      
      move.PP--;
      yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name} used {move.Base.Name}");

      // Check if move hits logic
      if (CheckIfMoveHits(move, sourceUnit.Monster, targetUnit.Monster))
      {
         sourceUnit.PlayAttackAnimation();
         yield return new WaitForSeconds(1f);
         targetUnit.PlayHitAnimation();

         if (move.Base.Category == MoveCategory.Status)
         {
            yield return (RunMoveEffects(move.Base.Effects, sourceUnit.Monster, targetUnit.Monster, move.Base.Target));
         }

         else
         {
            var damageDetails = targetUnit.Monster.TakeDamage(move, sourceUnit.Monster);
            yield return targetUnit.Hud.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
         }

         if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Monster.HP > 0)
         {
            foreach (var secondary in move.Base.Secondaries)
            {
               var rnd = UnityEngine.Random.Range(1, 101);
               if(rnd <= secondary.Chance)
                  yield return (RunMoveEffects(secondary, sourceUnit.Monster, targetUnit.Monster, secondary.Target));
            }
         }

         if (targetUnit.Monster.HP <= 0)
         {
            yield return dialogBox.TypeDialog($"{targetUnit.Monster.Base.Name} Fainted");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
         }
      }
      
      else
      {
         yield return dialogBox.TypeDialog($"{sourceUnit.Monster.Base.Name}'s Attack Missed!");
      }
   }
   
   IEnumerator RunMoveEffects(MoveEffects effects, Monster source, Monster target, MoveTarget moveTarget)
   {
      // Stat boosting
      if (effects.Boosts != null)
      {
         if(moveTarget == MoveTarget.Self)
            source.ApplyBoosts(effects.Boosts);
         else
            target.ApplyBoosts(effects.Boosts);
      }

      // Status condition
      if (effects.Status != ConditionID.none)
      {
         target.SetStatus(effects.Status);
      }
      
      // Volatile status condition
      if (effects.VolatileStatus != ConditionID.none)
      {
         target.SetVolatileStatus(effects.VolatileStatus);
      }
      
      yield return ShowStatusChanges(source);
      yield return ShowStatusChanges(target);
   }

   IEnumerator RunAfterTurn(BattleUnit sourceUnit)
   {
      if (state == BattleState.BattleOver) yield break;
      yield return new WaitUntil((() => state == BattleState.RunningTurn));
      
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
   
   bool CheckIfMoveHits(Move move, Monster source, Monster target)
   {
      if (move.Base.AlwaysHits)
         return true;
      
      // (!) Check if random number between 1 - 100 is <= new move accuracy number
      // (if it will hit or not)

      // accuracy variable
      float moveAccuracy = move.Base.Accuracy;

      int accuracy = source.StatBoosts[Stat.Accuracy];
      int evasion = target.StatBoosts[Stat.Evasion];

      var boostValues = new float[] {1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f};

      if (accuracy > 0)
         moveAccuracy *= boostValues[accuracy];
      else
         moveAccuracy /= boostValues[-accuracy];
      
      if (evasion > 0)
         moveAccuracy /= boostValues[evasion];
      else
         moveAccuracy *= boostValues[-evasion];
      
      // Random number generated, if generated number is <= move accuracy, move will hit
      return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
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
            prevState = state;
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
         var move = playerUnit.Monster.Moves[currentMove];
         if (move.PP == 0) return;
         
         dialogBox.EnableMoveSelector(false);
         dialogBox.EnableDialogText(true);
         StartCoroutine(RunTurns(BattleAction.Move));
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

         if (prevState == BattleState.ActionSelection)
         {
            prevState = null;
            StartCoroutine(RunTurns(BattleAction.SwitchMonster));
         }
         else
         {
            state = BattleState.Busy;
            StartCoroutine(SwitchMonster(selectedMember));
         }
      }
      
      else if (Input.GetKeyDown(KeyCode.Escape))
      {
         partyScreen.gameObject.SetActive(false);
         ActionSelection();
      }
   }

   IEnumerator SwitchMonster(Monster newMonster)
   {
      if (playerUnit.Monster.HP > 0)
      {
         yield return dialogBox.TypeDialog($"Come back {playerUnit.Monster.Base.Name}!");
         playerUnit.PlayFaintAnimation();
         yield return new WaitForSeconds(2.0f);
      }
      
      playerUnit.Setup(newMonster);
      dialogBox.SetMoveNames(newMonster.Moves);
      yield return dialogBox.TypeDialog($"Go {newMonster.Base.Name}!");

      state = BattleState.RunningTurn;
   }
}




