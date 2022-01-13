using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour
{
  // Reference to Menu GameObject
  [SerializeField] private GameObject menu;

  // Fire this event when Enter is pressed on a selected item in the menu
  public event Action<int> OnMenuSelected;
  
  // Fire this event to return to the main menu list
  public event Action OnBack;
  
  private List<TMP_Text> menuItems;

  private int selectedItem = 0;

  private void Awake()
  {
    // Get the menuItem TMP text children in menu
    menuItems = menu.GetComponentsInChildren<TMP_Text>().ToList();
  }

  public void OpenMenu()
  {
    menu.SetActive(true);
    
    // Make sure an item is selected when the menu is open
    UpdateItemSelection();
  }
  
  public void CloseMenu()
  {
    menu.SetActive(false);
  }

  // Menu item selection highlighting
  public void HandleUpdate()
  {
    // Store previous selection in a variable
    int prevSelection = selectedItem;

    // Get all the items in the list
    if (Input.GetKeyDown(KeyCode.DownArrow))
      ++selectedItem;
    else if (Input.GetKeyDown(KeyCode.UpArrow))
      --selectedItem;

    // Clamp the selected item between 0 and the length of the menu items list
    selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

    // Only call UpdateItemSelection if there has been a change in the selection:
    if (prevSelection != selectedItem)
      UpdateItemSelection();
    
    // TODO: Implement Save/Load
    // Notify the GameController about the item selections
    if (Input.GetKeyDown(KeyCode.Return))
    {
      // Event handler that notifies GameController script to SAVE game (Don't save the game here!)
      OnMenuSelected?.Invoke(selectedItem);
      CloseMenu();
    }
    else if (Input.GetKeyDown(KeyCode.Escape))
    {
      OnBack?.Invoke();
      CloseMenu();
    }
  }

// Update the selected item in the UI
  void UpdateItemSelection()
  {
    // Loop through the menu items
    for (int i = 0; i < menuItems.Count; i++)
    {
      if(i == selectedItem)
        menuItems[i].color = GlobalSettings.i.HighlightedColor;
      else
        menuItems[i].color = Color.black;
    }
  }
}
