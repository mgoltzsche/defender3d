using UnityEngine;
using System.Collections;

public class Lobby : AbstractGUIComponent {	
	
	NetworkWindow networkWindow;
	
	protected override void OnGUI(){
		base.OnGUI();
		if(render) {
			CenterComponentOnScreen();
			GUILayout.Window(1,position, DrawComponent, "Lobby");
		}
	}
	
	protected override void DrawComponent(int id) {
		GUILayout.BeginVertical("playerList");
		
		foreach (DefenderController defender in GameFacade.instance.defenders)
			GUILayout.Label(defender.gameObject.name);
		
		GUILayout.EndVertical();
		
		if (Network.isServer && GUILayout.Button("Start",GUILayout.MinHeight(30f)))
			GameFacade.instance.StartGame();
	}
	
	protected override void resetView() {}
}
