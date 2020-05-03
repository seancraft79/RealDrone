//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIButton : UIElement
{ 
	public enum Type {Rectangle, Round, RoundRectangle}; 

	public UIButton(UIElement parent, Type type, string elemName="") : base(parent)
	{ 	
		string prefabName = "Button"+type;
		LoadPrefab(parent, prefabName, elemName);
	}
	
	public void SetText(string text, int fontSize=14)
	{
		Text t = GetObject().GetComponentInChildren<Text>();
		t.text = text;
		t.fontSize = fontSize;
	}

	public void OnValueChanged(UnityAction callback)
	{
		GetObject().GetComponentInChildren<Button>().onClick.AddListener(callback);
	}
}
