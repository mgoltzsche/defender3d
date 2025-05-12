using UnityEngine;

namespace State {
	namespace HUD {
		public class MenuHUD : IState {
			
			private InGameMenu menu;
			
			public MenuHUD(DefenderController parent){
				parent.gameObject.AddComponent<InGameMenu>();
				menu = parent.gameObject.GetComponent<InGameMenu>();
				menu.percentageWidth(.5f);
				menu.percentageHeight(.8f);	
				menu.ToggleVisibility();
			}
			
			public void Init() {			
			}
			
			public void Update() {				
			}
		}
	}
}