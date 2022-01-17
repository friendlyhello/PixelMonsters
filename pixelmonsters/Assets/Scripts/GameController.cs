using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Menu, Bag }
public class GameController : MonoBehaviour
{
  
  // References
  [SerializeField] private PlayerController playerController;
  [SerializeField] private BattleSystem battleSystem;
  [SerializeField] private Camera worldCamera;
  [SerializeField] private InventoryUI inventoryUI;

  private MenuController menuController;
  
  private GameState state;

  private void Awake()
  {
    menuController = GetComponent<MenuController>();
    
    // Lock cursor
    Cursor.lockState = CursorLockMode.Locked;
    
    // Hide cursor
    Cursor.visible = false;
  }

  // Subscribe to events
  private void Start()
  {
    playerController.OnEncountered += StartBattle;
    battleSystem.OnBattleOver += EndBattle;
    
    // Event Subscribers
    menuController.OnBack += () =>
    {
      state = GameState.FreeRoam;
    };

    menuController.OnMenuSelected += OnMenuSelected;
  }

  void StartBattle()
  {
    state = GameState.Battle;
    battleSystem.gameObject.SetActive(true);
    worldCamera.gameObject.SetActive(false);

    var playerParty = playerController.GetComponent<MonsterParty>();
    var wildMonster = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildMonster();
    
    battleSystem.StartBattle(playerParty, wildMonster);
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
      if (Input.GetKeyDown(KeyCode.Tab))
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
    
    else if (state == GameState.Bag)
    {
      Action OnBack = () =>
      {
        inventoryUI.gameObject.SetActive(false);
        state = GameState.FreeRoam;
      };
      
      inventoryUI.HandleUpdate(OnBack);
    }
    
  }

  private void OnMenuSelected(int selectedItem)
  {
    if (selectedItem == 0)
    {
      // Pixel Monster is selected
    }
    else if (selectedItem == 1)
    {
      // Bag is selected
      inventoryUI.gameObject.SetActive(true);
      state = GameState.Bag;
      
      Debug.Log("Bag is selected");
    }
    else if (selectedItem == 2)
    {
      // Save is selected
    }
    else if (selectedItem == 3)
    {
      // Load is selected
    }
  }
}
