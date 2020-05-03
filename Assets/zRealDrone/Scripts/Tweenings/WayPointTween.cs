using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WayPointTween : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveY(2f, 1).From(true).SetLoops(-1, LoopType.Yoyo);
    }

}
