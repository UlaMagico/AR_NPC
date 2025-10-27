using UnityEngine;
using UnityEngine.Rendering;

public struct NPCInfo
{
    public string name; //顯示的名字
    public string nameInFile; //在檔案上的名字
    public Vector2 location; //location in real world
}

public class NPC : ScriptableObject
{
    readonly string NPCPrefabDir = "AR_GameObject";

    NPCInfo info;
    GameObject prefab;

    NPC(string name, string nameInFile, Vector2 location)
    {
        info.name = name;
        info.nameInFile = nameInFile;
        info.location = location;

        //prefab = Resource.Load<GameObject>(ARGameObjectDir + nameInFile);
    }
}