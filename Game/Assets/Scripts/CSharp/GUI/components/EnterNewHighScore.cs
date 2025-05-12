using UnityEngine;
using System.Collections;


public class EnterNewHighScore : AbstractGUIComponent {
	
	private GlobalConfig globalCFG;
	
	public string playerName = "";
	public int points;
	
	// Use this for initialization
	protected override void OnGUI(){
		
		if(render) {
			base.OnGUI();
			CenterComponentOnScreen();
			GUILayout.Window(2,position, DrawComponent, "Neuer HighScore");
		}
	}
	
	protected override void DrawComponent(int id){				
		GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
				GUILayout.Label("Name: ");
				playerName = GUILayout.TextField(playerName);
			GUILayout.EndHorizontal();
		
			if(GUILayout.Button("Speichern")) {
				this.ToggleVisibility();
			}
		
		GUILayout.EndVertical();
	}

	
	protected override void resetView() {
		GlobalConfig.getInstance().highScore.AddNewEntry(playerName, points);
		GlobalConfig.getInstance().PersistHighScore();
		
		gameObject.GetComponent<HighScoreView>().ToggleVisibility();
	}
}

