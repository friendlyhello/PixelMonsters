using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }
public class GameController : MonoBehaviour
{
  
  // References
  [SerializeField] private PlayerController playerController;
  [SerializeField] private BattleSystem battleSystem;
  [SerializeField] private Camera worldCamera;
  
  private GameState state;
  
  // Subscribe to events
  private void Start()
  {
    playerController.OnEncountered += StartBattle;
    battleSystem.OnBattleOver += EndBattle;
  }

  void StartBattle()
  {
    state = GameState.Battle;
    battleSystem.gameObject.SetActive(true);
    worldCamera.gameObject.SetActive(false);

    battleSystem.StartBattle();
  }

  void EndBattle(bool won)
  {
    state = GameState.FreeRoam;
    battleSystem.gameObject.SetActive(false);
    worldCamera.gameObject.SetActive(true); 
  }

  private void Update()
  {
    if (state == GameState.FreeRoam)
    {
      // Give control to Player
      playerController.HandleUpdate();
    }
    else if (state == GameState.Battle)
    {
      // Give control to Battle System
      battleSystem.HandleUpdate();
    }
  }
}
