using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
  [SerializeField] private int lettersPerSecond;

  // References
  [SerializeField] private Text dialogText;
  [SerializeField] private GameObject actionSelector;
  [SerializeField] private GameObject moveSelector;
  [SerializeField] private GameObject moveDetails;

  [SerializeField] private List<Text> actionTexts;
  [SerializeField] private List<Text> moveTexts;

  [SerializeField] private Text ppText;
  [SerializeField] private Text typeText;

  private Color highlightedColor;

  private void Start()
  {
    highlightedColor = GlobalSettings.i.HighlightedColor;
  }

  // Set dialog to dialog text
  public void SetDialog(string dialog)
  {
    dialogText.text = dialog;
  }
  
  // Typing effect for dialog
  public IEnumerator TypeDialog(string dialog)
  {
    dialogText.text = "";
    foreach (char letter in dialog.ToCharArray())
    {
      dialogText.text += letter;
      yield return new WaitForSeconds(1f/lettersPerSecond);
    }
  }
  
  // Enable/Disable Selectors
  public void EnableDialogText(bool enabled)
  {
    dialogText.enabled = enabled;
  }
  
  public void EnableActionSelector(bool enabled)
  {
    actionSelector.SetActive(enabled);
  }
  
  public void EnableMoveSelector(bool enabled)
  {
    moveSelector.SetActive(enabled);
    moveDetails.SetActive(enabled);
  }
  
  public void UpdateActionSelection(int selectedAction)
  {
    for (int i=0; i<actionTexts.Count; ++i)
    {
      if (i == selectedAction)
        actionTexts[i].color = highlightedColor;
      else
        actionTexts[i].color = Color.black;
    }
  }

  public void UpdateMoveSelection(int selectedMove, Move move)
  {
    for (int i = 0; i < moveTexts.Count; i++)
    {
      if (i == selectedMove)
        moveTexts[i].color = highlightedColor;
      else
        moveTexts[i].color = Color.black;
    }

    ppText.text = $"PP {move.PP}/{move.Base.PP}";
    typeText.text = move.Base.Type.ToString();
    
    if(move.PP == 0)
      ppText.color = Color.red;
    else
      ppText.color = Color.black;
  }

  public void SetMoveNames(List<Move> moves)
  {
    // Look through all the moves list
    for (int i = 0; i < moveTexts.Count; i++)
    {
      // Some monsters might have less that 4 moves, so check
      if (i < moves.Count)
        moveTexts[i].text = moves[i].Base.Name;
      else
        moveTexts[i].text = " - ";
    }
  }
}