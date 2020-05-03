using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneObject : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"{gameObject.name} OnCollision {collision.gameObject.name}");
    }
}
