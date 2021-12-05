using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
  [SerializeField] private int lettersPerSecond;
  [SerializeField] private Color highlightedColor;
  
  // References
  [SerializeField] private Text dialogText;
  [SerializeField] private GameObject actionSelector;
  [SerializeField] private GameObject moveSelector;
  [SerializeField] private GameObject moveDetails;

  [SerializeField] private List<Text> actionTexts;
  [SerializeField] private List<Text> moveTexts;

  [SerializeField] private Text ppText;
  [SerializeField] private Text typeText;

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
}