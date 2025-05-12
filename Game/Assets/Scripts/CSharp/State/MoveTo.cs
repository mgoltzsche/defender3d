using UnityEngine;

namespace State {

	public class MoveTo : IState {
		
		Vector3 targetPosition;
		IState transition;
		AbstractCharacterController ctx;
		
		public MoveTo(AbstractCharacterController ctx, IState transition) {
			this.ctx = ctx;
			this.transition = transition;
		}
		
		public MoveTo(AbstractCharacterController ctx, Vector3 targetPosition, IState transition) {
			this.ctx = ctx;
			this.targetPosition = targetPosition;
			this.transition = transition;
		}
		
		public void Init() {
		}
		
		public void Update() {
			ctx.MoveTo(targetPosition, true);
			ctx.TurnTo(targetPosition);
			
			if (Vector3.Distance(ctx.transform.position, targetPosition) < GameFacade.instance.waypointPrecision)
				ctx.state = transition;
		}
	}
}