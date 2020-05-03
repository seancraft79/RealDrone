using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class GameController_Training : MonoBehaviour
{
    public Transform[] waypoints;
    public Transform droneWpArrow;
    public DroneController drone;
    private int waypointCount = 0;
    public DroneLevel1Controller Level1Controller;

    private void Start()
    {
        drone.AddHitObjListener(OnDroneCollision); 
        Debug.Log("GameController_Training start waypoints: " + waypoints.Length);
    }

    void OnDroneCollision(ControllerColliderHit controllerColliderHit)
    {
        Debug.Log("OnDroneCollision : " + controllerColliderHit.collider.name);
        if(controllerColliderHit.collider.CompareTag("WayPointItem"))
        {
            controllerColliderHit.collider.gameObject.SetActive(false);
            
            waypointCount++;
            Debug.Log($"wayPointCount: {waypointCount}");
            if (waypointCount > waypoints.Length - 1)
            {
                drone.OnDroneSuccess();
                Level1Controller.OnSuccessGame();
            } else drone.OnDroneWayPoint();
        }
    }

    private void FixedUpdate()
    {
        var wayPointIndex = GetWayVisibleWaypointIndex();
        if(wayPointIndex > -1) droneWpArrow.LookAt(waypoints[wayPointIndex]);
        else droneWpArrow.gameObject.SetActive(false);
    }

    int GetWayVisibleWaypointIndex()
    {
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i].gameObject.activeSelf)
                return i;
        }

        return -1;
    }
}
