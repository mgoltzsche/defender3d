using UnityEngine;
using System.Collections;

public class LaserCannonController : AbstractWeaponController {

	public float projectileSpeed = 2000f;
	public float projectileTimeout = 5f;
	public float delay = 0.1f;
	public float cooldownTime = 1.3f;
	public float maxOverheat = 15f;
	public GameObject laserTemplate;
	LaserController[] laserInstances;
	int totalProjectileAmount;
	int currentLaserIndex = 0;
	float lastShotTime, _overheat;
	public float overheat {
		get {
			return _overheat;
		}
	}
	
	protected override void InstantiateProjectiles() {
		totalProjectileAmount = (int) Mathf.Ceil(projectileTimeout / delay);
		
		laserInstances = new LaserController[totalProjectileAmount];
		
		for (int i = 0; i < totalProjectileAmount; i++) {
			laserInstances[i] = GameFacade.instance.InstantiateObject(laserTemplate).GetComponent<LaserController>();
			laserInstances[i].Init(shooter, projectileTimeout);
		}
	}
	
	void Update() {
		_overheat = Mathf.Max(0f, _overheat - cooldownTime * Time.deltaTime);
	}
	
	public override void Fire() {
		float shotDeltaTime = Time.time - lastShotTime;
		
		if (shotDeltaTime > delay && _overheat + 1 <= maxOverheat) {
			Vector3 shotDirection = transform.forward;
			Vector3 shooterVelocity = shooter.rigidbody.velocity;
			Vector3 laserPosition = transform.position + shotDirection * 20f;
			Vector3 laserVelocity = projectileSpeed * shotDirection;
			
			if (Vector3.Dot(shooterVelocity.normalized, transform.forward) > 0) {
				Vector3 addedShooterVelocity = Vector3.Project(shooterVelocity, shotDirection);
				laserPosition += addedShooterVelocity * Time.fixedDeltaTime;
				laserVelocity += addedShooterVelocity;
			}
			
			LaserController laser = laserInstances[currentLaserIndex];
			laser.Fire(laserPosition, laserVelocity);
			
			currentLaserIndex = ++currentLaserIndex % totalProjectileAmount;
			lastShotTime = Time.time;
			_overheat++;
			GameFacade.instance.Call(shooter, "PlayLaserSound");
		}
	}
}
