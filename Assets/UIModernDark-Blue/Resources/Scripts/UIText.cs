//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

public class UIText : UIElement
{ 
	public UIText(UIElement parent, string text="", int fontSize=14) : base(parent, "Text")
	{ 	
		SetText(text, fontSize);
	}
	
	public void SetText(string text, int fontSize=-1)
	{
		Text t = GetObject().GetComponent<Text>();
		t.text = text;
		if (fontSize > 0) {
			t.fontSize = fontSize;
		}
	}

	
	public void SetAlignment(TextAnchor alignment)
	{
		Text t = GetObject().GetComponent<Text>();
		t.alignment = alignment;
	}	
}
