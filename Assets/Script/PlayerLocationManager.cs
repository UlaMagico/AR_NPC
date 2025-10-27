using System.Collections;                                    
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Android;

public class PlayerLocationManager : MonoBehaviour
{
    public TMP_Text debugTxt;
    public bool gps_ok = false;
    public bool debugMod = false;
    Vector2 currLoc = new(-1, -1);
    float horizontalAccuracy = -1;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // Request permission
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled on device or app does not have permission to access location");
            debugTxt.text = "Location not enabled on device or app does not have permission to access location";
        }


        // Starts the location service.
        Input.location.Start();
        debugTxt.text = "Waiting for permission";

        // Waits until the location service initializes
        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            debugTxt.text = "Waiting for permission: " + maxWait;
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in time this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            debugTxt.text += "\nTimed Out";
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            debugTxt.text += "\nUnable to determine device location";
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            Debug.Log("Location:\n" + Input.location.lastData.latitude +
                    "\n" + Input.location.lastData.longitude +
                    "\n" + Input.location.lastData.altitude +
                    "\n" + Input.location.lastData.horizontalAccuracy +
                    "\n" + Input.location.lastData.timestamp);

            debugTxt.text
                = "Location:"
                + "\nLatitude: " + Input.location.lastData.latitude
                + "\nLongitude: " + Input.location.lastData.longitude
                + "\nHorizontalAccuracy: " + Input.location.lastData.horizontalAccuracy
                + "\nTime: " + Input.location.lastData.timestamp;

            gps_ok = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gps_ok && !debugMod)
        {
            debugTxt.text = "GPS:...";
            currLoc[0] = Input.location.lastData.latitude;
            currLoc[1] = Input.location.lastData.longitude;
            horizontalAccuracy = Input.location.lastData.horizontalAccuracy;
        }
    }

    void OnDestroy()
    {
        StopGPS();
    }

    public void StopGPS()
    {
        if (Input.location.status == LocationServiceStatus.Running)
            Input.location.Stop();
    }

    public Vector2 GetPlayerLocation()
    {
        return currLoc;
    }

    public double GetPlayerLat()
    {
        return currLoc[0];
    }
    
    public float GetPlayerLon()
    {
        return currLoc[1];
    }

    public void SetPlayerLat(float lat)
    {
        currLoc[0] = lat;
    }

    public void SetPlayerLon(float lon)
    {
        currLoc[1] = lon;
    }

    public void SetDebugMode(bool b)
    {
        debugMod = b;
    }
}