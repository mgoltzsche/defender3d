using UnityEngine;
using System.Collections;
using State.HUD;

public class InGameMenu : AbstractGUIComponent {	
	private NetworkWindow networkWindow;
	
	void Start(){
		this.percentageWidth(0.5f);
		this.percentageHeight(0.8f);
	}
	
	protected override void OnGUI(){
		
		if(render) {
			base.OnGUI();
			CenterComponentOnScreen();
			GUILayout.Window(1,position, DrawComponent, "Defender");
		}
	
	}
	
	protected override void DrawComponent(int id){
		if(GUILayout.Button("Zur"+char.ConvertFromUtf32(252)+"ck zum Spiel", GUILayout.MinHeight(30f))){
			this.ToggleVisibility();							
		}
		
		if(GUILayout.Button("Zur"+char.ConvertFromUtf32(252)+"ck zum Hauptmen"+char.ConvertFromUtf32(252), GUILayout.MinHeight(30f))){
			Application.LoadLevel(0);			
		}
		
		GUILayout.FlexibleSpace();
				
		if(GUILayout.Button("Beenden", GUILayout.MinHeight(30f))){
			Application.Quit();
		}
		
		
		
	}	
	protected override void resetView (){				
	}
}