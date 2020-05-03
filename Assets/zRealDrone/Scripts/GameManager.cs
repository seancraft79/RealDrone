using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class GameManager : SingletonPattern<GameManager>
{
    readonly string TAG = "[GameManager]";

    public string DroneType { get; set; }
    public bool IsFromGameToLobby { get; set; } = false;
    
    protected override void Init()
    {
        base.Init();
		Debug.Log("GameManager ... Init");
	}

}