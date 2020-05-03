//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

public class UIPanel : UIElement
{ 
	public enum Type {Empty, Flat, Thick, Thin}; 
	public enum Border {None, Raised, Sunken, RaisedEtched, SunkenEtched}; 
	
	public UIPanel(UIElement parent, Type type, Border border=Border.None, string elemName="") : base(parent)
	{ 	
		string prefabName = "Panel"+type+border;
		LoadPrefab(parent, prefabName, elemName);
	}
}
