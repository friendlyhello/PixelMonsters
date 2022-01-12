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
