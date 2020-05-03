using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneArrowTween : MonoBehaviour
{
    public GameObject arrow;
    bool visible = true;

    private void Start()
    {
        StartCoroutine(FlickerTween());
    }

    IEnumerator FlickerTween()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            visible = !visible;
            arrow.SetActive(visible);
        }
    }

}
