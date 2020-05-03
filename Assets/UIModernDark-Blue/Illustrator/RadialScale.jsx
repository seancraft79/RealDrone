function whereAmI()
{
	var where
	try {
		app.documents.test();
	}
	catch (err){
		where = File(err.fileName);
	}
	return where.path;
}

function debug(text)
{
	var myJSXPath = whereAmI();
	var debugFile = new File(myJSXPath+"/debug.txt");
	debugFile.open("r");
	content = debugFile.read();
	debugFile.close();
	
	debugFile.open("w");
	debugFile.write(content);
	debugFile.writeln(text);
	debugFile.close();
}


function color(r, g, b)
{
	var c = new RGBColor();
	with (c) {
		red   = Math.min(r, 255.0);
		green = Math.min(g, 255.0);
		blue  = Math.min(b, 255.0);
	}
	
	return c;
}


//===============================================================================


function createPanel(parent)
{                                 
	panel = parent.add('panel', undefined);
	panel.alignChildren = "right";
	panel.preferredSize = [390,-1];
	panel.margins = Array(0,5,50,10);    // left, top, right, bottom
	return panel;
}

function createCeckBox(parent, label, initValue, onValueChange)
{                                 
	var h = 15;
	parent.st = parent.add('statictext', undefined, label);
	parent.cb = parent.add('checkbox', undefined); 
	parent.cb.preferredSize = [190, h];
	parent.cb.value = initValue;
	
	parent.cb.onClick = function () { 
		onValueChange(parent, this.value);
	};
}

function createSlider(parent, label, initValue, minValue, maxValue, onValueChange)
{                                 
	var h = 15;
	parent.st = parent.add('statictext', undefined, label);
	parent.sl = parent.add('slider', undefined, initValue, minValue, maxValue); 
	parent.sl.preferredSize = [150, h];
	parent.et = parent.add('edittext');
	parent.et.preferredSize = [30, h];  
	parent.et.justify = 'right';

	parent.et.text = parent.sl.value;  
	
	parent.sl.helpTip = 'You can type in greater numbers in the number field';	
	
	parent.et.onChange = function () { 
		var value = Number(this.text); 
		// If the users enters a value outside the slider's min and max values, we adjust the min and max values
		if (value < this.parent.sl.minvalue) {
			this.parent.sl.minvalue = value;
		} 
		if (value > this.parent.sl.maxvalue) {
			this.parent.sl.maxvalue = value;
		}
		this.parent.sl.value = value;
		this.parent.sl.onChanging();
	};
	parent.sl.onChanging = function () {
		var value = Math.floor(this.value);
		this.parent.et.text = value;
		onValueChange(parent, value);
	};
}

function createFontList(parent, label, initValue, onValueChange)
{                                 
	parent.st = parent.add('statictext', undefined, label);
	parent.fontList = parent.add("dropdownlist"); 
	parent.fontList.preferredSize = [190, -1];
	var fonts = app.textFonts; 
	for (var i = 0; i < fonts.length; i++)
	{
		var item = parent.fontList.add("item", fonts[i].name); 
		if (fonts[i].name == initValue) {
			item.selected = true;
		}
	}	
		
	parent.fontList.onChange = function () { 
		var fontName = this.selection.text;	
		onValueChange(parent, fontName);
	};
}

// helper function to find the topmost parent of a dialog element (= dialog window)
function findRadialScale(elem)
{
	while (elem.parent != undefined) {
		elem = elem.parent;
	}
	
	return elem.radialScale;
}


