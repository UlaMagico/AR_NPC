using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AR_GameManager : MonoBehaviour
{
    //some path-----------------------------------------
    readonly string ARGameObjectInfodir = "AR_GameObjectInfo"; //for reading info like location or isCollected
    readonly string ARGameObjectDir = "AR_GameObject";

    //for AR and location--------------------------------
    public double visiableDistance = 0.02; //收集物件的可見範圍(km)
    public double destroyDistance = 0.03; //收集物件超過此範圍(km)則移除
    public PlayerLocationManager playerLocationManager;
    private LocationCalculator locationCalculator = new LocationCalculator();
    public Vector2 playerLocation;
    public Camera mainCamera;

    //NPC info --------------------------------
    private string NPCName = "Black bear";
    private string NPCNameInFile = "black_bear";
    [SerializeField] private GameObject NPCPrefab;
    private Vector2 NPCLocation = new(1, 1); //NPC location in real world
    private int taskCnt = 1;
    private string conversationTextPath = "Conversation/black_bear_Q1";

    // for conversation----------------------------
    private ConversationSystem conversationSystem = new ConversationSystem();

    //in AR---------------------------------------
    private GameObject spawnedNPC;
    private bool isVisible = false;
    public Vector3 spawnPoint = new(2, 0, 2);

    //for UI ----------------------------------------------
    public TMP_Text ARGameInfoText; //給player看的info
    public Button actButton;
    int actButtonMod = 1;
    /*
    與NPC互動的button
    actButtonMod
    1.Call: 將NPC呼叫至面前
    2.Talk: 與NPC交談
    */

    //TODO:可能要搞個對話系統//
    public GameObject conversationBlock;

    [System.Obsolete]
    void Start()
    {
        playerLocation = playerLocationManager.GetPlayerLocation();
        string path;
        if (taskCnt>0)
        {
            path = ARGameObjectDir + "/NPC/Quest_" + NPCNameInFile;        
        }
        else
        {
            path = ARGameObjectDir + "/NPC/" + NPCNameInFile;
        }

        if(NPCPrefab == null) Debug.Log("cannot load NPC prefab: \"" + path + "\"");
        
        NPCPrefab = Resources.Load<GameObject>(path);
        updateActButton();
    }

    [System.Obsolete]
    void Update()
    {
        //location--------------------------------------------
        playerLocation = playerLocationManager.GetPlayerLocation();

        double distance = locationCalculator.distance(
            playerLocation[0], playerLocation[1],
            NPCLocation[0], NPCLocation[1]
        );

        isVisible = distance < visiableDistance;

        //set AR NPC act---------------------------------------------
        actButtonMod = 1; //default is call
        string ARGameInfo = "";
        if (NPCPrefab != null)
        {
            if (isVisible)
            {
                ARGameInfo = "There's NPC nearby you!\n"
                        + "Try to call for NPC!";
                if (spawnedNPC == null)
                {
                    spawnGameObject();
                }
                else if (isNPCInCamera())
                {
                    ARGameInfo = "NPC is in front of you!\n"
                               + "Try to talk to NPC!";
                    actButtonMod = 2;
                }
            }
            else if (spawnedNPC != null && distance > destroyDistance)
            {
                destroyGameObject();
                ARGameInfo = "There's NPC in this area\n"
                           + "It's " + (distance * 1000).ToString("f2") + "m away from you!";
            }
            else
            {
                ARGameInfo = "There's NPC in this area\n"
                           + "It's " + (distance * 1000).ToString("f2") + "m away from you!";
            }
            Debug.Log("Distance: " + distance);

        }
        else
        {
            ARGameInfo = "No NPC in this area";
        }

        ARGameInfoText.text = ARGameInfo;
        updateActButton();
    }

    void spawnGameObject()
    {
        if (spawnedNPC == null)
        {
            spawnedNPC = Instantiate(NPCPrefab, spawnPoint, new Quaternion(0, 90, 0, 0));
        }
    }

    void destroyGameObject()
    {
        if (spawnedNPC != null)
        {
            Destroy(spawnedNPC);
            spawnedNPC = null;
        }
    }

    //用來切換目前準備生成的NPC或收集物件
    void setNPCPrefab(GameObject gameObject)
    {
        destroyGameObject();
        NPCPrefab = gameObject;
    }

    //將NPC呼叫(移動)至面前，預防生成再奇怪的地方
    public void callNPC()
    {
        if (isVisible && spawnedNPC != null)
        {
            //面相玩家
            float targetRotation = mainCamera.transform.eulerAngles.y;
            mainCamera.transform.eulerAngles = new Vector3(0, targetRotation);

            //在玩家前面
            Quaternion rotation = Quaternion.Euler(0, targetRotation, 0);
            float offset = 1; //前面1個單位距離
            Vector3 pos = mainCamera.transform.position
                        + rotation * new Vector3(0, 0, offset);
            spawnedNPC.transform.position = pos;
        }
    }

    [System.Obsolete]
    public void talkToNPC()
    {
        if (!conversationBlock.active) conversationBlock.SetActive(true);
        else conversationBlock.SetActive(false);
    }

    //判斷NPC是否在相機範圍內
    public bool isNPCInCamera()
    {
        if (spawnedNPC != null)
        {
            Bounds bounds = spawnedNPC.GetComponent<Renderer>().bounds;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }
        else return false;
    }

    [System.Obsolete]
    public void updateActButton()
    {
        actButton.onClick.RemoveAllListeners();
        switch (actButtonMod)
        {
            case 1:
                actButton.GetComponentInChildren<TMP_Text>().text = "Call";
                actButton.onClick.AddListener(callNPC);
                break;
            case 2:
                actButton.GetComponentInChildren<TMP_Text>().text = "Talk";
                actButton.onClick.AddListener(talkToNPC);
                break;
        }
    }
}
