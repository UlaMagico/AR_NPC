using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AR_GameDebugger : MonoBehaviour
{
    public GameObject debugPanel;
    public TMP_InputField latInput;
    public TMP_InputField lonInput;
    public GameObject ARCameraOffset;
    public Camera mainCamera;
    public TMP_Text ARCameraInfoText;
    public TMP_Text locationText;

    public PlayerLocationManager playerLocationManager;

    void Start()
    {
        CloseDebugPanel();
    }

    void Update()
    {
        Vector3 vector = ARCameraOffset.transform.position;
        string str = "AR camera offset:\n"
                    + vector.x + " "
                    + vector.y + " "
                    + vector.z + "\n";
        vector = mainCamera.transform.position;
        str +=  "mainCamera:\n"
                    + vector.x + " "
                    + vector.y + " "
                    + vector.z + "\n";
        ARCameraInfoText.text = str;

        str = "Location:"
            + "\nLatitude: " + playerLocationManager.GetPlayerLat().ToString()
            + "\nLongitude: " + playerLocationManager.GetPlayerLon().ToString();
        locationText.text = str;
    }

    public void OpenDebugPanel()
    {
        if (debugPanel != null)
        {
            debugPanel.SetActive(true);
        }
    }
    public void CloseDebugPanel()
    {
        if (debugPanel != null)
        {
            debugPanel.SetActive(false);
        }
    }

    public void SetPlayerLoc()
    {
        float lat = (latInput.text != "")? float.Parse(latInput.text) : 0 ;
        float lon = (latInput.text != "")? float.Parse(lonInput.text) : 0;

        playerLocationManager.SetPlayerLat(lat);
        playerLocationManager.SetPlayerLon(lon);
    }
}
