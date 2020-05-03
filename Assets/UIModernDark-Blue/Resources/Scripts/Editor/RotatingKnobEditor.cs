//--------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling (Arid Ocean)  
// http://www.aridocean.com
//--------------------------------------------------

using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Inspector class used to edit RotatingKnobs.
/// </summary>
     
namespace UnityEditor.UI
{
	[CustomEditor(typeof(RotatingKnob), true)]
	public class RotatingKnobEditor : SelectableEditor
	{
		public override void OnInspectorGUI()
		{     
			this.serializedObject.Update();
			base.OnInspectorGUI();
			
			RotatingKnob knob = target as RotatingKnob;  
			
			knob.arcDegrees = EditorGUILayout.Slider("Arc Degrees", knob.arcDegrees, 0.0f, 360.0f);
			knob.rotation = EditorGUILayout.Slider("Rotation", knob.rotation, -180.0f, 180.0f);
			knob.minValue = EditorGUILayout.Slider("Min. Value", knob.minValue, -100.0f, 100.0f);
			knob.maxValue = EditorGUILayout.Slider("Max. Value", knob.maxValue, -100.0f, 100.0f);
			knob.decimals = (int)EditorGUILayout.Slider("No. of Decimals", knob.decimals, 0, 4);
			knob.value = EditorGUILayout.Slider("Value", knob.value, 0.0f, 100.0f);
			knob.sensitivity = EditorGUILayout.Slider("Sensitivity", knob.sensitivity, 0.0f, 3.0f);
			knob.inputMethod = (RotatingKnob.InputMethod)EditorGUILayout.EnumPopup("Input Method", knob.inputMethod);
			
			this.serializedObject.ApplyModifiedProperties();
		}
	}
}