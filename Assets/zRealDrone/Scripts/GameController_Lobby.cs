using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

public class GameController_Lobby : MonoBehaviour
{
    public GameObject drone_SG1, drone_SG2;
    public Transform cameraPos_SG1, cameraPos_SG2;
    public Transform mainCamera;
    public GameObject selectScenePanel;
    public TextMeshProUGUI txtDroneTitle;
    public TextMeshProUGUI txtSpeed, txtAlt, txtAccel, txtBattery;
    
    private GameObject currentDrone;
    
    // Sound
    private AudioSource audioSource;
    public AudioClip acClick;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        Debug.Log("GameController_Lobby ... Start");
        SetDrone("drone_SG1");
        drone_SG1.transform.DORotate(new Vector3(drone_SG1.transform.rotation.x, 180, drone_SG1.transform.rotation.z), 5f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
        drone_SG2.transform.DORotate(new Vector3(drone_SG1.transform.rotation.x, 180, drone_SG1.transform.rotation.z), 5f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
        
        // Admob banner
        if (GameManager.Instance.IsFromGameToLobby)
        {
            Debug.Log("Lobby From game show interstitial");
            GoogleAdsManager.Instance.ShowInterstitial();
            GoogleAdsManager.Instance.ToggleBanner(true);
        }
        else
        {
            GoogleAdsManager.Instance.RequestBanner();
            GoogleAdsManager.Instance.RequestInterstitial();
        }
        
        GameManager.Instance.IsFromGameToLobby = false;
    }
    
    private void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKey(KeyCode.Escape))
        {
            GoogleAdsManager.Instance.ShowInterstitial();
            Application.Quit();
        }
#endif
    }

    public void OnArrowLeftClicked()
    {
        audioSource.PlayOneShot(acClick);
        if (!currentDrone.gameObject.name.Equals("drone_SG1"))
        {
            txtDroneTitle.text = "";
            txtBattery.text = "";
            txtAlt.text = "";
            txtSpeed.text = "";
            txtAccel.text = "";
            mainCamera.DOMove(cameraPos_SG1.position, 1f).OnComplete(() => {
                SetDrone("drone_SG1");
            });
        }
    }

    public void OnArrowRightClicked()
    {
        audioSource.PlayOneShot(acClick);
        if (!currentDrone.gameObject.name.Equals("drone_SG2"))
        {
            txtDroneTitle.text = "";
            txtBattery.text = "";
            txtAlt.text = "";
            txtSpeed.text = "";
            txtAccel.text = "";
            mainCamera.DOMove(cameraPos_SG2.position, 1f).OnComplete(() => {
                SetDrone("drone_SG2");
            });
        }
    }

    void SetDrone(string type)
    {
        GameManager.Instance.DroneType = type;
        
        if(type.Equals("drone_SG1"))
        {
            currentDrone = drone_SG1;
            txtBattery.text = "5";
            txtAlt.text = "6";
            txtSpeed.text = "7";
            txtAccel.text = "6";
            txtDroneTitle.text = "SG - 1";
        }
        else
        {
            currentDrone = drone_SG2;
            txtBattery.text = "7";
            txtAlt.text = "5";
            txtSpeed.text = "5";
            txtAccel.text = "4";
            txtDroneTitle.text = "SG - 2";
        }
    }

    public void OnDroneSelected()
    {
        audioSource.PlayOneShot(acClick);
        selectScenePanel.SetActive(true);
    }

    public void OnSelectTraining()
    {
        audioSource.PlayOneShot(acClick);
        // SceneManager.LoadScene("TrainingScene");
        StartCoroutine(LoadSceneAsync("TrainingScene"));
    }

    public void OnSelectRacing()
    {
        audioSource.PlayOneShot(acClick);
        // SceneManager.LoadScene("RacingScene");
        StartCoroutine(LoadSceneAsync("RacingScene"));
    }

    public void OnCancelSelection()
    {
        audioSource.PlayOneShot(acClick);
        selectScenePanel.SetActive(false);
    }
    
    IEnumerator LoadSceneAsync(string sceneName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // Debug.Log($"sceneLoading: {asyncLoad.progress}");
            yield return null;
        }
    }
}
