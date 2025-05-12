using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour {
	
	public Transform explosionTemplate, smokeTemplate;
	AbstractCharacterController shooter;
	float lastInit, timeout;
	
	public void Init(AbstractCharacterController shooter, float timeout) {
		this.shooter = shooter;
		this.timeout = timeout;
		
		Idle();
	}
	
	public void Idle() {
		rigidbody.isKinematic = true;
		enabled = collider.enabled = false;
		rigidbody.position = Vector2.zero;
		particleSystem.enableEmission = false;
	}
	
	public void Fire(Vector3 position, Vector3 velocity) {
		rigidbody.isKinematic = true;
		rigidbody.position = position;
		rigidbody.rotation = Quaternion.LookRotation(velocity);
		rigidbody.isKinematic = false;
		rigidbody.velocity = velocity;
		particleSystem.enableEmission = true;
		enabled = collider.enabled = true;
		lastInit = Time.realtimeSinceStartup;
		
		Invoke("IdleIfTimeoutReached", timeout);
	}
	
	void IdleIfTimeoutReached() {
		if (Time.realtimeSinceStartup - lastInit > timeout)
			Idle();
	}
	
	void OnTriggerEnter(Collider c) {
		// destroy laser
		if (shooter != null) {
			Idle();
			
			/*// show smoke
			Transform smoke = (Transform) Instantiate(smokeTemplate, transform.position, transform.rotation);
			// destroy smoke after 5 seconds
			Destroy(smoke.gameObject, 5f);*/
			
			/*Vector3 targetPosition = c.transform.position;
			Quaternion targetRotation = c.transform.rotation;*/
			
			AbstractCharacterController victim = c.GetComponent<AbstractCharacterController>();
			
			if (victim != null && c.gameObject != shooter.gameObject && !victim.dead &&
					(shooter.friendlyFire || GameFacade.instance.IsEnemy(shooter, victim)) &&
					(victim.shield == null || !victim.shield.gameObject.activeSelf)) {
				GameFacade.instance.Hit(shooter, victim);
				//shooter.GetComponent<AbstractCharacterController>().points++;
				// show explosion: Doesn't work yet
	
				//Transform explosion = (Transform) Instantiate(explosionTemplate, targetPosition, targetRotation);
				//explosion.GetComponent<TimedObjectDestructor>().enabled = true;
			}
		}
	}
}
