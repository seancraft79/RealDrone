//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using System.Collections;

public class TooltipManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	// reference to the tooltip background image
	public Image tooltip; 

	// reference to the Text component of the tooltip
	public string tooltipText;

	//--------------------------------------------------------------------------
	// Parameters, can be changed 
	
	// duration over which the tooltip is faded in
	private float fadeIn  = 0.2f; 

	// duration over which the tooltip is faded out
	private float fadeOut = 0.1f; 
	
	// delay until the tooltip is displayed after the mouse enters the ui component
	private float triggerDelay = 0.5f;   
	
	// vertical gap between tooltip and its ui component
	private float tooltipOffset = -5.0f;   
	
	// inset of text within tooltip background image
	private float textInset = 20.0f;   


	//--------------------------------------------------------------------------
	//--------------------------------------------------------------------------
	// Internal variables, don't change	
	private TweenAlpha tweenAlpha;
	
	// time after last mouse movement
	private float triggerTime = 0;
   
   // flag set if mouse is hovering over the ui component
	private bool hovering = false;
		
   // flag set after tooltip has been displayed, to avoid showing it once more
	private bool tooltipDisplayed = false;
	
	// flag to enable/disable the processing inside Update() 
	private bool updateEnabled = false; 

	// last mouse position for detecting mouse moves inside the ui component	
	private Vector3 lastMousePos = new Vector3(-1,-1,0); 
	
	private Text textComponent;
	
	public void SetTooltip(Image image, string text)
	{  
		tooltip = image;
		tooltipText = text;
	}
	
	public void Start() 
	{     
		updateEnabled = false;
		tooltipDisplayed = false;

		if (tooltip) { 
			// the transparency of the tooltip (=alpha) is set on the CanvasGroup attached to the tooltip background image 
			CanvasGroup g = tooltip.GetComponent<CanvasGroup>();
			if (g == null) { 
				throw new System.Exception("Error "+gameObject.name+": Tooltip image must have a CanvasGroup attached.");
			} 

			tweenAlpha = new TweenAlpha(g);

			textComponent = tooltip.GetComponentInChildren<Text>();
			if (textComponent == null) { 
				throw new System.Exception("Error "+gameObject.name+": Tooltip image must have a Text object attached.");
			} 
		}
		else {
			throw new System.Exception("Error "+gameObject.name+": Tooltip Manager must have a tooltip image attached.");
		}
	}	
	
	private bool hasMouseMoved()
	{                          
		Vector3 mousePos = Input.mousePosition;

		if (mousePos.x != lastMousePos.x || mousePos.y != lastMousePos.y) {
			lastMousePos = mousePos;
			return true;
		}
		return false;
	}        
	
	private void startTriggerDelay()
	{                          
		triggerTime = Time.time;  
	}
	
	private void enableTooltip()
	{
		if ( !(tooltipDisplayed || tweenAlpha.IsVisible()) ) {
			tooltipDisplayed = true;

			// set the tooltip text			
			textComponent.text = tooltipText;  
			
			// update tooltip background dimensions from text dimensions
			RectTransform rImage = tooltip.rectTransform;
			RectTransform rText = textComponent.rectTransform;
			rImage.SetSizeWithCurrentAnchors((RectTransform.Axis)0, rText.rect.width+textInset); 
			rImage.SetSizeWithCurrentAnchors((RectTransform.Axis)1, textComponent.preferredHeight+textInset); 
			
			// tooltip is to be enabled
			tweenAlpha.FadeIn(fadeIn);
			updateEnabled = true;
		}
	}
	
	private void disableTooltip()
	{
		if (tweenAlpha.IsVisible()) {
			tweenAlpha.FadeOut(fadeOut);
	
			// enable processing in Update()
			updateEnabled = true;   
		}
	}

	private Rect getScreenRect(RectTransform rt) 
	{
		var corners = new Vector3[4];	
		
		// if the canvas is in Screen Space-Camera or World Space mode, get the camera attached to the canvas
		Camera cam = null;
		Canvas canvas = transform.root.gameObject.GetComponent<Canvas>();
		if (canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
			cam = canvas.worldCamera;	
		}
		
		rt.GetWorldCorners(corners);

		Vector3 min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		Vector3 max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
		
		for (int i = 0; i < 4; ++i) 
		{
			Vector3 spos = RectTransformUtility.WorldToScreenPoint(cam, corners[i]); // cam == null is permissible
			min = Vector3.Min(spos, min);
			max = Vector3.Max(spos, max);
		}

		return new Rect(min.x, min.y, max.x-min.x, max.y-min.y);
	}	

	public void Update()
	{                           
		if (hovering) {
			if (hasMouseMoved()) {
				startTriggerDelay();   
				disableTooltip();
			}
			else if (Time.time > triggerTime+triggerDelay) {  
				enableTooltip();
			}
		}  	
			
		if (tooltip && updateEnabled) { 
			tweenAlpha.Update();
			
			// dimensions of the tooltip
			Rect rTooltip = tooltip.rectTransform.rect;  

			// rectangle of ui element in screen coordinates			
			Rect elemRect = getScreenRect(gameObject.GetComponent<RectTransform>());

			// rectangle of canvas in screen coordinates	
			Rect canvasRect = getScreenRect(transform.root.gameObject.GetComponent<RectTransform>());  
			
			Vector3 pos = new Vector3(0,0);  

			// positions tooltip in the middle below the bottom border of the ui element			
			pos.x = elemRect.center.x-0.5f*rTooltip.width;
			pos.y = elemRect.y-rTooltip.height;
			
			// add a little offset to the vertical position
			pos.y += tooltipOffset;
			
			// adjust position if outside screen bounds
			if (pos.x+rTooltip.width > canvasRect.x+canvasRect.width) {
				// tooltip is (partially) outside right screen border
				pos.x = canvasRect.x+canvasRect.width-rTooltip.width;
			}   
			
			if (pos.x < canvasRect.x) {
				// tooltip is (partially) outside left screen border
				pos.x = canvasRect.x;
			}   
			
			if (pos.y+rTooltip.height > canvasRect.y+canvasRect.height) {
				// tooltip is (partially) outside top screen border 
				pos.y = canvasRect.y+canvasRect.height-rTooltip.height;
			} 
			
			if (pos.y < canvasRect.y) {
				// tooltip is (partially) outside bottom screen border 
				pos.y = canvasRect.y;
			} 

			tooltip.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos.x, rTooltip.width);		
			tooltip.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, pos.y, rTooltip.height);		

			if (!tweenAlpha.IsFading()) {
				// after tooltip is completely faded in or out, disable further processing in Update() to save cpu time
				updateEnabled = false;
			}
		}
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{                 
		hovering = true;  
		tooltipDisplayed = false;
	}
	
	public void OnPointerExit(PointerEventData eventData)
	{            
		hovering = false;
	}
}
