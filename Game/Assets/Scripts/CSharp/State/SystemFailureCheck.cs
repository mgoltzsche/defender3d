using UnityEngine;

namespace State {

	public class SystemFailureCheck : IState {
		
		DefenderController ctx;
		float systemFailureDelay;
		
		public SystemFailureCheck(DefenderController ctx, float systemFailureDelay) {
			this.ctx = ctx;
			this.systemFailureDelay = systemFailureDelay;
		}
		
		public void Init() {
		}
		
		public void Update() {
			if (ctx.transform.position.y > GameFacade.instance.terrain.yMax && ctx.state.GetType() != typeof(SystemFailure))
				ctx.state = new SystemFailure(ctx, ctx.state, systemFailureDelay);
		}
	}
}