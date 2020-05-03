//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using System; 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems; 

namespace UnityEngine.UI
{
	[RequireComponent(typeof(Image))]  
	public class RotatingKnob : Selectable, IDragHandler
	{                           
		public enum InputMethod {Rotation, Translation}; 

		[Serializable]
		public class RotatingKnobEvent : UnityEvent<float>{} 
		
		public RotatingKnobEvent onValueChanged = new RotatingKnobEvent(); 	
	
		[SerializeField]
		private float mArcDegrees = 240; 
		[SerializeField]
		private float mRotation = -30; 
		[SerializeField]
		private float mMinValue = 0; 
		[SerializeField]
		private float mMaxValue = 100; 
		[SerializeField]
		private float mValue = 0; 
		[SerializeField]
		private int mDecimals = 0; 
		[SerializeField]	
		private float mSensitivity = 1.3f;
		[SerializeField]	
		private InputMethod mInputMethod = InputMethod.Rotation;
		
		private float mLastMouseAngle = 0f;
		private Text mValueField;
		private Image mKnobHandle;
					
		public RotatingKnob()
		{ 	
			interactable = true;
		}   
		
		/// <summary>
		/// Gets or sets the arc degrees.
		/// </summary>
		/// <value>The knob's arc degrees.</value>
		public float arcDegrees
		{
			get { return mArcDegrees; }
			set { mArcDegrees = value; OnValueChanged(); }
		}		
		
		/// <summary>
		/// Gets or sets the rotation.
		/// </summary>
		/// <value>The knob's rotation.</value>
		public float rotation
		{
			get { return mRotation; }
			set { mRotation = value; OnValueChanged(); }
		}		
		
		/// <summary>
		/// Gets or sets the minimum value.
		/// </summary>
		/// <value>The knob's minimum value.</value>
		public float minValue
		{
			get { return mMinValue; }
			set { 
				mMinValue = value; 
				if (mMaxValue == mMinValue) {
					mMinValue -= 1.0f;
				}                
				OnValueChanged(); 
			}
		}		
		
		/// <summary>
		/// Gets or sets the maximum value.
		/// </summary>
		/// <value>The knob's maximum value.</value>
		public float maxValue
		{
			get { return mMaxValue; }
			set { 
				mMaxValue = value; 
				if (mMaxValue == mMinValue) {
					mMaxValue += 1.0f;
				}                
				OnValueChanged(); 
			}
		}		
				
		/// <summary>
		/// Gets or sets the number of decimals for number formatting.
		/// </summary>
		/// <value>The knob's number of decimals.</value>
		public int decimals
		{
			get { return mDecimals; }
			set { mDecimals = value; OnValueChanged(); }
		}		
		
		/// <summary>
		/// Gets or sets the value.
		/// The internal value is normalized between 0 and 1
		/// </summary>
		/// <value>The knob's value.</value>
		public float value
		{
			get { return Mathf.Lerp(mMinValue, mMaxValue, mValue); }
			set { 
				mValue = (value-mMinValue)/(mMaxValue-mMinValue);
				mValue = Mathf.Clamp01(mValue);
				OnValueChanged(); 
			}
		}		
		
		/// <summary>
		/// Gets or sets the sensitivity.
		/// </summary>
		/// <value>The knob's sensitivity.</value>
		public float sensitivity
		{
			get { return mSensitivity; }
			set { mSensitivity = value; }
		}
		
		/// <summary>
		/// Gets or sets the input method.
		/// </summary>
		/// <value>The knob's input method.</value>
		public InputMethod inputMethod
		{
			get { return mInputMethod; }
			set { mInputMethod = value; }
		}

		protected override void Awake()
		{
			mValueField = gameObject.GetComponentInChildren<Text>();
			if (mValueField == null) {
				throw new System.Exception("Error "+gameObject.name+": value text field missing.");
			} 

			Image[] images = gameObject.GetComponentsInChildren<Image>();
			foreach (Image image in images) {
				if (image.name == "KnobHandle") {
					mKnobHandle = image;
					break;
				}
			}
				
			if (mKnobHandle == null) {
				throw new System.Exception("Error "+gameObject.name+": knob handle missing.");
			} 

			OnValueChanged();			
		}
			
		private void OnValueChanged()
		{  
			mValue = Mathf.Clamp01(mValue);    
			float angle = mRotation+Mathf.Lerp(0, mArcDegrees, mValue);  

			// Rotate the knob area
			RectTransform rectKnob = targetGraphic.rectTransform;
			rectKnob.localRotation = Quaternion.AngleAxis(angle, Vector3.back);	 			

			// Rotate the knob handle backwards locally to maintain the position of gloss and shadow
			RectTransform rectHandle = mKnobHandle.rectTransform;
			rectHandle.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);	 			
			
			mValueField.text = Format(value);  // calls getter for value!
#if UNITY_EDITOR
			if (onValueChanged != null && Application.isPlaying) {
#else
			if (onValueChanged != null) {
#endif
				onValueChanged.Invoke(value);  // calls getter for value!			
			}
		}

		private string Format(float v) 
		{
			if (mDecimals <= 0) {
				return Mathf.RoundToInt(v).ToString()+"%";
			}
			
			return string.Format("{0:F"+mDecimals+"}%", v);
		} 
		
		// Calculate the angle of a vector from the knob center to the mouse position	
		private float calcMouseAngle(PointerEventData eventData)
		{                     
			// Calculate center of knob in world coordinates
			var rCrnrs = new Vector3[4];			 
			gameObject.GetComponent<RectTransform>().GetWorldCorners(rCrnrs); // bl, tl, tr, br			 
			var cx = rCrnrs[0].x+0.5f*(rCrnrs[2].x-rCrnrs[0].x);
			var cy = rCrnrs[0].y+0.5f*(rCrnrs[2].y-rCrnrs[0].y);
			Vector2 c = new Vector2(cx, cy);
			
			// Calculate vector from mouse position to center of knob			
			Vector2 dpos = eventData.position-c;
			
			// Calculate angle between 0 and 360 degrees
			float mouseAngle = Mathf.Rad2Deg*Mathf.Atan2(dpos.y, dpos.x);
			if (mouseAngle < 0) {
				mouseAngle = 360.0f+mouseAngle;
			}
			
			return mouseAngle;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{  
			base.OnPointerDown(eventData);
			mLastMouseAngle = calcMouseAngle(eventData);                 
		}

		public void OnDrag(PointerEventData eventData)
		{   
			float d = 0;                
			if (mInputMethod == InputMethod.Translation) {
				// A more primitive way to calculate rotation delta:
				// move mouse horizontally over the knob
				d = eventData.delta.x;
			}
			else if (mInputMethod == InputMethod.Rotation) {
				// move mouse in an arc over the knob, similar to a 'real' knob
				float mouseAngle = calcMouseAngle(eventData);                 
				d = mLastMouseAngle-mouseAngle;  
				if (Mathf.Abs(d) > 100.0f) {
					d = 0;
				} 
				mLastMouseAngle = mouseAngle;
			}
	
			float sensitivity = Mathf.Max(mSensitivity, 0.01f); // minimum sensible value for sensitivity
			
			float diff = sensitivity*d/300.0f;
			mValue += diff;
			OnValueChanged();
		}
	}
}
