using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTarget, lookTarget;
    public Transform fallingTarget;

    public float smoothSpeed = 0.09f; // 0.125f;
    public Vector3 offset;
    public bool ShouldFollow { get; set; } = true;

    public void CamUpdate()
    {
        if (ShouldFollow)
        {
            Vector3 desiredPosition = followTarget.position + offset;
            float speed = 2 * Time.deltaTime;
            //Debug.Log($"speed : {speed}, time : {Time.deltaTime}");
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, speed);
            transform.position = smoothedPosition;

            transform.LookAt(lookTarget);
        }
        else
        {
            float back = 30f * Time.deltaTime * -1;
            transform.position += new Vector3(0, 0, back);
            transform.LookAt(fallingTarget);
        }
    }

}
