//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIToggle : UIElement
{ 
	public enum Type {Square, Round, RoundRectangle}; 

	public UIToggle(UIElement parent, Type type, string elemName="") : base(parent)
	{ 	
		string prefabName = "Toggle"+type;
		LoadPrefab(parent, prefabName, elemName); 
		SetText(elemName);
		SetState(isOn: false);
	}
	
	public void SetText(string text, int fontSize=14)
	{
		Text t = GetObject().GetComponentInChildren<Text>();
		t.text = text;
		t.fontSize = fontSize;
	}     
	     
	public void SetState(bool isOn)
	{
		GetObject().GetComponent<Toggle>().isOn = isOn;
	}

	public void OnValueChanged(UnityAction<bool> callback)
	{
		GetObject().GetComponent<Toggle>().onValueChanged.AddListener(callback);
	}
	
}
