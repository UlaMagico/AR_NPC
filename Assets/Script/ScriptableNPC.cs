using System.IO;
using UnityEngine;
using UnityEngine.Rendering;


//this can create different NPC and store as asset
[CreateAssetMenu(fileName = "ScriptableNPC", menuName = "NPC/ScriptableNPC")]
public class ScriptableNPC : ScriptableObject
{

    public string NPCName; //顯示的名字
    public string nameInFile; //在檔案上的名字
    public Vector2 location; //location in real world
    public Sprite sprite;
    public GameObject prefab;
    GameObject taskSign; 
    //頭上的任務符號或特殊標示,child of prefab
    public int taskCnt = 0;

    public ScriptableNPC()
    {
        if (prefab != null) taskSign = prefab.transform.Find("task_sign").gameObject;
    }

    public void showTaskSign()
    {
        taskSign.SetActive(true);
    }

    public void hideTaskSign()
    {
        taskSign.SetActive(false);
    }
}