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

   [SerializeField] private Image upArrow;
   [SerializeField] private Image downArrow;
   
   private int selectedItem = 0;
   
   const int itemsInViewport = 8;
   
   private List<ItemSlotUI> slotUIList;

   // Get List of ItemSlots from InventoryUI script - needed here to show list in UI
   // cached reference:
   private Inventory inventory;
   
   // Cache reference 
   private RectTransform itemListRect;
   private void Awake()
   {
      inventory = Inventory.GetInventory();
      itemListRect = itemList.GetComponent<RectTransform>();
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

      HandleScrolling();
   }

   void HandleScrolling()
   {
      // Get height of itemSlot
      float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * slotUIList[0].Height;
      
      // Set y position of itemList
      // (!)   Cache reference here, item UI is buried in UI Canvas prefab,
      //       so can't use normal rect transform)
      itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

      // Condition: Show up arrow if there are more than four items at the bottom of the items list in viewport
      bool showUpArrow = selectedItem > itemsInViewport / 2;
      upArrow.gameObject.SetActive(showUpArrow);
      
      // Condition: Show down arrow if there are more items bellow what is seen in viewport
      bool showDownArrow = selectedItem + itemsInViewport / 2 < slotUIList.Count;
      downArrow.gameObject.SetActive(showDownArrow);
   }
}
