using UnityEngine;

namespace State {
	namespace HUD {
		public class RadarHUD : IState {
			
			DefenderController defender;
			GameLogic gameLogic;
			GameFacade facade;
			
			public RadarHUD(DefenderController defender){
				this.defender = defender;
				facade = GameFacade.instance;
				
				
				
			}
			
			public void Init() {			
			}
			
			public void Update() {
				Matrix4x4 bu = GUI.matrix;
				int mapViewWidth = defender.mapViewWidth;
				int mapViewHeight = defender.mapViewHeight;
				int mapMinX = Screen.width / 2 - mapViewWidth / 2;
				GUI.Box(new Rect(mapMinX, 0, mapViewWidth, mapViewHeight), "");
				
				foreach (AbstractCharacterController character in facade.allActiveCharacters) {
					bool isColonist = character is ColonistController;
					bool active = isColonist && ((ColonistController) character).attached;
					

					/*
					GUI.color = defender == character ? Color.blue : 
						isColonist ? Color.white : 
						facade.IsEnemy(defender, character) ? Color.red : Color.green;
					*/
					int x = mapMinX + (int) Mathf.Round((character.transform.position.z - facade.terrain.zMin) / facade.terrain.height * mapViewWidth) - 5;
					int y = (int) Mathf.Round((character.transform.position.x - facade.terrain.xMin) / facade.terrain.width * mapViewHeight) - 5;
					
					if(defender == character) {
						GUI.color = Color.blue;
						GUIUtility.RotateAroundPivot(defender.transform.eulerAngles.y+90, new Vector2(x+defender.playerDefender.width/2, y + defender.playerDefender.height/2));
						GUI.DrawTexture(new Rect(x,y, defender.playerDefender.width, defender.playerDefender.height), defender.playerDefender);
					}else if (isColonist){
						GUI.color = Color.white;
						GUI.Label(new Rect(x, y, 20, 20), character.dead ? "x" : "o");
					} else if (facade.IsEnemy(defender, character)){
						GUI.color = Color.red;
						GUI.Label(new Rect(x, y, 20, 20), character.dead ? "x" : "o");
					} else {
						DefenderController tmpPlayer = (DefenderController) character;
						GUI.color = Color.green;
						GUIUtility.RotateAroundPivot(tmpPlayer.transform.eulerAngles.y+90, new Vector2(x+tmpPlayer.friendlyDefender.width/2, y + tmpPlayer.friendlyDefender.height/2));
						GUI.DrawTexture(new Rect(x,y, tmpPlayer.friendlyDefender.width, tmpPlayer.friendlyDefender.height), tmpPlayer.friendlyDefender);
					}
					
					if (active)
						GUI.Label(new Rect(x, y, 20, 20), "| |");
					
					
					
					GUI.matrix = bu;
				}
			}
		}
	}
}