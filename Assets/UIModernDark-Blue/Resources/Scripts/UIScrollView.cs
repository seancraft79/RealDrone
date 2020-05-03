//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

public class UIScrollView : UIElement
{ 
	private UIElement mScrollableContent;
	
	public UIScrollView(UIElement parent, string objName="") : base(null, "ScrollView", objName)
	{ 	
      UIElement scrollParent = new UIPanel(parent, UIPanel.Type.Empty, elemName: "Scroll Parent");
		scrollParent.AddHorizontalLayoutGroup(expandHeight: true);

		SetParent(scrollParent);

      UIElement scrollbarVertical = new UIElement(scrollParent, "ScrollBar Vertical");
      UIElement scrollbarHorizontal = new UIElement(parent, "ScrollBar Horizontal");

		ScrollRect scrollRect = GetObject().GetComponent<ScrollRect>();
		scrollRect.horizontalScrollbar = scrollbarHorizontal.GetObject().GetComponent<Scrollbar>();
		scrollRect.verticalScrollbar = scrollbarVertical.GetObject().GetComponent<Scrollbar>();
	}
	
	public void AddScrollContent(UIElement content)
	{
		ScrollRect scrollRect = GetObject().GetComponent<ScrollRect>();
		scrollRect.content = content.GetObject().transform as RectTransform;
	}
	
	public void SetScrollPosition(float horizontalPos = 0, float verticalPos = 1)
	{
		ScrollRect scrollRect = GetObject().GetComponent<ScrollRect>();
		scrollRect.horizontalNormalizedPosition = horizontalPos;
		scrollRect.verticalNormalizedPosition = verticalPos;
	}
}
