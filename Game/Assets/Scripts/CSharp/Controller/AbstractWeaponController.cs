using UnityEngine;
using System.Collections.Generic;

public abstract class AbstractWeaponController : MonoBehaviour {
	
	public float angularSpeed = 100f;
	public float targetableArea = 0.1f;
	protected AbstractCharacterController shooter;
	Quaternion defaultLocalRotation;
	
	public void Init(AbstractCharacterController shooter) {
		this.shooter = shooter;
		
		defaultLocalRotation = transform.localRotation;
		
		InstantiateProjectiles();
	}
	
	protected abstract void InstantiateProjectiles();
	
	public void ResetAiming() {
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, defaultLocalRotation, Time.fixedDeltaTime * angularSpeed);
	}
	
	public void FireAtNearestEnemy(IEnumerable<AbstractCharacterController> nearEnemies) {
		float nearestDistance = float.MaxValue;
		AbstractCharacterController nearestEnemy = null;
		
		foreach (AbstractCharacterController enemy in nearEnemies) {
			Vector3 direction = enemy.transform.position - transform.position;
			float distance = direction.magnitude;
			
			if (distance < nearestDistance) {
				direction = direction.normalized;
				float directionForwardDotProduct = Vector3.Dot(direction, transform.forward);
				
				if (directionForwardDotProduct > targetableArea) {
					nearestDistance = distance;
					nearestEnemy = enemy;
				}
			}
		}
		
		if (nearestEnemy != null && Aim(nearestEnemy.transform.position))
			Fire();
	}
	
	public bool Aim(Vector3 targetPosition) {
		Vector3 targetDirection = (targetPosition - transform.position).normalized;
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * angularSpeed);
		float aimPrecision = Vector3.Dot(targetDirection, transform.forward);
		return aimPrecision > 0.95f; // return true if target could be hit
	}
	
	public abstract void Fire();
}
