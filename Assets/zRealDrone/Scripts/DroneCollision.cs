using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneCollision : MonoBehaviour
{
    Action collListener;
    bool canListen = true;

    public void SetListener(Action listener)
    {
        collListener = listener;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canListen) return;
        Debug.Log($"{gameObject.name} OnCollision by {collision.gameObject.name}, Tag : {collision.collider.tag.ToString()}");
        if(collision.collider.CompareTag("Floor"))
        {
            canListen = false;
            if (collListener != null) collListener();
        }
    }

    IEnumerator DestroyObjectDelayed()
    {
        canListen = false;
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
