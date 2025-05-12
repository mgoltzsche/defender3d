using UnityEngine;
using System.Collections.Generic;

namespace State {

	public class DetectAndAttackEnemies : IState {
		
		AbstractCharacterController c;
		
		public DetectAndAttackEnemies(AbstractCharacterController character) {
			c = character;
		}
		
		public void Init() {
		}
		
		public void Update() {
			c.DetectAndAttackEnemies();
		}
		
		public override string ToString() {
			return "DetectAndAttackNearDefenders";
		}
	}
}