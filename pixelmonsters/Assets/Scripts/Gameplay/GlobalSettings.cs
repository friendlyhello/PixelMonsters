using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
   // UI Highlighted Color
   [SerializeField] private Color highlightedColor;

   public Color HighlightedColor => highlightedColor;
   
   // Singleton so it can be accessed from anywhere
   public static GlobalSettings i { get; private set; }

   private void Awake()
   {
      i = this;
   }
}
