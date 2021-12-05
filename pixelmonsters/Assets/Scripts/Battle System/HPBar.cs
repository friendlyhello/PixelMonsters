using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    // Reference to UI image Health in BattleHud/HPBar game object
    [SerializeField] private GameObject health;
    
    // hpBar.SetHP((float)monster.HP / monster.MaxHp) is the HpNormalized getting passed in
    public void SetHP(float HpNormalized)
    {
        health.transform.localScale = new Vector3(HpNormalized, 1f);
    }
}
