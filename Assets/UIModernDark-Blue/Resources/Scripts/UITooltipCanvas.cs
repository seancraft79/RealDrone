//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UITooltipCanvas : UIElement
{ 
	public UITooltipCanvas(UIElement parent=null, string elemName="") : base()
	{ 	
		string prefabName = "TooltipCanvas";
		LoadPrefab(parent, prefabName, elemName);
	}
	
	public Image GetImage()
	{
		Image image = GetObject().GetComponentInChildren<Image>();
		if (image == null) { 
			throw new System.Exception("Error "+GetObject().name+": Tooltip canvas must have an Image attached.");
		}
		return image; 
	}
}
