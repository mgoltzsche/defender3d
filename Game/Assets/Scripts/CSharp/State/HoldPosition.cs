using UnityEngine;

namespace State {

	public class HoldPosition : IState {
		
		AbstractCharacterController ctx;
		float seconds, startTime;
		IState transition;
		Vector3 position;
		
		public HoldPosition(AbstractCharacterController ctx, Vector3 position, float seconds, IState transition) {
			this.ctx = ctx;
			this.position = position;
			this.seconds = seconds;
			this.transition = transition;
		}
		
		public void Init() {
			startTime = Time.time;
		}
		
		public void Update() {
			ctx.MoveTo(position);
			ctx.StabilizeRotation();
			
			if (Time.time - startTime > seconds)
				ctx.state = transition;
		}
	}
}
