using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using UnityEngine;

public class ModalManager : MonoBehaviour
{
    public const string OK = "ok";
    public const string CANCEL = "cancel";

    public ModalWindowManager modalWindowManager;

    private Action<string> callBack;

    public void SetListener(Action<string> cb)
    {
        callBack = cb;
    }

    public void OnOk()
    {
        callBack?.Invoke(OK);
        OnClose();
    }

    public void OnCancel()
    {
        callBack?.Invoke(CANCEL);
        OnClose();
    }

    private void OnClose()
    {
        modalWindowManager.CloseWindow();
    }

    public void ShowModal(string title, string message)
    {
        ShowModal(title, message, null);
    }
    
    public void ShowModal(string title, string message, Action<string> listener)
    {
        modalWindowManager.OpenWindow();
        modalWindowManager.titleText = title;
        modalWindowManager.descriptionText = message;
        modalWindowManager.UpdateUI();
        SetListener(listener);
    }
}
