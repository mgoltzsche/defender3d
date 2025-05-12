using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

public class HighScoreView : AbstractGUIComponent {
	
	GlobalConfig globalConfig;	
	public bool inGame = false;
	// Use this for initialization
	public HighScoreView () {
		Debug.Log ("Start");
		globalConfig = GlobalConfig.getInstance();
	}
	

	protected override void OnGUI(){
		
		if(render) {
			base.OnGUI();
			CenterComponentOnScreen();
			GUILayout.Window(1,position, DrawComponent, "HighScore");
		}
	}
	
	protected override void DrawComponent(int id){
		foreach(object[] entry in globalConfig.highScore.scores){
			GUILayout.BeginHorizontal();

				GUILayout.Label((string) entry[0]);
							
				GUILayout.FlexibleSpace();
			
				GUILayout.Label( entry[1].ToString() );
			GUILayout.EndHorizontal();
		}
		
		GUILayout.FlexibleSpace();
		
		if(inGame && GUILayout.Button("Nochmal spielen")){
			Application.LoadLevel(1);
		}
		
		if(GUILayout.Button("Zur"+char.ConvertFromUtf32(252)+"ck zum Hauptmen"+char.ConvertFromUtf32(252), GUILayout.MinHeight(30f))){
			if(inGame) {
				Application.LoadLevel(0);
			}else{
				this.ToggleVisibility();
				GetComponent<MainMenu>().ToggleVisibility();			
			}
		}
		
	}
	
	protected override void resetView (){				
	}
}
