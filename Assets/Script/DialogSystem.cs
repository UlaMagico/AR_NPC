using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    //some path------------------------------
    //all dialog should be put here(can have dir(Normal,Task or else) here)
    string sourceTextDir = "Assets/Resources/DialogTxt";

    //Dialogblock----------------------------
    public GameObject dialogBlock;

    //child of dialogBlock-------------------
    public Image speakerImg;
    public TMP_Text nameText;
    public TMP_Text dialogText;

    public Button rightBtn;
    //---------------------------------------

    private bool isTalking = false;
    private string[] sourceTexts = { "Hello", "Great to see you!" };
    private int lineIndex = 0;

    //Dialog control============================================================
    //產生對話
    public void generateDialog(ScriptableNPC NPC, string textSourcePath)
    {
        if (isTalking) return;

        loadSourceTexts(textSourcePath);
        lineIndex = 0;

        speakerImg.sprite = NPC.sprite;
        nameText.text = NPC.NPCName;
        dialogText.text = sourceTexts[0];

        if (lineIndex == sourceTexts.Length - 1)
        {
            setBtn(rightBtn, "Close", closeDialog);
        }
        else
        {
            setBtn(rightBtn, "Next", nextDialog);
        }

        dialogBlock.SetActive(true);

        isTalking = true;
    }

    public void nextDialog()
    {
        lineIndex++;
        string txt = sourceTexts[lineIndex];
        dialogText.text = txt;

        //last line
        if (lineIndex == sourceTexts.Length - 1)
        {
            setBtn(rightBtn, "Close", closeDialog);
        }
        
        Debug.Log(txt);
    }

    public void closeDialog()
    {
        if (!isTalking) return;

        dialogBlock.SetActive(false);

        isTalking = false;
    }

    //Other useful function=====================================
    //type "Action" is function doesn't have return value
    public void setBtn(Button targetBtn, string text, Action func)
    {
        targetBtn.GetComponentInChildren<TMP_Text>().text = text;
        targetBtn.onClick.RemoveAllListeners();
        targetBtn.onClick.AddListener(() => { func(); });
    }
    
    public void loadSourceTexts(string textSourcePath)
    {
        sourceTexts = File.ReadAllLines(Path.Combine(sourceTextDir, textSourcePath));
    }
}