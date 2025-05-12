using UnityEngine;
using System.Collections;

public class MainMenu : AbstractGUIComponent {	
	NetworkWindow networkWindow;

	public MainMenu(Rect that){
		this.position = that;
		Debug.Log("Konstruktor Rect");
	}
	
	protected override void OnGUI(){
		
		if(render) {
			base.OnGUI();
			CenterComponentOnScreen();
			GUILayout.Window(1,position, DrawComponent, "Defender");
		}
	}
	
	protected override void DrawComponent(int id){
		
		if(GUILayout.Button("Single",GUILayout.MinHeight(30f))) {
			Application.LoadLevel(1);
		}
		
		
		if(GUILayout.Button("COOP",GUILayout.MinHeight(30f))) {
			OpenNetworkWindow(2);
		}
		
		if(GUILayout.Button("Deathmatch",GUILayout.MinHeight(30f))) {
			OpenNetworkWindow(3);
		}
		
		if(GUILayout.Button("HighScore",GUILayout.MinHeight(30f))) {
			OpenHighScore();
		}
		
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button("Beenden", GUILayout.MinHeight(30f))){
			Application.Quit();
		}
		
	}
	
	private void OpenNetworkWindow(int scene) {
			this.ToggleVisibility();
			gameObject.AddComponent(typeof(NetworkWindow));
			networkWindow = (NetworkWindow) GetComponent(typeof(NetworkWindow));
			networkWindow.scene = scene;
			networkWindow.percentageWidth(.5f);
			networkWindow.percentageHeight(.8f);		
			
			networkWindow.ToggleVisibility();
	}
	
	private void OpenHighScore() {
			this.ToggleVisibility();
			gameObject.AddComponent(typeof(HighScoreView));
			HighScoreView highScore = (HighScoreView) GetComponent(typeof(HighScoreView));
			highScore.percentageWidth(.5f);
			highScore.percentageHeight(.8f);		
			
			highScore.ToggleVisibility();
	}
	
	protected override void resetView (){
	
	}
}
