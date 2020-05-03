using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrone : MonoBehaviour
{

    public Transform drone;

    public void Reset()
    {
        drone.GetComponent<DroneController>()?.Reset();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
