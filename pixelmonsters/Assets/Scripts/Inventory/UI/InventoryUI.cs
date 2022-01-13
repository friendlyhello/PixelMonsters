using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
   // Create ItemSlotUI dynamically
   
   // Reference to ItemList game object - ItemSlot UI will be spawned inside prefab
   [SerializeField] private GameObject itemList;
   
   // Reference to ItemSlotUI prefab
   [SerializeField] private ItemSlotUI itemSlotUI;
   
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
      
      // Attach new items from list!
      foreach (var itemSlot in inventory.Slots)
      {
         // Instantiate ItemSlotUI prefab and then attach itemList as a child game object
         var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
         slotUIObj.SetData(itemSlot);
      }
   }

   public void HandleUpdate(Action OnBack)
   {
      if (Input.GetKeyDown(KeyCode.Escape))
         OnBack?.Invoke();
   }
}
