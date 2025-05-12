using UnityEngine;
using System.Collections;

namespace State {
	namespace HUD {

		public class SpaceshipHUD : IState {
			
			DefenderController defender;
			Texture2D leftInActive, leftActive, rightInActive, rightActive;
			
			public SpaceshipHUD(DefenderController player) {
				this.defender = player;
				leftInActive = player.leftInActive;
				leftActive = player.leftActive;
				rightInActive = player.rightInActive;
				rightActive = player.rightActive;			
			}
			
			public void Init() {
			}
			
			public void Update () {
				DrawCrosshair();
				DrawTargetedCharacterLabel();
				DrawVelocityAndOverheatDisplay();
				DrawColonistRescueInfo();
			}
			
			void DrawCrosshair() {
				Rect crosshairPosition = new Rect(Screen.width / 2, Screen.height / 2, 0, 0);
				CenterRect(ref crosshairPosition, defender.crosshairTexture);
				GUI.DrawTexture(crosshairPosition, defender.crosshairTexture);
				
			}
			
			void DrawTargetedCharacterLabel() {
				if (defender.targetedCharacter != null) {
					if (defender.targetedCharacter.dead)
						GUI.color = Color.white;
					else
						GUI.color = GameFacade.instance.IsEnemy(defender, defender.targetedCharacter) ? Color.red : Color.green;
					
					string characterName = defender.targetedCharacter.name.Length == "(Clone)".Length
						? defender.targetedCharacter.name
						: defender.targetedCharacter.name.Replace("(Clone)", "");
					
					GUI.Label(new Rect(Screen.width / 2f + 50, Screen.height / 2f - 50, 200, 20), characterName);
				}
			}
			
			void DrawVelocityAndOverheatDisplay() {
				float boxX = Screen.width - leftInActive.width;
				float boxY = Screen.height - leftInActive.height;
				float boxWidth = leftInActive.width;
				float boxHeight = leftInActive.height;
				
				float speed = defender.rigidbody.velocity.magnitude / defender.maxVelocity;
				LaserCannonController laserCannon = defender.cannons[0].GetComponent<LaserCannonController>();
				float overheat = laserCannon.overheat / laserCannon.maxOverheat;
				
				GUI.color = Color.white;
				
				//LeftBox
				GUI.BeginGroup(new Rect(0, boxY, boxWidth, boxHeight));	
					GUI.DrawTexture(new Rect(0,0,leftInActive.width,leftInActive.height), leftInActive);		
				GUI.EndGroup();
				
				GUI.BeginGroup(new Rect(0, boxY + (boxHeight - boxHeight*speed), boxWidth, boxHeight*speed));				
					GUI.DrawTexture(new Rect(0,-(leftActive.height - leftActive.height*speed),leftActive.width, leftActive.height), leftActive);
				GUI.EndGroup();
				
				//RightBox
				GUI.BeginGroup(new Rect(boxX, boxY, boxWidth, boxHeight));				
					GUI.DrawTexture(new Rect(0,0,rightInActive.width,rightInActive.height), rightInActive);
				GUI.EndGroup();
				
				GUI.BeginGroup(new Rect(boxX, boxY + (boxHeight - boxHeight*overheat), boxWidth, boxHeight));	
					GUI.DrawTexture(new Rect(0,-(rightActive.height - rightActive.height*overheat),rightActive.width,rightActive.height), rightActive);		
				GUI.EndGroup();
			}
			
			void DrawColonistRescueInfo() {
				if (defender.transporting) {
					GUI.color = Color.green;
					GUI.Label(new Rect(Screen.width - 200, Screen.height - 30, 150, 20), "Rescue colonist!");
				}
			}
			
			void CenterRect(ref Rect position, Texture2D texture) {
				position.x -= texture.width / 2;
				position.y -= texture.height / 2;
				position.width = texture.width;
				position.height = texture.height;
			}
		}
	}
}