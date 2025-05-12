using UnityEngine;
using System.Collections;


public class NetworkWindow : AbstractGUIComponent {
	
	public int scene = 2;
	private GlobalConfig globalCFG;
	private string serverIP = "enter host IP";
	
	private float gameDuration = 5.0f;
	
	void Awake(){
		globalCFG = GlobalConfig.getInstance();
	}
	
	
	protected override void OnGUI(){
		
		if(render) {
			base.OnGUI();
			CenterComponentOnScreen();
			GUILayout.Window(2,position, DrawComponent, "Netzwerk");
		}
	}
	
	protected override void DrawComponent(int id){	
		
		GUILayout.BeginHorizontal();
			
			if(scene == 3) {
				GUILayout.Label("Dauer: " + string.Format("{0:00}:{1:00}", Mathf.Floor(gameDuration / 60f), gameDuration % 60)+" hh : mm");
			
				gameDuration = GUILayout.HorizontalSlider(gameDuration, 1.0f , 60.0f);
			}
			
			if(GUILayout.Button("Host",GUILayout.MinHeight(30f))) {
				globalCFG.deathMatchDuration = gameDuration;
				Application.LoadLevel(scene);
			}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("Connect");
		
		serverIP = GUILayout.TextField(serverIP, 15);
		if(GUILayout.Button("Connect")) {
			globalCFG.serverIP = serverIP.Trim();
			Application.LoadLevel(scene);
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button("Zur"+char.ConvertFromUtf32(252)+"ck zum Hauptmen"+char.ConvertFromUtf32(252), GUILayout.MinHeight(30f))){
			this.ToggleVisibility();
			GetComponent<MainMenu>().ToggleVisibility();			
		}
		
	}

	protected override void resetView (){
		serverIP = "enter host IP";	
		
	}
}
