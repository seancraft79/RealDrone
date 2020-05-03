using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using Michsky.UI.ModernUIPack;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DroneLevel1Controller : MonoBehaviour
{
    public DroneController drone;
    public TextMeshProUGUI txtDistance, txtBattery, txtHeight, txtYaw, txtTarget;
    public GameObject warningPanel, successPanel, batteryWarning, altitudeWarning;
    
    // Loding panel
    public GameObject loadingPanel;
    public TextMeshProUGUI txtLoading;
    
    // Dialog
    public ModalManager modalManager;
    public GameObject joystickCanvas;
    
    // Waypoints
    public WayPoint[] waypoints;
    public Transform droneWpArrow;
    private int wayPointAcquired = 0;

    // Elapse time
    public TextMeshProUGUI txtElapseTime;
    private double elapseMilliSec = 0;
    private double elapseSec = 0, elapseMin = 0, elapseMilli = 0;
    private string strElapseMin, strElapseSec, strElapseMilli = "00";
    private bool isElapseTime = false;
    
    private float timeSec = 0;

    private AudioSource audioSource;
    private Transform droneArrowLookTarget;
    private bool isDroneArrowLookTarget = false;

    public bool isGame = false;

    private void Awake()
    {
        GoogleAdsManager.Instance.ToggleBanner(false);
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GoogleAdsManager.Instance.RequestInterstitial();

        Debug.Log($"DroneLevelController ... Start droneType: {GameManager.Instance.DroneType}");
        ResetGame();
    }

    public void ResetGame()
    {
        StartCoroutine(ResetGameController());
    }

    IEnumerator ResetGameController()
    {
        Debug.Log("ResetGame ...");
        
        yield return new WaitForSeconds(3f);
        
        Debug.Log("ResetGame ... start");
        isGame = true;
        drone.isGame = true;
        
        wayPointAcquired = 0;
        drone.AddHitObjListener(OnDroneCollision);
        Debug.Log("Start waypoints: " + waypoints.Length);
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i].OnEnableWayPoint();
        }
        txtTarget.SetText(wayPointAcquired + "/" + waypoints.Length);
        
        SetDroneLookTarget();
        
        warningPanel.SetActive(false);
        successPanel.SetActive(false);
        
        drone.Reset();
        
        elapseMin = 0;
        elapseSec = 0;
        elapseMilli = 0;
        elapseMilliSec = 0;
        if (txtElapseTime != null)
        {
            isElapseTime = true;
        }
    }

    void OnDroneCollision(ControllerColliderHit hit)
    {
        Debug.Log("-------------------");
        Debug.Log("OnDroneCollision : " + hit.collider.transform.parent.name);
        if(hit.collider.CompareTag("WayPointItem"))
        {
            var wayPoint = hit.collider.transform.parent.GetComponent<WayPoint>();
            if(wayPoint != null) wayPoint.OnDisableWayPoint();
            else Debug.LogError("Hit Waypoint null");
            
            wayPointAcquired = GetWaypointAcquired();
            Debug.Log($"wayPointAcquired count: {wayPointAcquired}");
            txtTarget.SetText(wayPointAcquired + "/" + waypoints.Length);
            if (wayPointAcquired > waypoints.Length - 1)
            {
                drone.OnDroneSuccess();
                OnSuccessGame();

                isDroneArrowLookTarget = false;
                droneWpArrow.gameObject.SetActive(false);
            }
            else
            {
                SetDroneLookTarget();
                drone.OnDroneWayPoint();
            }
        } else if (hit.collider.CompareTag("Final"))
        {
            Debug.Log("Final");
        }
    }

    void SetDroneLookTarget()
    {
        droneArrowLookTarget = GetDroneArrowLookTarget();
        isDroneArrowLookTarget = droneArrowLookTarget != null;
    }

    int GetWaypointAcquired()
    {
        int count = 0;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (!waypoints[i].IsEnabled) count++;
        }

        return count;
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (isElapseTime) ElapseTime();

#if UNITY_ANDROID
        if (Input.GetKey(KeyCode.Escape))
        {
            OnBackBtnClicked();
        }
