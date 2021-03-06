class MaterialNode_ScreenTexelSize extends MaterialNode {

function Awake(editorWindow:SFMaterialEditor) {
	super.Awake(editorWindow);
	title = "ScreenTexelSize";
	outputs.Add(new MaterialNodeSocket(this));
	
	outputs[0].value = 0.0;
	outputs[0].title = "Out";
	
	var foundTextureNodes:int = 0;
	for (i = 0; i < editor.nodes.length; i++) {
		if (editor.nodes[i].title == title) {
			foundTextureNodes += 1;
		}
	}
	title2 = title+(foundTextureNodes+1);
}

function GenTagInfos() {
	outputs[0].value = "(float2(1.0/_ScreenParams.x,1.0/_ScreenParams.y))";
}

}
