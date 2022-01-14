using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemSlotUI : MonoBehaviour
{
  // Reference to Name and Item Count text in the ItemSlot prefab
  [SerializeField] private TMP_Text nameText;
  [SerializeField] private TMP_Text countText;
  
  // Expose variable w/properties
  public TMP_Text NameText => nameText;
  public TMP_Text CountText => countText;

  public void SetData(ItemSlot itemSlot)
  {
    nameText.text = itemSlot.Item.Name;
    countText.text = $"x {itemSlot.Count}";
  }
}