#endif
    }

    private void ElapseTime()
    {
        elapseMilliSec += Time.deltaTime;
        if (elapseMilliSec >= 1)
        {
            elapseMilliSec = 0;
            elapseSec++;
        }
        if (elapseSec > 59)
        {
            elapseSec = 0;
            elapseMin++;
        }

        if (elapseMin > 60)
        {
            elapseMin = 0;
        }

        elapseMilli = Math.Truncate(elapseMilliSec * 100);

        strElapseMin = "" + elapseMin;
        if (elapseMin < 10) strElapseMin = "0" + elapseMin;

        strElapseSec = "" + elapseSec;
        if (elapseSec < 10) strElapseSec = "0" + elapseSec;

        strElapseMilli = "" + elapseMilli;
        if (elapseMilli < 10) strElapseMilli = "0" + elapseMilli;

        txtElapseTime.SetText(strElapseMin + ":" + strElapseSec + ":" + strElapseMilli);  
    }

    private void FixedUpdate()
    {
        if (!isGame) return;
        
        txtDistance.text = drone.DroneDistanceFromHome + " m";
        txtHeight.text = drone.DroneHeight + " m";
        txtYaw.text = drone.DroneYaw + "";
        txtBattery.text = drone.DroneBattery + " %";
        // Battery warging
        if (drone.DroneState == DroneController.DRONE_STATE_FLYING)
        {
            if (drone.DroneBattery < DroneController.BATTERY_WARNING || drone.IsHeightLimitExeed)
            {
                timeSec += Time.deltaTime;
                // batteryWarningPanel
                if ((int)timeSec % 2 == 0)
                {
                    warningPanel.SetActive(true);
                }
                else warningPanel.SetActive(false);
            }
            else
            {
                if (warningPanel.activeSelf) warningPanel.SetActive(false);
                timeSec = 0;
            }

            // Battery warning
            if (drone.DroneBattery < DroneController.BATTERY_WARNING)
            {
                batteryWarning.SetActive(true);
            } else batteryWarning.SetActive(false);

            // Height warning
            if (drone.IsHeightLimitExeed)
            {
                altitudeWarning.SetActive(true);
            } else altitudeWarning.SetActive(false);
        }
        
        else
        {
            if (warningPanel.activeSelf) warningPanel.SetActive(false);
            timeSec = 0;
        }
        
        if(isDroneArrowLookTarget) droneWpArrow.LookAt(droneArrowLookTarget);
    }
    
    private void SetWaypoint()
    {
        var wayPointIndex = GetWayVisibleWaypointIndex();
        if(wayPointIndex > -1) droneWpArrow.LookAt(waypoints[wayPointIndex].transform);
        else droneWpArrow.gameObject.SetActive(false);
    }

    Transform GetDroneArrowLookTarget()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i].IsEnabled)
                return waypoints[i].transform;
        }

        return null;
    }
    
    int GetWayVisibleWaypointIndex()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i].IsEnabled)
                return i;
        }

        return -1;
    }

    public Button btnSoundToggle;
    public Sprite sprSoundOn, sprSoundOff;
    private bool isSoundMute = false;
    public void OnSoundToggle()
    {
        isSoundMute = !isSoundMute;
        drone.OnSoundToggle(isSoundMute);
        audioSource.mute = isSoundMute;
        if(isSoundMute)
        {
            btnSoundToggle.GetComponent<Image>().sprite = sprSoundOff;
        }
        else btnSoundToggle.GetComponent<Image>().sprite = sprSoundOn;
    }

    public void OnSuccessGame()
    {
        successPanel.SetActive(true);
    }

    public void OnBackBtnClicked()
    {
        joystickCanvas.SetActive(false);
        modalManager.ShowModal("RealDrone", "Go back to lobby?", result =>
        {
            if (result.Equals(ModalManager.OK))
            {
                StartCoroutine(LoadSceneAsync("Lobby"));
            }
            else joystickCanvas.SetActive(true);
        });
        
    }
    
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        Debug.Log("DroneLevelController Interstitial OnAdClosed");
        
    }

    /**
     * Button go back to Lobby
     */
    public void OnLobbyClicked()
    {
        StartCoroutine(LoadSceneAsync("Lobby"));
    }

    /**
     * Button retry
     */
    public void OnRetryClicked()
    {
        // Reset game
        warningPanel.SetActive(false);
        successPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    IEnumerator LoadSceneAsync(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        GameManager.Instance.IsFromGameToLobby = true;
        isGame = false;
        audioSource.mute = true;
        drone.OnSoundToggle(true);
        loadingPanel.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);    

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            var progress = Math.Round(asyncLoad.progress, 2);
            // Debug.Log($"sceneLoading: {progress}");
            txtLoading.SetText(progress + "%");
            yield return null;
        }
    }
}
