using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class AR_GameManager : MonoBehaviour
{
    //for AR and location--------------------------------
    public double visiableDistance = 0.02; //收集物件的可見範圍(km)
    public double destroyDistance = 0.03; //收集物件超過此範圍(km)則移除
    public PlayerLocationManager playerLocationManager;
    private LocationCalculator locationCalculator = new LocationCalculator();
    public Vector2 playerLocation;
    public Camera mainCamera;

    //NPC info --------------------------------
    //TODO:想辦法在特定區域加載NPC,暫時自己放
    public ScriptableNPC NPC;

    // for conversation----------------------------
    [SerializeField] private DialogSystem dialogSystem;

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
        if (NPC.taskCnt > 0)
        {
            //顯示任務符號
            NPC.showTaskSign();
        }
        else
        {
            //不顯示符號
            NPC.hideTaskSign();
        }

        updateActButton();
    }

    [System.Obsolete]
    void Update()
    {
        //location--------------------------------------------
        playerLocation = playerLocationManager.GetPlayerLocation();

        double distance = locationCalculator.distance(
            playerLocation[0], playerLocation[1],
            NPC.location[0], NPC.location[1]
        );

        isVisible = distance < visiableDistance;

        //set AR NPC act---------------------------------------------
        actButtonMod = 1; //default is call
        string ARGameInfo = "";
        if (NPC.prefab != null)
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
        }
        else ARGameInfo = "No NPC in this area";

        ARGameInfoText.text = ARGameInfo;
        updateActButton();
    }

    void spawnGameObject()
    {
        if (spawnedNPC == null)
        {
            spawnedNPC = Instantiate(NPC.prefab, spawnPoint, new Quaternion(0, 90, 0, 0));
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
        /*
        if (!conversationBlock.active) conversationBlock.SetActive(true);
        else conversationBlock.SetActive(false);
        */
        string sourceTextPath = "Task/test_NPC_task_1.txt";
        dialogSystem.generateDialog(NPC, sourceTextPath);
    }

    //判斷NPC是否在相機範圍內
    //TODO:將範圍縮小至中間
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
