using UnityEngine;

namespace State {

	public class ObstacleAvoidance : IState {
		
		public Vector3 avoidancePosition;
		public IState transition;
		AbstractCharacterController ctx;
		
		public ObstacleAvoidance(AbstractCharacterController ctx) {
			this.ctx = ctx;
		}
		
		public void Init() {
		}
		
		public void Update() {
			ctx.MoveTo(avoidancePosition, true);
			ctx.TurnTo(avoidancePosition);
			
			if (Vector3.Distance(ctx.transform.position, avoidancePosition) < GameFacade.instance.waypointPrecision)
				ctx.continueState = transition;
		}
	}
}