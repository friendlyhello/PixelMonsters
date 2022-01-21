using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
   // Reference to Name text, Level text, and the HP Bar
   [SerializeField] private Text nameText;
   [SerializeField] private Text levelText;
   [SerializeField] private Text statusText;
   
   // Reference to HPBar class
   [SerializeField] private HPBar hpBar;

   [SerializeField] private Color psnColor;
   [SerializeField] private Color brnColor;
   [SerializeField] private Color parColor;
   [SerializeField] private Color slpColor;
   [SerializeField] private Color frzColor;
   
   // Reference to Monster class
   private Monster _monster;

   public Dictionary<ConditionID, Color> statusColors;

   // Pass in Monster class
   public void SetData(Monster monster)
   {
      _monster = monster;
      
      nameText.text = monster.Base.Name;
      levelText.text = "Lvl " + monster.Level;
      
      // Thanks to reference to HPBar class, SetHP function can be accessed with dot notation:
      hpBar.SetHP((float)monster.HP / monster.MaxHp); // parameter passed in to HpNormalized in SetHP

      statusColors = new Dictionary<ConditionID, Color>()
      {
         {ConditionID.psn, psnColor},
         {ConditionID.brn, brnColor},
         {ConditionID.par, parColor},
         {ConditionID.slp, slpColor},
         {ConditionID.frz, frzColor}
      };
      
      SetStatusText();
      _monster.OnStatusChanged += SetStatusText;
   }

   private void SetStatusText()
   {
      if (_monster.Status == null)
      {
         statusText.text = "";
      }
      
      else
      {
         statusText.text = _monster.Status.Id.ToString().ToUpper();
         statusText.color = statusColors[_monster.Status.Id];  
      }
   }
   public IEnumerator UpdateHP()
   {
      if (_monster.hpChanged)
      {
         yield return hpBar.SetHPSmooth((float)_monster.HP / _monster.MaxHp);
         _monster.hpChanged = false;
      }
   }
}
