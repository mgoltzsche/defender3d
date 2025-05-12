using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using State;

public class MutantController : AbstractCharacterController {
	
	IState defaultState;
	
	public MutantController() {
		defaultState = new DelegatingState(new Patrol(this), new PlanDefenderDestruction(this));
	}
	
	public override void Spawn() {
		rigidbody.isKinematic = false;
		ResetState();
	}
	
	public override void ResetState() {
		state = defaultState;
	}
	
	public override void UpdateAssociation() {
		ResetState();
	}
	
	public void AttackDefenderIfAvailable() {
		DefenderController targetDefender = facade.anyDefender;
		Debug.Log ("defender available: " + (targetDefender != null));
		
		if (targetDefender != null) {
			Debug.Log ("mutant: defender associated: " + targetDefender.name);
			facade.Subscribe(this, targetDefender);
			state = new Follow(this, targetDefender.rigidbody, false, -100f, Vector3.zero, defaultState);
		}
	}
	
	protected override bool CanCrashWith(Rigidbody rigidbody) {
		return rigidbody.GetComponent<DefenderController>() != null;
	}
	
	void OnCollisionEnter(Collision c) {
		DefenderController defender = c.gameObject.GetComponent<DefenderController>();
		
		if (defender != null)
			GameFacade.instance.Hit(this, defender);
	}
}