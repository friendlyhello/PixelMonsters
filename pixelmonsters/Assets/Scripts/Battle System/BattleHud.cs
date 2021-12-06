using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
   // Reference to Name text, Level text, and the HP Bar
   [SerializeField] private Text nameText;
   [SerializeField] private Text levelText;
   
   // Reference to HPBar class
   [SerializeField] private HPBar hpBar;

   // Reference to Monster class
   private Monster _monster;
   
   // Pass in Monster class
   public void SetData(Monster monster)
   {
      _monster = monster;
      
      nameText.text = monster.Base.Name;
      levelText.text = "Lvl " + monster.Level;
      
      // Thanks to reference to HPBar class, SetHP function can be accessed with dot notation:
      hpBar.SetHP((float)monster.HP / monster.MaxHp); // parameter passed in to HpNormalized in SetHP
   }

   public IEnumerator UpdateHP()
   {
      yield return hpBar.SetHPSmooth((float)_monster.HP / _monster.MaxHp);
   }
}
