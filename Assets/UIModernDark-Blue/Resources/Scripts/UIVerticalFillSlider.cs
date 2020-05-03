//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIVerticalFillSlider : UISliderBase
{ 
	public UIVerticalFillSlider(UIElement parent) : base(parent, "Vertical Fill Slider Labeled")
	{ 	
		GetObject().GetComponentInChildren<Slider>().onValueChanged.AddListener(UpdateLabel);
	}
}