function RadialScale(docRef, centerx, centery)
{             
	this.docRef = docRef;
	this.degToRad = Math.PI/180.0;
	this.centerx = centerx;
	this.centery = centery;
	this.minvals = {}; 
	this.settings = {};

	this.deleteGraphics();

	this.settings['initialized'] = true;
       
	this.default('radius', 147, 0);	
	this.default('arcDegrees', 240, 0);	
	this.default('rotation', 30);	
	this.default('minValue', 0);	
	this.default('maxValue', 100);	

	this.default('labelFont', this.getDefaultFont());	

	this.default('color', 0xFF0000);	
	
	this.default('majorTicks', 11, 2);	
	this.default('majorTickLength', 52, 0);	
	this.default('majorTickWidth1', 5, 0);	
	this.default('majorTickWidth2', 2, 0);	
	this.default('majorLabelSize', 20, 0);	
	this.default('majorLabelOffset', 5);	
	this.default('majorLabelDecimals', 0, 0);	
	this.default('majorLabelVertical', true);	
	this.default('majorLabelLast', true);	

	this.default('mediumTicks', 1, 0);	
	this.default('mediumTickLength', 40, 0);	
	this.default('mediumTickWidth1', 3, 0);	
	this.default('mediumTickWidth2', 1, 0);	
	this.default('mediumLabelSize', 0, 0);	
	this.default('mediumLabelOffset', 5);	
	this.default('mediumLabelDecimals', 0, 0);	
	this.default('mediumLabelVertical', true);	

	this.default('minorTicks', 5, 0);	
	this.default('minorTickLength', 25, 0);	
	this.default('minorTickWidth1', 1, 0);	
	this.default('minorTickWidth2', 1, 0);	
	this.default('minorLabelSize', 0, 0);	
	this.default('minorLabelOffset', 5);	
	this.default('minorLabelDecimals', 0, 0);	
	this.default('minorLabelVertical', true);	
	
	this.update();
}

RadialScale.prototype.getStoredSettings = function()
{                       
	try {
		var tag = this.group.tags.getByName('settings');
		this.settings = eval(tag.value);		
	}
	catch (err){ 
		this.settings = {};
	}
}		

RadialScale.prototype.setStoredSettings = function()
{                         
	try {
		var tag = this.group.tags.getByName('settings');
	}
	catch (err){ 
		var tag = this.group.tags.add();
		tag.name = 'settings';
	} 

	tag.value = this.settings.toSource();
}		

RadialScale.prototype.default = function(name, value, minval)
{                         
	this.minvals[name] = minval;
	if ( !(name in this.settings) ) {
		this.settings[name] = value;
	}
}		

RadialScale.prototype.get = function(name)
{                       
	return this.settings[name];
}	

RadialScale.prototype.set = function(name, value)
{                         
	var minval = this.minvals[name];
	if (minval !== undefined) {
		value = Math.max(minval, value);
	}  
	
	this.settings[name] = value;
	this.update();
}		

RadialScale.prototype.getDefaultFont = function()
{
	// create a dummy TextFont object to obtain the default font used
	var tf = this.docRef.textFrames.add();
	tf.contents = "Text";
	var p = tf.paragraphs[0];
	var defaultFont = p.characterAttributes.textFont.name;
	tf.remove();
	return defaultFont;
}

RadialScale.prototype.getDefaultColor = function()
{
	var fillColor = this.docRef.defaultFillColor;
	if (!this.docRef.defaultFilled) {
		fillColor = color(0,0,0);
	}
	
	return fillColor;
}

RadialScale.prototype.update = function()
{
	this.deleteGraphics();

	this.setStoredSettings();

	this.drawMajorTickMarks();
	if (this.get('mediumTicks') > 0) {	
		this.drawMediumTickMarks();
	}
	if (this.get('minorTicks') > 0) {	
		this.drawMinorTickMarks();
	}

	app.redraw();
}

RadialScale.prototype.deleteGraphics = function()
{                  
	try {
		this.layer = this.docRef.layers.getByName('Radial Scale');
	}
	catch (err){ 
		this.layer = this.docRef.layers.add();
		this.layer.name = "Radial Scale";
	}

	try {
		this.group = this.layer.groupItems.getByName('Radial Scale');
		if ( !('initialized' in this.settings) ){		
			this.getStoredSettings();
		}
		this.group.remove();
	}
	catch (err){ }

	this.group = this.layer.groupItems.add();
	this.group.name = "Radial Scale";
}

