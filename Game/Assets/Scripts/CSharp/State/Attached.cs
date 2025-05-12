using UnityEngine;

namespace State {

	public class Attached : IState {
		
		ColonistController colonist;
		bool autoDetach;
		GameFacade facade;
		IState transition;
		
		public Attached(ColonistController colonist, bool autoDetach, IState transition) {
			this.colonist = colonist;
			this.autoDetach = autoDetach;
			this.transition = transition;
		}
		
		public void Init() {
			facade = GameFacade.instance;
		}
		
		public void Update() {
			if (autoDetach && colonist.transform.position.y < facade.terrain.GetMinY(colonist.transform.position) + 10) {
				TransportingCharacterController transporter = colonist.attachedTo;
				
				facade.DetachColonist(colonist);
				
				Debug.Log("colonist rescued");
				
				facade.Call(transporter, "SetPoints", transporter.points + colonist.pointsWorth);
				
				colonist.state = transition;
			}
		}
	}
}