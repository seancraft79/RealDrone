//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))] 
public class TweenAlpha
{
	private enum FadeMode {FadeIn, FadeOut};
	
	private CanvasGroup g;
	private float lerp = 0;
	private float lastAlpha = 0;
	private float duration = 0;
	private FadeMode fadeMode;

	public TweenAlpha(CanvasGroup _g)
	{
		g = _g;
		g.alpha = 0;
		lastAlpha = 0;
		fadeMode = FadeMode.FadeIn;
	}
	
	private float Lerp(float duration)
	{                         
		// returns a value between 0 and 1 for fading in/out
		lerp += Time.deltaTime/duration;
		lerp = Mathf.Clamp01(lerp);	
		return lerp;
	}
		
	public void FadeIn(float _duration)
	{
		duration = _duration;  
		fadeMode = FadeMode.FadeIn; 		
		lerp = 0; 		
	}
		
	public void FadeOut(float _duration)
	{
		duration = _duration;
		fadeMode = FadeMode.FadeOut;
		lastAlpha = g.alpha; 
		lerp = 0; 		
	}

	// returns true if the ui component is being faded in or out, i.e. neither	fully visible nor fully hidden
	public bool IsFading()
	{
		return (g.alpha > 0 && g.alpha < 1);
	}
		
	// returns true if the ui component is fully visible	
	public bool IsVisible()
	{
		return g.alpha > 0;;
	}
		
	public void Update()
	{
		float d = Lerp(duration);   
		g.alpha = (fadeMode == FadeMode.FadeIn ? d : lastAlpha-d); 
	}
}
