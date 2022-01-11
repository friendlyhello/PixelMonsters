using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Menu }
public class GameController : MonoBehaviour
{
  
  // References
  [SerializeField] private PlayerController playerController;
  [SerializeField] private BattleSystem battleSystem;
  [SerializeField] private Camera worldCamera;

  private MenuController menuController;
  
  private GameState state;

  private void Awake()
  {
    menuController = GetComponent<MenuController>();
  }

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
      
      // Open/Close Menu
      if (Input.GetKeyDown(KeyCode.Return))
      {
        menuController.OpenMenu();
        state = GameState.Menu;
      }
    }
    
    else if (state == GameState.Battle)
    {
      // Give control to Battle System
      battleSystem.HandleUpdate();
    }
    
    else if (state == GameState.Menu)
    {
      menuController.HandleUpdate();
    }
  }
}
