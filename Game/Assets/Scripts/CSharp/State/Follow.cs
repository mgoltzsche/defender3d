using UnityEngine;

namespace State {

	public class Follow : IState {
		
		protected Rigidbody target;
		AbstractCharacterController ctx;
		bool careful;
		Vector3 offset;
		float distance;
		IState transition;
		
		public Follow(AbstractCharacterController character, Rigidbody target, bool careful, float distance, Vector3 offset, IState transition) {
			this.ctx = character;
			this.target = target;
			this.careful = careful;
			this.distance = distance;
			this.offset = offset;
			this.transition = transition;
		}
		
		public void Init() {
		}
		
		public void Update() {
			Vector3 targetPosition = target.position + offset;
			Vector3 direction = targetPosition - ctx.transform.position;
			
			if (careful) {
				// apply target velocity
				float targetVelocityMagnitude = target.velocity.magnitude;
				
				if (targetVelocityMagnitude != 0)
					targetPosition += target.velocity * (direction.magnitude * 0.2f / targetVelocityMagnitude);
			}
			
			float currentDistance = direction.magnitude;
			float correctedDistance = currentDistance - distance;
			float correctionFactor = correctedDistance / currentDistance;
			targetPosition += direction * correctionFactor - direction;
			
			ctx.MoveTo(targetPosition, careful);
			ctx.TurnTo(target.position);
			
			if (transition != null && correctedDistance < GameFacade.instance.waypointPrecision)
				ctx.state = transition;
		}
		
		public override string ToString() {
			return "Follow(" + target.name + ")";
		}
	}
}