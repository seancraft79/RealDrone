//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UISlider : UIElement
{ 
	public enum Type {Horizontal, Vertical}; 
	public enum Style {Plain, Labelled, LabelTab}; 

	public UISlider(UIElement parent, Type type, Style style=Style.Labelled, string elemName="") : base(parent)
	{ 	
		string prefabName = "Slider"+type+style;
		if (elemName == "") {
			elemName = prefabName;
		}
		LoadPrefab(parent, prefabName, elemName);
		
		if (style == Style.Labelled || style == Style.LabelTab) {
			GetObject().GetComponentInChildren<Slider>().onValueChanged.AddListener(UpdateLabel);
		}
	}
	
	protected void SetLabel(string text, int fontSize=14)
	{
		Text t = GetObject().GetComponentInChildren<Text>();
		t.text = text;
		t.fontSize = fontSize;
	}

	protected void UpdateLabel(float value)
	{
		SetLabel(Mathf.RoundToInt(value*100)+"%");
	}
	
	public void SetValue(float value)
	{
		GetObject().GetComponentInChildren<Slider>().value = value;	
	}

	public void OnValueChanged(UnityAction<float> callback)
	{
		GetObject().GetComponentInChildren<Slider>().onValueChanged.AddListener(callback);
	} 
}
