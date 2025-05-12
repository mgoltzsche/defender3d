using UnityEngine;

namespace State {

	public class Patrol : IState {
		
		AbstractCharacterController ctx;
		Vector3 targetPosition;
		
		public Patrol(AbstractCharacterController ctx) {
			this.ctx = ctx;
		}
		
		public void Init() {
			targetPosition = ctx.randomWaypoint;
		}
		
		public void Update() {
			ctx.MoveTo(targetPosition, false);
			ctx.TurnTo(targetPosition);
			
			if (Vector3.Distance(ctx.transform.position, targetPosition) < GameFacade.instance.waypointPrecision)
				ctx.state = this;
		}
		
		public override string ToString() {
			return "Patrol";
		}
	}
}	
