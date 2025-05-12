using UnityEngine;
using System;
using System.Collections.Generic;
using State;

public class SaucererController : AbstractCharacterController {
	
	DefenderController targetDefender;
	IState patrolState, detectAndAttackDefenders;
	
	public SaucererController() {
		detectAndAttackDefenders = new DetectAndAttackEnemies(this);
		patrolState = new DelegatingState(new Patrol(this), detectAndAttackDefenders);
	}
	
	public override void ResetState() {
		targetDefender = null;
		rigidbody.useGravity = false;
		state = patrolState;
	}
	
	public override void DetectAndAttackEnemies() {
		LinkedList<AbstractCharacterController> detectedEnemies = new LinkedList<AbstractCharacterController>();
		DefenderController nearestDefender = null;
		float nearestDistance = characterDetectionDistance;
		
		if (targetDefender != null) {
			float targetDefenderDistance = Vector3.Distance(transform.position, targetDefender.transform.position);
			
			if (targetDefenderDistance <= characterDetectionDistance)
				nearestDistance = characterDetectionDistance;
			else {
				targetDefender = null;
				//state = patrolState;
			}
		}
		
		foreach (DefenderController defender in facade.defenders) {
			float distance = Vector3.Distance(transform.position, defender.transform.position);
			
			if (distance < characterDetectionDistance)
				detectedEnemies.AddLast(defender);
			
			if (distance < nearestDistance) {
				nearestDistance = distance;
				nearestDefender = defender;
			}
		}
		
		if (nearestDefender != null && nearestDefender != targetDefender) {
			targetDefender = nearestDefender;
			state = new DelegatingState(new Follow(this, targetDefender.rigidbody, true, 300f, Vector3.zero, null), detectAndAttackDefenders);
			facade.Subscribe(this, targetDefender);
		}
		
		// fire at near defenders
		FireAtNearestEnemy(detectedEnemies);
	}
	
	public override void UpdateAssociation () {
		ResetState();
	}
	
	protected override void ModifySpawnPosition(ref Vector3 position) {
		position.y = UnityEngine.Random.Range(position.y, facade.terrain.yMax);
	}
}
