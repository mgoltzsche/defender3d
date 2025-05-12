using UnityEngine;

namespace State {

	public class AttachNearColonist : IState {
		
		DefenderController ctx;
		
		public AttachNearColonist(DefenderController ctx) {
			this.ctx = ctx;
		}
		
		public void Init() {
		}
		
		public void Update() {
			ctx.AttachNearColonist();
		}
	}
}