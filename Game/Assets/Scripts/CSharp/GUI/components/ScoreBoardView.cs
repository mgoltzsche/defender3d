using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;

public class ScoreBoardView : AbstractGUIComponent {
	
	public string headLine;
	public GUIStyle style;
	public string msg;
	public HighScore scores;
	public bool won;
	
	// Use this for initialization
	void OnEnable() {
		this.percentageWidth(.5f);
		this.percentageHeight(.6f);
		this.ToggleVisibility();
		
	}
	
	protected override void OnGUI(){
		
		if(render) {
			
			base.OnGUI();
			
			CenterComponentOnScreen();
			
			
			GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
						style.fontSize = 48;
						GUILayout.Label(headLine, style);
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
		
				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
						style.fontSize = 24;
						GUILayout.Label(msg,style);
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			GUILayout.Window(2,position, DrawComponent, "HighScore");			
		}
	}
	
	protected override void DrawComponent(int id){
		foreach(object[] entry in scores.scores){
			GUILayout.BeginHorizontal();

				GUILayout.Label((string) entry[0]);
							
				GUILayout.FlexibleSpace();
			
				GUILayout.Label( entry[1].ToString() );
			GUILayout.EndHorizontal();
		}
		
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button("Zur"+char.ConvertFromUtf32(252)+"ck zum Hauptmen"+char.ConvertFromUtf32(252), GUILayout.MinHeight(30f))){
				Application.LoadLevel(0);
		}
		
	}
	
	protected override void resetView (){				
	}
}
