using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
   // Create ItemSlotUI dynamically
   
   // Reference to ItemList game object - ItemSlot UI will be spawned inside prefab
   [SerializeField] private GameObject itemList;
   
   // Reference to ItemSlotUI prefab
   [SerializeField] private ItemSlotUI itemSlotUI;

   [SerializeField] private Image itemIcon;
   [SerializeField] private TMP_Text itemDescription;
   
   private int selectedItem = 0;

   private List<ItemSlotUI> slotUIList;

   // Get List of ItemSlots from InventoryUI script - needed here to show list in UI
   // cached reference:
   private Inventory inventory;
   private void Awake()
   {
      inventory = Inventory.GetInventory();
   }

   private void Start()
   {
      // Show list of items in the UI
      UpdateItemList();
   }

   void UpdateItemList()
   {
      // Clear existing placeholder items in list
      foreach (Transform child in itemList.transform)
         Destroy(child.gameObject);
      
      // Initialize ItemSlotUi List
      slotUIList = new List<ItemSlotUI>();

      // Attach new items from list!
      foreach (var itemSlot in inventory.Slots)
      {
         // Instantiate ItemSlotUI prefab and then attach itemList as a child game object
         var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
         slotUIObj.SetData(itemSlot);
         
         // Add instantiated prefab to slotUIList
         slotUIList.Add(slotUIObj);
      }
      
      UpdateItemSelection();
   }

   public void HandleUpdate(Action OnBack)
   {
      int prevSelection = selectedItem;

      // Get all the items in the list
      if (Input.GetKeyDown(KeyCode.DownArrow))
         ++selectedItem;
      else if (Input.GetKeyDown(KeyCode.UpArrow))
         --selectedItem;

      // Clamp the selected item between 0 and the length of the menu items list
      selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

      // Only call UpdateItemSelection if there has been a change in the selection:
      if (prevSelection != selectedItem)
         UpdateItemSelection();
      
      if (Input.GetKeyDown(KeyCode.Escape))
         OnBack?.Invoke();
   }

   void UpdateItemSelection()
   {
      // Loop through the menu items
      for (int i = 0; i < slotUIList.Count; i++)
      {
         if(i == selectedItem)
            slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
         else
            slotUIList[i].NameText.color = Color.black;
      }
      // Set the item icon
      var item = inventory.Slots[selectedItem].Item;
      itemIcon.sprite = item.Icon;
      
      // Set the item decription
      itemDescription.text = item.Description;
   }
}
