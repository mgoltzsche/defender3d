using UnityEngine;
using System.Collections;

public class DerivedEngineAnimationController : MonoBehaviour {
	
	Vector3 lastPosition, lastVelocity, lastEulerAngles, lastAngularVelocity;
	public Vector3 acceleration;
	
	void FixedUpdate () {
		DefenderController defender = GetComponent<DefenderController>();
		
		if (defender.dead) {
			defender.AnimateEngine(Vector3.zero, Vector3.zero);
			return;
		}
		
		Vector3 velocity = transform.position - lastPosition / Time.fixedDeltaTime;
		Vector3 angularVelocity = transform.rotation.eulerAngles - lastEulerAngles / Time.fixedDeltaTime;
		
		acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;
		Vector3 angularAcceleration = (angularVelocity - lastAngularVelocity) / Time.fixedDeltaTime;
		
		lastPosition = transform.position;
		lastVelocity = velocity;
		lastEulerAngles = transform.rotation.eulerAngles;
		lastAngularVelocity = angularVelocity;
		
		defender.AnimateEngine(-acceleration, -angularAcceleration);
	}
}
