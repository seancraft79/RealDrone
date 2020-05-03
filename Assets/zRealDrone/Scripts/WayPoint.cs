using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public int index;
    public int totalCount;
    private bool isEnabled = true;
    public bool IsEnabled
    {
        get { return isEnabled; }
    }
    [SerializeField] private GameObject wayPointObj;
    [SerializeField] private ParticleSystem particle;

    public void OnEnableWayPoint()
    {
        // Debug.Log($"{gameObject.name} OnEnableWayPoint");
        wayPointObj.gameObject.SetActive(true);
        particle.gameObject.SetActive(false);
        isEnabled = true;
    }
    
    public void OnDisableWayPoint()
    {
        // Debug.Log($"{gameObject.name} OnDisableWayPoint");
        wayPointObj.gameObject.SetActive(false);
        particle.gameObject.SetActive(true);
        isEnabled = false;
    }
}
