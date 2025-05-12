using UnityEngine;

namespace State {
	namespace HUD {
		public class SystemFailureHUD : IState {
			
			public void Init() {
			}
			
			public void Update() {
				GUI.color = Color.red;
				GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height / 2 - Screen.height / 7, 150, 40), "SYSTEM FAILURE");
			}
		}
	}
}