using UnityEngine;
using State.HUD;

namespace State {

	public class SystemFailure : IState {
		
		public float startTime;
		DefenderController ctx;
		IState transition, lastGuiState;
		float delay, drag;
		
		public SystemFailure(DefenderController ctx, IState transition, float delay) {
			this.ctx = ctx;
			this.transition = transition;
			this.delay = delay;
		}
		
		public void Init() {
			startTime = Time.time;
			ctx.rigidbody.useGravity = true;
			ctx.hudState = new SystemFailureHUD();
			
			ctx.rigidbody.AddForce(Vector3.down * 20000 + new Vector3(Random.Range(-3000, 3000), Random.Range(-3000, 3000), Random.Range(-3000, 3000)), ForceMode.Acceleration);
		}
		
		public void Update() {
			if (Time.time - startTime > delay) {
				ctx.rigidbody.useGravity = false;
				ctx.hudState = GameFacade.instance.GetDefaultHUD(ctx);
				ctx.state = transition;
			}
		}
	}
}