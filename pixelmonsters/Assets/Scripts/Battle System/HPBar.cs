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
    
    public IEnumerator SetHPSmooth(float newHp)
    {
        float curHp = health.transform.localScale.x;
        float changeAmt = curHp - newHp;

        while (curHp - newHp > Mathf.Epsilon)
        {
            curHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHp, 1f);
    }
}
