//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UITextInputField : UIElement
{ 
	public UITextInputField(UIElement parent, string text="") : base(parent, "TextInputField")
	{ 	
		//SetText(text);   // Bug, see below
	}
	
	public void SetText(string text)
	{
		//InputField input = GetObject().GetComponent<InputField>();
		//input.text = text; // Bug in 4.6.1 gives ArgumentOutOfRangeException
	}

	public void onEndEdit(UnityAction<string> callback)
	{
		GetObject().GetComponent<InputField>().onEndEdit.AddListener(callback);
	}
	
}
