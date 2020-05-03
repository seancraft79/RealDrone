using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTarget : MonoBehaviour
{
    public Transform target;
    private bool _istargetNotNull;

    private void Awake()
    {
        _istargetNotNull = target != null;
    }

    void Update()
    {
        if(_istargetNotNull) transform.LookAt(target);
    }
}
