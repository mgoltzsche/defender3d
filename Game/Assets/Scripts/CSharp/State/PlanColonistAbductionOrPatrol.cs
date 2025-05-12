using UnityEngine;

namespace State {

	public class PlanColonistAbductionOrPatrol : IState {
		
		LanderController lander;
		
		public PlanColonistAbductionOrPatrol(LanderController lander) {
			this.lander = lander;
		}
		
		public void Init() {
		}
		
		public void Update() {
			lander.PlanColonistAbductionOrPatrol();
		}
	}
}