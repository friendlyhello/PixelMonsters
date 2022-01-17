using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    
    private PartyMemberUI[] memberSlots;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Monster> monsters)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if(i < monsters.Count)
                memberSlots[i].SetData(monsters[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a Monster";
    }
}
