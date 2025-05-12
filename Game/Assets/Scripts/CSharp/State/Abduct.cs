using UnityEngine;
using System.Collections;

namespace State {
	public class Abduct : IState {
		
		private LanderController lander;
		private IState transition;
		private Vector3 position;
		
		public Abduct(LanderController lander, IState transition) {
			this.lander = lander;
			this.transition = transition;
		}
		
		public void Init() {
			position = lander.transform.position;
			
			if (lander.PerformColonistAbduction())
				lander.state = new DelegatingState(new HoldPosition(lander, position, 5f, transition), new DetectAndAttackEnemies(lander));
			else
				lander.ResetState();
		}
		
		public void Update() {
		}
	}
}