RadialScale.prototype.drawTickMark = function(g, len, a, w1, w2)
{                  
	var p = g.pathItems.add();
	p.filled = true;
	p.fillColor = this.getDefaultColor();
	p.stroked = false;
	p.closed = true; 
	var h1 = w1/2;
	var h2 = w2/2; 
	var x1 = this.get('radius');
	var x2 = x1+len;
	
	p.setEntirePath(Array(Array(x1,-h1), Array(x2,-h2), Array(x2,h2), Array(x1,h1), Array(x1,h1)));
	p.rotate(a-this.get('rotation'), true, true, true, true, Transformation.DOCUMENTORIGIN);	
	p.translate(this.centerx, this.centery);	
} 

 
RadialScale.prototype.drawLabel = function(g, len, offset, a, size, value, vertical)
{                               
	var angle = a-this.get('rotation');
	var tf = g.textFrames.add();
	if (value === '-0') {
		value = '0';
	}
	tf.contents = value;
	
	var p = tf.paragraphs[0];
	var font = app.textFonts.getByName(this.get('labelFont'));	
	p.characterAttributes.textFont = font;
	p.characterAttributes.baselineShift = 0;
	p.paragraphAttributes.justification = Justification.CENTER;
	p.fillColor = this.getDefaultColor();
	p.size = size;

	// create temporary dummy outline object to get the true bounding box of the text
	var tfc = tf.duplicate();
	tfc = tfc.createOutline();
	var h = tfc.height;
	var w = tfc.width;
	tfc.remove();
	
	// move center of text to origin
	tf.translate(0, -h/2);	

	if (vertical) {
		tf.rotate(-angle, true, false, false, false, Transformation.CENTER);	
		if (offset >= 0) {
			tf.translate(w/2, 0);	
		}
		else {
			tf.translate(-w/2, 0);	
		}
	}
	else {
		if (angle <= 0 || angle > 180) {
			tf.rotate(90, true, false, false, false, Transformation.CENTER);	
		}
		else {
			tf.rotate(-90, true, false, false, false, Transformation.CENTER);	
		}
	}	

	tf.translate(this.get('radius')+len+offset, 0);	
	tf.rotate(angle, true, false, false, false, Transformation.DOCUMENTORIGIN);	
	tf.translate(this.centerx, this.centery);	
}  

RadialScale.prototype.drawMajorTickMarks = function()
{   
	var len = Math.min(this.get('majorTickLength'), this.get('radius'));

	var gt = this.group.groupItems.add();
	gt.name = "Major tick marks";

	var gl = this.group.groupItems.add();
	gl.name = "Major labels";
	
	var a = 0; 
	var majorTicks = this.get('majorTicks');
	var da = this.get('arcDegrees')/(majorTicks-1); 
	var v = this.get('maxValue');
	var dv = (this.get('maxValue')-this.get('minValue'))/(majorTicks-1); 

	for (var i = 0; i < majorTicks; i++)
	{       
		this.drawTickMark(gt, len, a, this.get('majorTickWidth1'), this.get('majorTickWidth2'));
		 
		if (this.get('majorLabelSize') > 0) {		 
			var s = v.toFixed(this.get('majorLabelDecimals'));
			if ( i > 0 || (i == 0 && (this.get('majorLabelLast') && this.get('arcDegrees') < 360)) ) {
				this.drawLabel(gl, len, this.get('majorLabelOffset'), a, this.get('majorLabelSize'), s, this.get('majorLabelVertical'));
			}   
		}
		a+=da; 
		v-=dv;
	}
}

RadialScale.prototype.drawMediumTickMarks = function()
{   
	var len = Math.min(this.get('mediumTickLength'), this.get('radius'));

	var gt = this.group.groupItems.add();
	gt.name = "Medium tick marks";

	var gl = this.group.groupItems.add();
	gl.name = "Medium labels";
	
	var a = 0; 
	var n = (this.get('majorTicks')-1)*(this.get('mediumTicks')+1);
	var da = this.get('arcDegrees')/n;
	var v = this.get('maxValue');
	var dv = (this.get('maxValue')-this.get('minValue'))/n; 
	var skip = this.get('mediumTicks')+1;

	for (var i = 0; i < n; i++)
	{       
		if (i%skip != 0) {
			this.drawTickMark(gt, len, a, this.get('mediumTickWidth1'), this.get('mediumTickWidth2'));

			if (this.get('mediumLabelSize') > 0) {		 
				var s = v.toFixed(this.get('mediumLabelDecimals'));
				this.drawLabel(gl, len, this.get('mediumLabelOffset'), a, this.get('mediumLabelSize'), s, this.get('mediumLabelVertical'));
			}
		}
		a+=da; 
		v-=dv;
	}
}

