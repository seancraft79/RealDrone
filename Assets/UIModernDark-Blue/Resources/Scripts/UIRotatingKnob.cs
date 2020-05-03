//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIRotatingKnob : UIElement
{ 
	public UIRotatingKnob(UIElement parent) : base(parent, "RotatingKnob")
	{ 	
	}
	
	public void OnValueChanged(UnityAction<float> callback)
	{
		GetObject().GetComponentInChildren<RotatingKnob>().onValueChanged.AddListener(callback);
	} 
	
}
