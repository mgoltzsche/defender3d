using UnityEngine;
using System.Collections.Generic;

namespace State {

	public class PlanDefenderDestruction : IState {
		
		MutantController mutant;
		
		public PlanDefenderDestruction(MutantController mutant) {
			this.mutant = mutant;
		}
		
		public void Init() {
			mutant.AttackDefenderIfAvailable();
		}
		
		public void Update() {
		}
	}
}