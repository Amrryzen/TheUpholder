using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class npcInteraction : MonoBehaviour, Interacable
{
    public npcDialog dialogData;
    public GameObject dialogPanel;
    public TMP_Text dialogText, nameText;
    public Image potraitImage;

    private int dialogIndex;
    private bool isTyping, isDialogActive;

    public bool canInteract()
    {
        return !isDialogActive;
    }

    public void Interact()
    {
        if (dialogData == null || (PauseController.IsGamePaused && !isDialogActive))
            return;
        if (isDialogActive) 
        { 
            NextLine();
        }
        else
        {
           StartDialog();
        }
    }

    void StartDialog()
    {
        isDialogActive = true;
        dialogIndex = 0;

        nameText.SetText(dialogData.name);
        potraitImage.sprite = dialogData.npcPortrait;

        dialogPanel.SetActive(true);
        PauseController.SetPause(true);

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogText.SetText(dialogData.dialogueLines[dialogIndex]);
            isTyping = false;
        }
        else if (dialogIndex + 1 < dialogData.dialogueLines.Length) 
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            //end
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogText.SetText("");

        foreach(char letter in dialogData.dialogueLines[dialogIndex])
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(dialogData.typingSpeed);
        }

        isTyping = false;
        if (dialogData.autoProgressLines.Length > dialogIndex && dialogData.autoProgressLines[dialogIndex])
        {
            yield return new WaitForSeconds(dialogData.autoProgressDelay);
            NextLine();
        }
         
    }
    public void EndDialog()
    {
        StopAllCoroutines();
        isDialogActive = false;
        dialogText.SetText("");
        dialogPanel.SetActive(false);
        PauseController.SetPause(false);
    }
}
