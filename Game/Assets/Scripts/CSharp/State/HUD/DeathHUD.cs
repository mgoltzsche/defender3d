using UnityEngine;

namespace State {
	namespace HUD {
		public class DeathHUD : IState {
			
			public void Init() {
			}
			
			public void Update() {
				GUI.color = Color.red;
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - Screen.height / 7, 200, 40), "YOU HAVE BEEN DESTROYED!");
			}
		}
	}
}