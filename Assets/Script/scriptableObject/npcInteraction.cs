using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class npcInteraction : MonoBehaviour, Interacable
{
    [Header("Dialog Data")]
    public NPCdialog dialogData;           // ScriptableObject untuk dialog
    public string questToGiveID;           // Kosongkan jika tidak ada quest

    [Header("Dialog UI")]
    public GameObject dialogPanel;
    public TMP_Text dialogText, nameText;
    public Image portraitImage;

    private int dialogIndex;
    private bool isTyping, isDialogActive;

    [Header("Quest UI")]
    public GameObject pQuest;      // GameObject yang ada skrip QuestUI-nya
    public QuestUI questUI;        // Skrip QuestUI-nya


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

        nameText.SetText(dialogData.npcName);
        portraitImage.sprite = dialogData.npcPortrait;

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
            dialogIndex++;
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialog();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogText.SetText("");

        foreach (char letter in dialogData.dialogueLines[dialogIndex])
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

    void EndDialog()
{
    StopAllCoroutines();
    isDialogActive = false;
    dialogText.SetText("");
    dialogPanel.SetActive(false);
    PauseController.SetPause(false);

    if (!string.IsNullOrEmpty(questToGiveID))
    {
        QuestManager.Instance.StartQuest(questToGiveID);

        // Aktifkan panel quest dan refresh
        if (pQuest != null) pQuest.SetActive(true);
        if (questUI != null) questUI.RefreshQuestList();
    }
}


}