RadialScale.prototype.drawMinorTickMarks = function()
{   
	var len = Math.min(this.get('minorTickLength'), this.get('radius'));

	var gt = this.group.groupItems.add();
	gt.name = "Minor tick marks";

	var gl = this.group.groupItems.add();
	gl.name = "Minor labels";
	
	var i = 0;
	var a = 0; 
	var n = (this.get('majorTicks')-1)*(this.get('mediumTicks')+1)*(this.get('minorTicks')+1);
	var da = this.get('arcDegrees')/n;
	var v = this.get('maxValue');
	var dv = (this.get('maxValue')-this.get('minValue'))/n; 
	var skip = this.get('minorTicks')+1;

	for (var i = 0; i < n; i++)
	{       
		if (i%skip != 0) {
			this.drawTickMark(gt, len, a, this.get('minorTickWidth1'), this.get('minorTickWidth2'));
		 
			if (this.get('minorLabelSize') > 0) {		 
				var s = v.toFixed(this.get('minorLabelDecimals'));
				this.drawLabel(gl, len, this.get('minorLabelOffset'), a, this.get('minorLabelSize'), s, this.get('minorLabelVertical'));
			}
		}
		a+=da; 
		v-=dv;
	}
}



//=========================================================================================
function ParameterDialog(radialScale)
{                               
	this.dialog = this.createDialog(radialScale); 
}
	
