using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using State;

public class LanderController : TransportingCharacterController {
	
	public GameObject mutantTemplate;
	ColonistController targetColonist;
	float lastAbductionTime;
	IState enemyDetection, abductionPlanning;
	
	public LanderController() {
		enemyDetection = new DetectAndAttackEnemies(this);
		abductionPlanning = new PlanColonistAbductionOrPatrol(this);
	}
	
	protected override void ModifySpawnPosition(ref Vector3 position) {
		position.y = facade.terrain.yMax + 400;
	}
	
	public override void DetectAndAttackEnemies() {
		FireAtNearestEnemy(DetectNearDefenders());
	}
	
	LinkedList<AbstractCharacterController> DetectNearDefenders() {
		LinkedList<AbstractCharacterController> defenders = new LinkedList<AbstractCharacterController>();
		
		foreach (DefenderController character in facade.defenders)
			if (Vector3.Distance(character.transform.position, transform.position) < characterDetectionDistance)
				defenders.AddLast(character);
		
		return defenders;
	}
	
	public void OnDrawGizmos() {
		if(targetColonist != null) {
			Color prefColor = Gizmos.color;
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(transform.position, targetColonist.transform.position);
			Gizmos.color = prefColor;
		}
		
		/*Vector3 scanStart = transform.position;
		Color colorTmp = Gizmos.color;
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(scanStart, scanStart + velocity * 100f);
		Gizmos.color = colorTmp;*/
	}
	
	protected override void OnDie() {
		targetColonist = null;
		base.OnDie();
	}
	
	public override void ResetState() {
		targetColonist = null;
		rigidbody.useGravity = false;
		state = abductionPlanning;
	}
	
	public override void TurnTo(Vector3 position) {
		Vector3 direction = (position - transform.position).normalized;
		Vector3 angularVelocity = rigidbody.angularVelocity;
		float velocityAngle = angularVelocity.magnitude * Mathf.Rad2Deg;
		Vector3 predictedUp = Quaternion.AngleAxis(velocityAngle, angularVelocity) * transform.forward;
		Vector3 rotation = Vector3.Cross(predictedUp, direction);
		
		rotation = Vector3.Project(rotation, transform.up);
		
		Turn(rotation * rotationSpeed);
	}
	
	public override void UpdateAssociation() {
		ResetState();
	}
	
	[RPC]
	public override void OnDetach() {
		base.OnDetach();
		ResetState();
	}
	
	public bool PerformColonistAbduction() {
		if (Time.time - lastAbductionTime > 5) {
			lastAbductionTime = Time.time;
			
			facade.AttachColonist(targetColonist, this, false);
			return true;
		}
		
		return false;
	}
	
	public void Mutate() {
		Vector3 mutantPosition = transform.position;
		ColonistController mutatingColonist = attachedColonist;
		
		GameFacade.instance.DetachColonist(attachedColonist);
		
		// respawn
		Spawn();
		
		// destroy colonist
		mutatingColonist.Die();
		
		// spawn mutant
		MutantController mutant = GameFacade.instance.InstantiateObject(mutantTemplate, mutantPosition, Quaternion.Euler(Vector3.zero)).GetComponent<MutantController>();
		mutant.Spawn();
		
	}
	
	public void PlanColonistAbductionOrPatrol() {
		// configure abduction of a near colonist
		ColonistController nearestColonist = null;
		float nearestDistance = float.MaxValue;
		
		foreach (ColonistController colonist in facade.colonists) {
			if (IsColonistAlreadyAssociated(colonist, facade.landers))
				continue;
			
			float distance = Vector3.Distance(transform.position, colonist.transform.position);
			
			if (nearestDistance > distance) {
				nearestDistance = distance;
				nearestColonist = colonist;
			}
		}
		
		if (nearestColonist == null)
			state = new MoveTo(this, randomWaypoint, abductionPlanning);
		else {
			targetColonist = nearestColonist;
			facade.Subscribe(this, targetColonist);
			
			// set states
			TerrainFacade t = facade.terrain;
			Vector3 mutationPosition = new Vector3(t.center.x, t.yMax + 400, t.center.z);
			IState mutateState = new Mutate(this);
			IState moveToMutationPosition = new DelegatingState(new MoveTo(this, mutationPosition, mutateState), enemyDetection);
			IState abductState = new Abduct(this, moveToMutationPosition);
			state = new DelegatingState(new Follow(this, targetColonist.rigidbody, false, 0f, Vector3.up * 60f, abductState), enemyDetection);
		}
	}
	
	bool IsColonistAlreadyAssociated(ColonistController colonist, IEnumerable<LanderController> landers) {
		foreach (LanderController lander in landers)
			if (lander.targetColonist == colonist)
				return true;
		
		return false;
	}
	
	protected override Vector3 CalculateStabilizationDirection() {
		return Vector3.up;
	}
	
	protected override void ProjectStabilization(ref Vector3 stabilization) {
	}
}
