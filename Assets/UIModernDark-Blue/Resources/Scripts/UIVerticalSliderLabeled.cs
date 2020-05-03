//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIVerticalSliderLabeled : UISliderBase
{ 
	public UIVerticalSliderLabeled(UIElement parent) : base(parent, "Vertical Slider Labeled")
	{ 	
		GetObject().GetComponentInChildren<Slider>().onValueChanged.AddListener(UpdateLabel);
	}
}
