//------------------------------------------------------------------------------
//            UI Modern Dark Blue
// Copyright © 2015 Michael Schmeling. All Rights Reserved.
// http://www.aridocean.com    
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

// This script should be attached to a Canvas object in your scene.

public class DemoScene : MonoBehaviour
{
	void Start()
	{
		// Get the Canvas the script is attached to
		UIElement canvas = new UIElement(gameObject);
		canvas.AddVerticalLayoutGroup(expandWidth: true, expandHeight: true);

		// Add a separate tooltip canvas, which is placed in sort order above all other elements
		// The tooltip canvas is assigned to a static variable in UIElement for easier access
		UIElement.SetTooltipCanvas(new UITooltipCanvas());


		// Create a background panel
      UIElement backgroundPanel = new UIPanel(canvas, UIPanel.Type.Flat, UIPanel.Border.None, "Background Panel");
		backgroundPanel.AddVerticalLayoutGroup(expandWidth: true, padding: new RectOffset(0,0,0,0));  // left, right, top, bottom

		// Create a main panel which will include all UI elements
      UIElement mainPanel = new UIPanel(backgroundPanel, UIPanel.Type.Thick, UIPanel.Border.RaisedEtched, "Main Panel");
		mainPanel.AddVerticalLayoutGroup(expandWidth: true, padding: new RectOffset(20,20,20,20));  // left, right, top, bottom

		// Create a header panel at the top with the title text
      UIElement titlePanel = new UIPanel(mainPanel, UIPanel.Type.Thick, UIPanel.Border.SunkenEtched, elemName: "Title Panel");
		titlePanel.AddVerticalLayoutGroup(expandWidth: true, padding: new RectOffset(20,20,20,0));  // left, right, top, bottom
		titlePanel.AddLayoutElement(minHeight: 75);

		new UIText(titlePanel, "Modern Dark GUI Skin and Widgets", fontSize: 28);		
		new UIText(titlePanel, "for uGUI (4.6+)", fontSize: 20);		

		// create a panel for all content below the header panel
      UIElement contentPanel = new UIPanel(mainPanel, UIPanel.Type.Thick, UIPanel.Border.Sunken, elemName: "Content Panel");
		contentPanel.AddVerticalLayoutGroup(padding: new RectOffset(15,10,15,15));   // left, right, top, bottom
		contentPanel.AddLayoutElement(preferredHeight: 1000);

		// The content should be scrollable, so we create a scrollview
		// UIScrollView automatically creates a scroll parent panel which holds the scroll view and the two scroll bars
		// Note that the horizontal scrollbar is parented to the content panel in order to place it correctly
		UIScrollView scrollView = new UIScrollView(contentPanel);

		// Now create the actual scrollable	content panel, which holds all scrollable elements	
      UIElement scrollContent = new UIPanel(scrollView, UIPanel.Type.Empty, elemName: "ScrollContent");
		scrollContent.AddVerticalLayoutGroup(spacing: 0, padding: new RectOffset(0,20,0,20)); 
		scrollContent.AddContentSizeFitter();
		scrollView.AddScrollContent(scrollContent);

		//------------------------------------------------------------------------

		// We create three rows of UI elements. Each row has a panel at the top which is used to display a short
		// message whenever the user interacts with an UI element in this row.
      UIElement row;
      UIElement elems;
      UIElement displayRow;
		//------------------------------------------------------------------------
      row = new UIPanel(scrollContent, UIPanel.Type.Empty, elemName: "Row 1");
		row.AddVerticalLayoutGroup(spacing: 5);  

		// A panel for the message text		
      displayRow = new UIPanel(row, UIPanel.Type.Thin, UIPanel.Border.Sunken, "Display Row 1");
		displayRow.AddVerticalLayoutGroup(padding: new RectOffset(400,0,10,0));   // left, right, top, bottom
		displayRow.AddLayoutElement(preferredWidth: 900, preferredHeight: 40);
		UIText displayText1 = new UIText(displayRow, fontSize: 16);
		displayText1.SetAlignment(TextAnchor.MiddleCenter);

		// A panel for the UI elements in this row
		// An empty UIPanel is used to hold the layout group for its children. The panel itself is not visible to the user ('Empty').		
      elems = new UIPanel(row, UIPanel.Type.Empty, elemName: "Elements Row 1");
		elems.AddHorizontalLayoutGroup(spacing: 10, padding: new RectOffset(0,0,2,5));

		// A regular (square) toggle button
		// An event handler is attched to the button which display a short text in the display panel at the top of the row
		// Similar handler are attached to all UI elements
      UIToggle squareToggle = new UIToggle(elems, UIToggle.Type.Square, "Square Toggle");
      squareToggle.SetText("Toggle"); 
      squareToggle.AddTooltip("This is a Square Toogle");
		squareToggle.AddLayoutElement(preferredWidth: 90);
		squareToggle.SetState(isOn: true); 
		squareToggle.OnValueChanged((v) => {
			displayText1.SetText( (v ? "Square Toggle On" : "Square Toggle Off") );
		});

      UIToggle roundToggle = new UIToggle(elems, UIToggle.Type.Round, "Round Toggle");
		roundToggle.AddLayoutElement(preferredWidth: 90);
      roundToggle.SetText("Toggle");
      roundToggle.AddTooltip("This is a Round Toogle");
		roundToggle.SetState(isOn: true); 
		roundToggle.OnValueChanged((v) => {
			displayText1.SetText( (v ? "Round Toggle On" : "Round Toggle Off") );
		});

		UIButton roundButton = new UIButton(elems, UIButton.Type.Round, "Round Button");
		roundButton.AddLayoutElement(preferredWidth: 130);
      roundButton.AddTooltip("This is a Round Button");
		roundButton.OnValueChanged(() => {
			displayText1.SetText("Round Button clicked");
		});

		UIButton rectButton = new UIButton(elems, UIButton.Type.Rectangle, "Rectangular Button");
		rectButton.SetText("Rect Button");
		rectButton.AddLayoutElement(minWidth: 130);
      rectButton.AddTooltip("This is a Rectangle Button");
		rectButton.OnValueChanged(() => {
			displayText1.SetText("Rectangular Button clicked");
		});

		UIButton roundRectButton = new UIButton(elems, UIButton.Type.RoundRectangle, "Round Rectangular Button");
		roundRectButton.SetText("Rnd Rect Button");
		roundRectButton.AddLayoutElement(minWidth: 150);
      roundRectButton.AddTooltip("This is a Round Rectangle Button");
		roundRectButton.OnValueChanged(() => {
			displayText1.SetText("Round Rectangular Button clicked");
		});


		//------------------------------------------------------------------------
		// All subsequent row are created in the same manner, with a display area at the top and some UI elements in a row below 
      row = new UIPanel(scrollContent, UIPanel.Type.Empty, elemName: "Row 2");
		row.AddVerticalLayoutGroup(spacing: 5);  

      displayRow = new UIPanel(row, UIPanel.Type.Thin, UIPanel.Border.Sunken, "Display Row 2");
		displayRow.AddVerticalLayoutGroup(padding: new RectOffset(400,0,10,0));   // left, right, top, bottom
		displayRow.AddLayoutElement(preferredWidth: 900, preferredHeight: 40);
		UIText displayText2 = new UIText(displayRow, fontSize: 16);
		displayText2.SetAlignment(TextAnchor.MiddleCenter);

      elems = new UIPanel(row, UIPanel.Type.Empty, elemName: "Elements Row 2");
		elems.AddHorizontalLayoutGroup(spacing: 10, padding: new RectOffset(0,0,2,5));

		// This is just a standard input field
		UITextInputField input = new UITextInputField(elems, "Type here...");
		input.AddLayoutElement(minWidth: 100, preferredWidth: 300, minHeight: 100);
      input.AddTooltip("This is a Text Input Field");
		input.onEndEdit((s) => { 
			displayText2.SetText("Input submitted: "+s);
		});


		// A group panel is used as a toggle group		
      UIPanel radioGroup1 = new UIPanel(elems, UIPanel.Type.Thin, UIPanel.Border.Sunken, "Radio Group 1");
      // The radio buttons will be arranged vertically
		radioGroup1.AddVerticalLayoutGroup(spacing: 5, padding: new RectOffset(20,0,15,0));   // left, right, top, bottom
		radioGroup1.AddLayoutElement(preferredWidth: 180, preferredHeight: 100);

      UIToggle radio11 = new UIToggle(radioGroup1, UIToggle.Type.Square, "Radio Button 1");
		radio11.AddLayoutElement(preferredWidth: 190);
      radio11.AddTooltip("This is a Radio Button");
		// Sets the intial state of this toggle button to 'on'
		radio11.SetState(isOn: true); 
		
		// When a toggle button is attached to the group panel it is turns into a radio group
		radioGroup1.AddToggle(radio11);
		radio11.OnValueChanged((v) => {
			displayText2.SetText( (v ? "Radio Button 1 On" : "Radio Button 1 Off") );
		});

      UIToggle radio12 = new UIToggle(radioGroup1, UIToggle.Type.Square, "Radio Button 2");
		radio12.AddLayoutElement(preferredWidth: 190); 
      radio12.AddTooltip("This is a Radio Button");
		// add the second toggle button to the radio group
		radioGroup1.AddToggle(radio12);
		radio12.OnValueChanged((v) => {
			displayText2.SetText( (v ? "Radio Button 2 On" : "Radio Button 2 Off") );
		});


		// all other radio groups are created in the same manner a above
      UIPanel radioGroup2 = new UIPanel(elems, UIPanel.Type.Thin, UIPanel.Border.RaisedEtched, "Radio Group 2");
		radioGroup2.AddVerticalLayoutGroup(spacing: 5, padding: new RectOffset(20,0,15,0));   // left, right, top, bottom
		radioGroup2.AddLayoutElement(preferredWidth: 180, preferredHeight: 100);

      UIToggle radio21 = new UIToggle(radioGroup2, UIToggle.Type.Round, "Radio Button 3");
		radio21.AddLayoutElement(preferredWidth: 190);
      radio21.AddTooltip("This is a Radio Button");
		radio21.SetState(isOn: true);
		radioGroup2.AddToggle(radio21);
		radio21.OnValueChanged((v) => {
			displayText2.SetText( (v ? "Radio Button 3 On" : "Radio Button 3 Off") );
		});

      UIToggle radio22 = new UIToggle(radioGroup2, UIToggle.Type.Round, "Radio Button 4");
		radio22.AddLayoutElement(preferredWidth: 190);
      radio22.AddTooltip("This is a Radio Button");
		radioGroup2.AddToggle(radio22);
		radio22.OnValueChanged((v) => {
			displayText2.SetText( (v ? "Radio Button 4 On" : "Radio Button 4 Off") );
		});
         
         
		// all other radio groups are created in the same manner a above
      UIPanel radioGroup3 = new UIPanel(elems, UIPanel.Type.Thin, UIPanel.Border.SunkenEtched, "Radio Group 3");
		radioGroup3.AddVerticalLayoutGroup(spacing: 5, padding: new RectOffset(20,0,15,0));   // left, right, top, bottom
		radioGroup3.AddLayoutElement(preferredWidth: 180, preferredHeight: 100);

      UIToggle radio31 = new UIToggle(radioGroup3, UIToggle.Type.Round, "Radio Button 5");
		radio31.AddLayoutElement(preferredWidth: 190);
      radio31.AddTooltip("This is a Radio Button");
		radio31.SetState(isOn: true);
		radioGroup3.AddToggle(radio31);
		radio31.OnValueChanged((v) => {
			displayText2.SetText( (v ? "Radio Button 5 On" : "Radio Button 5 Off") );
		});

      UIToggle radio32 = new UIToggle(radioGroup3, UIToggle.Type.Round, "Radio Button 6");
		radio32.AddLayoutElement(preferredWidth: 190);
      radio32.AddTooltip("This is a Radio Button");
		radioGroup3.AddToggle(radio32);
		radio32.OnValueChanged((v) => {
			displayText2.SetText( (v ? "Radio Button 6 On" : "Radio Button 6 Off") );
		});


		//------------------------------------------------------------------------
		// All subsequent row are created in the same manner, with a display area at the top and some UI elements in a row below 
      row = new UIPanel(scrollContent, UIPanel.Type.Empty, elemName: "Row 3");
		row.AddVerticalLayoutGroup(spacing: 5, padding: new RectOffset(0,0,0,0));   // left, right, top, bottom
		
      displayRow = new UIPanel(row, UIPanel.Type.Thin, UIPanel.Border.Raised, "Display Row 3");
		displayRow.AddVerticalLayoutGroup(padding: new RectOffset(400,0,10,0));   // left, right, top, bottom
		displayRow.AddLayoutElement(preferredWidth: 900, preferredHeight: 40);
		UIText displayText3 = new UIText(displayRow, fontSize: 16);
		displayText3.SetAlignment(TextAnchor.MiddleCenter);

      elems = new UIPanel(row, UIPanel.Type.Empty, elemName: "Elements Row 3");
		elems.AddHorizontalLayoutGroup(spacing: 10, padding: new RectOffset(0,0,10,5), alignment: TextAnchor.MiddleLeft);

		// This row shows the various slider variants
		// When the user moves a slider, a corresponding text with the slider value is shown 
		// in the display area on top of the row
		UISlider slider21 = new UISlider(elems, UISlider.Type.Horizontal, UISlider.Style.Plain); 
      slider21.AddTooltip("This is a Plain Horizontal Slider");
		slider21.SetValue(0.6f);
		slider21.OnValueChanged((v) => { 
			displayText3.SetText("Horizontal Slider changed: "+Mathf.RoundToInt(v*100)+"%");
		});  
		
		UISlider slider23 = new UISlider(elems, UISlider.Type.Vertical, UISlider.Style.Plain);
      slider23.AddTooltip("This is a Plain Vertical Slider");
		slider23.SetValue(0.7f);
		slider23.OnValueChanged((v) => { 
			displayText3.SetText("Vertical Slider changed: "+Mathf.RoundToInt(v*100)+"%");
		});  
		
		UISlider slider22 = new UISlider(elems, UISlider.Type.Horizontal, UISlider.Style.Labelled);
      slider22.AddTooltip("This is a Labelled Horizontal Slider");
		slider22.SetValue(0.6f);
		slider22.OnValueChanged((v) => { 
			displayText3.SetText("Horizontal Labeled Slider changed: "+Mathf.RoundToInt(v*100)+"%");
		});  

		UISlider slider31 = new UISlider(elems, UISlider.Type.Vertical, UISlider.Style.Labelled);
      slider31.AddTooltip("This is a Labelled Vertical Slider");
		slider31.SetValue(0.4f);
		slider31.OnValueChanged((v) => { 
			displayText3.SetText("Vertical Labelled Slider changed: "+Mathf.RoundToInt(v*100)+"%");
		});  

		UISlider slider4 = new UISlider(elems, UISlider.Type.Vertical, UISlider.Style.LabelTab);
      slider4.AddTooltip("This is a Labelled Vertical Slider with Tab");
		slider4.SetValue(0.7f);
		slider4.OnValueChanged((v) => { 
			displayText3.SetText("Vertical Tab Label Slider changed: "+Mathf.RoundToInt(v*100)+"%");
		});  

		UISlider slider5 = new UISlider(elems, UISlider.Type.Horizontal, UISlider.Style.LabelTab);
      slider5.AddTooltip("This is a Labelled Horizontal Slider with Tab");
		slider5.SetValue(0.4f);
		slider5.OnValueChanged((v) => { 
			displayText3.SetText("Horizontal Tab Label Slider changed: "+Mathf.RoundToInt(v*100)+"%");
		});  


		//------------------------------------------------------------------------
		// All subsequent row are created in the same manner, with a display area at the top and some UI elements in a row below 
      row = new UIPanel(scrollContent, UIPanel.Type.Empty, elemName: "Row 4");
		row.AddVerticalLayoutGroup(spacing: 5, padding: new RectOffset(0,0,0,0));   // left, right, top, bottom
		
      displayRow = new UIPanel(row, UIPanel.Type.Thin, UIPanel.Border.Sunken, "Display Row 4");
		displayRow.AddVerticalLayoutGroup(padding: new RectOffset(400,0,10,0));   // left, right, top, bottom
		displayRow.AddLayoutElement(preferredWidth: 900, preferredHeight: 40);
		UIText displayText4 = new UIText(displayRow, fontSize: 16);
		displayText4.SetAlignment(TextAnchor.MiddleCenter);

      elems = new UIPanel(row, UIPanel.Type.Empty, elemName: "Elements Row 4");
		elems.AddHorizontalLayoutGroup(spacing: 10, padding: new RectOffset(0,0,10,5), alignment: TextAnchor.MiddleLeft);

      UIRotatingKnob knob1 = new UIRotatingKnob(elems);
		knob1.AddLayoutElement(preferredWidth: 200, preferredHeight: 200);
      knob1.AddTooltip("This is a Rotating Knob");
		knob1.OnValueChanged((v) => {
			displayText4.SetText("Knob changed: "+Mathf.Round(v*10)/10+"%");
		});


		// This is required to place the intial scroll position at the upper left corner
		// Otherwise the scroll view is positioned in the middle
		// Probably related to Unity 4.6.0 Bug 643604 
		scrollView.SetScrollPosition();
	}
 	
}