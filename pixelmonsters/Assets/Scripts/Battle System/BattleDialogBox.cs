using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
  // Reference to dialogue text
  [SerializeField] private Text dialogText;
  
  // Letters to type out per second
  [SerializeField] private int lettersPerSecond;
  
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
      yield return new WaitForSeconds(1f / lettersPerSecond);
    }
  }
}