ParameterDialog.prototype.createDialog = function(radialScale) 
{
	var dlg = new Window('dialog', 'Radial Scale Script © Michael Schmeling');
	//dlg.orientation = "column";  
	dlg.alignChildren = "right";
	dlg.margins = 2;
	dlg.spacing = 2; 
	
	// Add scale object to the dialog in order to access it from handler functions
	dlg.radialScale = radialScale;

	// Tabbed Panel
	var tabPanel = dlg.add('tabbedpanel');
	tabPanel.preferredSize = [400,300];
	
	//----------------------------------------------------------		
	var generalTab = tabPanel.add('tab', undefined, 'General');

	generalTab.generalPnl = createPanel(generalTab);	

	generalTab.generalPnl.radius = generalTab.generalPnl.add('group');
	createSlider(generalTab.generalPnl.radius, 'Scale radius:', radialScale.get('radius'), 1, 500, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('radius', value); 
	});


	generalTab.generalPnl.degrees = generalTab.generalPnl.add('group');
	createSlider(generalTab.generalPnl.degrees, 'Arc degrees:', radialScale.get('arcDegrees'), 30, 360, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('arcDegrees', value); 
	});

	generalTab.generalPnl.rotation = generalTab.generalPnl.add('group');
	createSlider(generalTab.generalPnl.rotation, 'Rotation:', radialScale.get('rotation'), 0, 360, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('rotation', value); 
	});

	generalTab.generalPnl.minvalue = generalTab.generalPnl.add('group');
	createSlider(generalTab.generalPnl.minvalue, 'Min. value:', radialScale.get('minValue'), -100, 100, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('minValue', value); 
	});

	generalTab.generalPnl.maxvalue = generalTab.generalPnl.add('group');
	createSlider(generalTab.generalPnl.maxvalue, 'Max. value:', radialScale.get('maxValue'), 0, 1000, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('maxValue', value); 
	});

	generalTab.generalPnl.fonts = generalTab.generalPnl.add('group');
	createFontList(generalTab.generalPnl.fonts, 'Label font:', radialScale.get('labelFont'), function(elem, value) { 
		findRadialScale(elem).set('labelFont', value); 
	});
		
		
	//----------------------------------------------------------		
	var majorTab = tabPanel.add('tab', undefined, 'Major Ticks');
	majorTab.majorTicksPnl = majorTab.add('panel', undefined);
	majorTab.majorTicksPnl.alignChildren = "right";
		
	majorTab.majorTicksPnl.ticks = majorTab.majorTicksPnl.add('group');

	createSlider(majorTab.majorTicksPnl.ticks, 'Major tick marks:', radialScale.get('majorTicks'), 5, 50, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('majorTicks', value); 
	});

	majorTab.majorTicksPnl.length = majorTab.majorTicksPnl.add('group');
	createSlider(majorTab.majorTicksPnl.length, 'Length:', radialScale.get('majorTickLength'), 1, 500, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('majorTickLength', value); 
	});

	majorTab.majorTicksPnl.width1 = majorTab.majorTicksPnl.add('group');
	createSlider(majorTab.majorTicksPnl.width1, 'Bottom width:', radialScale.get('majorTickWidth1'), 1, 10, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('majorTickWidth1', value); 
	});

	majorTab.majorTicksPnl.width2 = majorTab.majorTicksPnl.add('group');
	createSlider(majorTab.majorTicksPnl.width2, 'Tip width:', radialScale.get('majorTickWidth2'), 1, 10, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('majorTickWidth2', value); 
	});

	majorTab.majorTicksPnl.labelSize = majorTab.majorTicksPnl.add('group');
	createSlider(majorTab.majorTicksPnl.labelSize, 'Label size:', radialScale.get('majorLabelSize'), 0, 100, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('majorLabelSize', value); 
	});

	majorTab.majorTicksPnl.labelOffset = majorTab.majorTicksPnl.add('group');
	createSlider(majorTab.majorTicksPnl.labelOffset, 'Label offset:', radialScale.get('majorLabelOffset'), -200, 100, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('majorLabelOffset', value); 
	});

	majorTab.majorTicksPnl.labelDecimals = majorTab.majorTicksPnl.add('group');
	createSlider(majorTab.majorTicksPnl.labelDecimals, 'Label decimals:', radialScale.get('majorLabelDecimals'), 0, 5, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('majorLabelDecimals', value); 
	});

	majorTab.majorTicksPnl.labelVertical = majorTab.majorTicksPnl.add('group');
	createCeckBox(majorTab.majorTicksPnl.labelVertical, 'Label vertical:', radialScale.get('majorLabelVertical'), function(elem, value) {
		findRadialScale(elem).set('majorLabelVertical', value); 
	});

	majorTab.majorTicksPnl.labelLast = majorTab.majorTicksPnl.add('group');
	createCeckBox(majorTab.majorTicksPnl.labelLast, 'Draw last label:', radialScale.get('majorLabelLast'), function(elem, value) {
		findRadialScale(elem).set('majorLabelLast', value); 
	});


	//----------------------------------------------------------		
	var mediumTab = tabPanel.add('tab', undefined, 'Medium Ticks');
	mediumTab.medTicksPnl = mediumTab.add('panel', undefined);
	mediumTab.medTicksPnl.alignChildren = "right";


	mediumTab.medTicksPnl.ticks = mediumTab.medTicksPnl.add('group');
	createSlider(mediumTab.medTicksPnl.ticks, 'Medium tick marks:', radialScale.get('mediumTicks'), 0, 10, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('mediumTicks', value); 
	});

	mediumTab.medTicksPnl.length = mediumTab.medTicksPnl.add('group');
	createSlider(mediumTab.medTicksPnl.length, 'Medium ticks length:', radialScale.get('mediumTickLength'), 1, 500, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('mediumTickLength', value); 
	});

	mediumTab.medTicksPnl.width1 = mediumTab.medTicksPnl.add('group');
	createSlider(mediumTab.medTicksPnl.width1, 'Bottom width:', radialScale.get('mediumTickWidth1'), 1, 10, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('mediumTickWidth1', value); 
	});

	mediumTab.medTicksPnl.width2 = mediumTab.medTicksPnl.add('group');
	createSlider(mediumTab.medTicksPnl.width2, 'Tip width:', radialScale.get('mediumTickWidth2'), 1, 10, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('mediumTickWidth2', value); 
	});


	mediumTab.medTicksPnl.labelSize = mediumTab.medTicksPnl.add('group');
	createSlider(mediumTab.medTicksPnl.labelSize, 'Label size:', radialScale.get('mediumLabelSize'), 0, 50, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('mediumLabelSize', value); 
	});

	mediumTab.medTicksPnl.labelOffset = mediumTab.medTicksPnl.add('group');
	createSlider(mediumTab.medTicksPnl.labelOffset, 'Label offset:', radialScale.get('mediumLabelOffset'), -200, 100, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('mediumLabelOffset', value); 
	});

	mediumTab.medTicksPnl.labelDecimals = mediumTab.medTicksPnl.add('group');
	createSlider(mediumTab.medTicksPnl.labelDecimals, 'Label decimals:', radialScale.get('mediumLabelDecimals'), 0, 5, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('mediumLabelDecimals', value); 
	});

	mediumTab.medTicksPnl.labelVertical = mediumTab.medTicksPnl.add('group');
	createCeckBox(mediumTab.medTicksPnl.labelVertical, 'Label vertical:', radialScale.get('mediumLabelVertical'), function(elem, value) {
		findRadialScale(elem).set('mediumLabelVertical', value); 
	});

	//----------------------------------------------------------		
	var minorTab = tabPanel.add('tab', undefined, 'Minor Ticks');
	minorTab.minTicksPnl = minorTab.add('panel', undefined);
	minorTab.minTicksPnl.alignChildren = "right";


	minorTab.minTicksPnl.ticks = minorTab.minTicksPnl.add('group');
	createSlider(minorTab.minTicksPnl.ticks, 'Minor tick marks:', radialScale.get('minorTicks'), 0, 10, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('minorTicks', value); 
	});

	minorTab.minTicksPnl.length = minorTab.minTicksPnl.add('group');
	createSlider(minorTab.minTicksPnl.length, 'Minor ticks length:', radialScale.get('minorTickLength'), 1, 500, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('minorTickLength', value); 
	});

	minorTab.minTicksPnl.width1 = minorTab.minTicksPnl.add('group');
	createSlider(minorTab.minTicksPnl.width1, 'Bottom width:', radialScale.get('minorTickWidth1'), 1, 10, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('minorTickWidth1', value); 
	});

	minorTab.minTicksPnl.width2 = minorTab.minTicksPnl.add('group');
	createSlider(minorTab.minTicksPnl.width2, 'Tip width:', radialScale.get('minorTickWidth2'), 1, 10, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('minorTickWidth2', value); 
	});

	minorTab.minTicksPnl.labelSize = minorTab.minTicksPnl.add('group');
	createSlider(minorTab.minTicksPnl.labelSize, 'Label size:', radialScale.get('minorLabelSize'), 0, 50, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('minorLabelSize', value); 
	});

	minorTab.minTicksPnl.labelOffset = minorTab.minTicksPnl.add('group');
	createSlider(minorTab.minTicksPnl.labelOffset, 'Label offset:', radialScale.get('minorLabelOffset'), -200, 100, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('minorLabelOffset', value); 
	});

	minorTab.minTicksPnl.labelDecimals = minorTab.minTicksPnl.add('group');
	createSlider(minorTab.minTicksPnl.labelDecimals, 'Label decimals:', radialScale.get('minorLabelDecimals'), 0, 5, function(elem, value) {
		value = Math.floor(value);
		findRadialScale(elem).set('minorLabelDecimals', value); 
	});

	minorTab.minTicksPnl.labelVertical = minorTab.minTicksPnl.add('group');
	createCeckBox(minorTab.minTicksPnl.labelVertical, 'Label vertical:', radialScale.get('minorLabelVertical'), function(elem, value) {
		findRadialScale(elem).set('minorLabelVertical', value); 
	});
	
	//----------------------------------------------------------		
	dlg.actionPnl = dlg.add('group', undefined, '');
	dlg.actionPnl.orientation = "column";
	dlg.actionPnl.closeBtn = dlg.actionPnl.add('button', undefined, 'Close');

	dlg.actionPnl.closeBtn.onClick = function() { 
		this.parent.parent.close(1); 
	};
	
	return dlg;
} 

ParameterDialog.prototype.run = function()
{
	this.dialog.show();
}
 
//===============================================================================================

function main()
{      
	if (documents.length > 0) {  
		var docRef = app.activeDocument;   
		artBoard = docRef.artboards[0];	

		var w = docRef.width;
		var h = docRef.height;
		var b = artBoard.artboardRect;  // left, top, right, bottom
	
		var x = b[0]+0.5*(b[2]-b[0]);
		var y = b[3]+0.5*(b[1]-b[3]);
		
		var scale = new RadialScale(docRef, x, y);	
		new ParameterDialog(scale).run();
	}
	else {
		alert('The script needs an open document!', 'Radial Scale Script Alert', true);
	}
}

main();
