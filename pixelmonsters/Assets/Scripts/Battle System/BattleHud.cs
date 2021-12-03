using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
   // Reference to Name text, Level text, and the HP Bar
   [SerializeField] private Text nameText;
   [SerializeField] private Text levelText;
   [SerializeField] private HPBar hpBar;

   public void SetData(Monster monster)
   {
      nameText.text = monster.Base.Name;
      levelText.text = "Lvl " + monster.Level;
      hpBar.SetHP((float)monster.HP / monster.MaxHp);
   }
}
