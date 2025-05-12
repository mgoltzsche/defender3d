using UnityEngine;

namespace State {

	public class Mutate : IState {
		
		private LanderController lander;
		
		public Mutate(LanderController lander) {
			this.lander = lander;
		}
		
		public void Init() {
			lander.Mutate();
		}
		
		public void Update() {
		}
	}
}