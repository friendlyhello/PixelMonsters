using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    // Bool to determine if it is a Player Unit or a Monster Unit
    [SerializeField] private bool isPlayerUnit;
    [SerializeField] private BattleHud hud;
    
    // Expose isPlayerUnit
    public bool IsPlayerUnit {
        get { return isPlayerUnit; }
    }
    
    // Expose HUD
    public BattleHud Hud {
        get { return hud; }
    }
    
    // Cache reference to image in Awake
    private Image image;
    private Vector3 originalPos;
    private Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        
        // Store original position of the image
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    // Monster class reference
    public Monster Monster { get; set; }

    // Function that will create a Monster with _base and level 
    public void Setup(Monster monster)
    {
        // Set Monster (reference created above) to Monster constructor 
        Monster = monster;
        
        // If it's a Player Unit, set image sprite of Player Unit (The back-facing one) with a monster sprite
        if (isPlayerUnit)
        {
            // Using cached Image declared above
            image.sprite = Monster.Base.BackSprite;
        }
        else
        {
            image.sprite = Monster.Base.FrontSprite;
        }

        hud.SetData(monster);
        
        image.color = originalColor;
        PlayEnterAnimation();
    }
    
    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500, originalPos.y);

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.grey, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}


