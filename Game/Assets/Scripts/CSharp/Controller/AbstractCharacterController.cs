using UnityEngine;
using System;
using System.Collections.Generic;
using State;

public abstract class AbstractCharacterController : MonoBehaviour {
	
	static private float VALID_SPAWN_DISTANCE = 300f;
	
	protected GameFacade facade;
	
	public Transform[] cannons, engines;
	public Transform shield;
	public int points, lifes = 1, pointsWorth = 1;
	public float maxAcceleration = 200f;
	public float maxVelocity = 100f;
	public float rotationSpeed = 5f;
	public float stability = 0.9f;
	public float stabilizationSpeed = 5f;
	public float characterDetectionDistance = 700f;
	public bool friendlyFire = false;
	
	public AudioClip weaponSound;
	public AudioClip engineSound;
	
	Vector3 currentAcceleration, currentAngularAcceleration;
	float lastProjectileHit;
	AbstractWeaponController[] weapons;
	IState obstacleAvoidanceState;
	ObstacleAvoidance obstacleAvoidance;
	bool _dead;
	public bool dead {
		get {
			return _dead;
		}
	}
	
	private IState _state = new NullState();
	protected IState dieState = new Die();
	public IState state {
		get {
			return _state;
		}
		set {
			_state = value;
			_state.Init();
		}
	}
	public IState continueState {
		set {
			_state = value;
		}
	}
	
	public virtual void Init(GameFacade facade) {
		this.facade = facade;
		obstacleAvoidance = new ObstacleAvoidance(this);
		obstacleAvoidanceState = new DelegatingState(obstacleAvoidance, new DetectAndAttackEnemies(this));
		rigidbody.isKinematic = true;
	}
	
	[RPC]
	public void EnableShield() {
		Debug.Log ("enable shield");
		if (shield != null) {
			shield.gameObject.SetActive(true);
			Invoke("DisableShield", 3f);
		}
	}
	[RPC]
	public void DisableShield() {
		if (shield != null)
			shield.gameObject.SetActive(false);
	}
	
	[RPC]
	public void SetPoints(int points) {
		this.points = points;
	}
	
	public Vector3 randomWaypoint {
		get {
			Vector3 p = facade.terrain.randomTerrainPosition;
			
			ModifyWaypointPosition(ref p);
			
			return p;
		}
	}
	
	protected virtual Vector3 RandomSpawnPosition() {
		while (true) {
			Vector3 p = facade.terrain.randomTerrainPosition;
			
			ModifySpawnPosition(ref p);
			
			if (!IsReservedTerrainPosition(p))
				return p;
		}
	}
	
	protected virtual void ModifySpawnPosition(ref Vector3 position) {
		position.y = facade.terrain.yMax + 100;
	}
	
	protected virtual void ModifyWaypointPosition(ref Vector3 position) {
		position.y = UnityEngine.Random.Range(position.y, facade.terrain.yMax);
	}
	
	protected virtual void ModifyObstacleNormal(ref Vector3 normal) {
	}
	
	protected virtual void FixedUpdate() {
		currentAcceleration = Vector3.zero;
		currentAngularAcceleration = Vector3.zero;
		
		_state.Update();
		AnimateEngine(currentAcceleration, currentAngularAcceleration);
		
		rigidbody.AddForce(currentAcceleration, ForceMode.Acceleration);
		rigidbody.AddTorque(currentAngularAcceleration, ForceMode.Acceleration);
	}
	
	public virtual void Spawn() {
		Vector3 centerDir = facade.terrain.center - transform.position;
		
		rigidbody.isKinematic = true;
		transform.position = RandomSpawnPosition();
		transform.rotation = Quaternion.LookRotation(centerDir);
		rigidbody.isKinematic = false;
		
		facade.Call(this, "SetDead", false);
		
		UseWeapons(cannons);
		ResetState();
		audio.Play();
	}
	
	[RPC]
	public void SetDead(bool dead) {
		_dead = dead;
		
		if (dead)
			AnimateDeath();
	}
	
	protected void UseWeapons(Transform[] useWeapons) {
		weapons = new AbstractWeaponController[useWeapons.Length];
		
		for (int i = 0; i < useWeapons.Length; i++) {
			weapons[i] = useWeapons[i].GetComponent<AbstractWeaponController>();
			weapons[i].Init(this);
		}
	}
	
	public virtual void ResetState() {
		_state = new NullState();
	}
	
	public virtual void UpdateAssociation() {
	}
	
	protected virtual void OnDie() {
		
		
	}
	
	public void DieLater() {
		if(audio != null && audio.isPlaying)
			audio.Stop();
		
		SetDead(true);
		OnDie();
		facade.Die(this);
		state = dieState;
		rigidbody.useGravity = true;
		Invoke("DieFinally", 5);
	}
	
	public void Die() {
		if(audio != null && audio.isPlaying)
			audio.Stop();
		
		_dead = true;
		OnDie();
		facade.Die(this);
		DieFinally();
	}
	
	void DieFinally() {
		if (--lifes > 0)
			Spawn();
		else {
			state = new NullState();
			facade.Call(this, "SetObjectInactive");
		}
	}
	
	[RPC]
	public void SetObjectInactive() {
		rigidbody.isKinematic = true;
		gameObject.SetActive(false);
	}
	
	protected virtual void AnimateDeath() {
		Detonator detonator = GetComponent<Detonator>();
		
		if(detonator != null)	
			detonator.Explode();
	}
	
	public void MoveTo(Vector3 position) {
		MoveTo(position, true);
	}
	
	public void MoveTo(Vector3 position, bool brake) {
		if (AvoidObstacles(position))
			return;
		
		Vector3 direction = position - rigidbody.position;
		
		//AvoidRigidbodyObstacles(ref direction);
		
		Vector3 acceleration = direction * 3;
		
		StabilizeAcceleration(ref acceleration);
		LimitAcceleration(ref acceleration);
		Accelerate(acceleration);
		
		if (brake)
			BrakeIfMovingAway(direction);
	}
	
	bool AvoidObstacles(Vector3 position) {
		RaycastHit hit;
		Vector3 testDirection;
		float testDistance;
		
		if (rigidbody.velocity.magnitude > 0.5) {
			testDirection = rigidbody.velocity.normalized;
			testDistance = rigidbody.velocity.magnitude;
		} else {
			testDirection = (position - transform.position).normalized;
			testDistance = 20;
		}
		
		Debug.DrawLine(transform.position, transform.position + testDirection * testDistance, Color.green);
		
		if (rigidbody.SweepTest(testDirection, out hit, testDistance)) {
			if (Vector3.Dot(hit.normal, testDirection) < -0.5) {
				Vector3 direction = transform.position - hit.point;
				Vector3 hitNormal;
				
				if (hit.rigidbody == null) {
					hitNormal = direction.normalized;
				} else {
					hitNormal = hit.rigidbody.velocity.normalized;
					
					if (CanCrashWith(hit.rigidbody))
						return false;
				}
				
				ModifyObstacleNormal(ref hitNormal);
				ModifyObstacleNormal(ref testDirection);
				
				Vector3 reflectedHitNormal = Vector3.Reflect(hitNormal, testDirection);
				
				if (Vector3.Dot(testDirection, hitNormal) < -0.7) {
					reflectedHitNormal += Vector3.Cross(reflectedHitNormal, Vector3.up) * UnityEngine.Random.Range(1, 50);
					reflectedHitNormal = reflectedHitNormal.normalized;
				}
				
				obstacleAvoidance.avoidancePosition = transform.position - reflectedHitNormal * (facade.waypointPrecision + 50);
				
				if (state != obstacleAvoidanceState) {
					obstacleAvoidance.transition = state;
					state = obstacleAvoidanceState;
					state.Update();
					
					return true;
				} else
					Debug.DrawLine(transform.position, obstacleAvoidance.avoidancePosition, Color.red);
			}
    	}
		
		return false;
	}
	
	protected virtual bool CanCrashWith(Rigidbody rigidbody) {
		return false;
	}
	
	public void Accelerate(Vector3 acceleration) {
		LimitVelocity(ref acceleration);
		
		currentAcceleration += acceleration;
	}
	
	void StabilizeAcceleration(ref Vector3 acceleration) {
		Vector3 normalizedDirection = acceleration.normalized;
		Vector3 vertical = Vector3.Cross(normalizedDirection, Vector3.up);
		Vector3 horizontal = Vector3.Cross(normalizedDirection, vertical);
		
		acceleration -= Vector3.Project(rigidbody.velocity, vertical) * 10;
		acceleration -= Vector3.Project(rigidbody.velocity, horizontal) * 10;
	}
	
	void BrakeIfMovingAway(Vector3 direction) {
		float velocityDirectionDistance = Vector3.Distance(rigidbody.velocity, direction);
		float distance = direction.magnitude;
		
		if (velocityDirectionDistance > distance)
			currentAcceleration -= rigidbody.velocity * 10 * (1 - distance / velocityDirectionDistance);
	}
	
	public virtual void AnimateEngine(Vector3 acceleration, Vector3 angularAcceleration) {
	}
	
	public virtual void TurnTo(Vector3 position) {
		Vector3 direction = (position - transform.position).normalized;
		Vector3 angularVelocity = rigidbody.angularVelocity;
		float velocityAngle = angularVelocity.magnitude * Mathf.Rad2Deg;
		Vector3 predictedUp = Quaternion.AngleAxis(velocityAngle, angularVelocity) * transform.forward;
		Vector3 rotation = Vector3.Cross(predictedUp, direction);
		
		Turn(rotation * rotationSpeed);
	}
	
	public void Turn(Vector3 rotation) {
		currentAngularAcceleration = rotation;
		StabilizeRotation();
	}
	
	public void StabilizeRotation() {
		if (currentAngularAcceleration.magnitude > 20)
			return;
		
		// calculate stabilization force (taken from http://wiki.unity3d.com/index.php?title=TorqueStabilizer)
		Vector3 stabilizationDirection = CalculateStabilizationDirection();
		
		Vector3 angularVelocity = rigidbody.angularVelocity;
		float velocityAngle = angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilizationSpeed;
		Vector3 predictedUp = Quaternion.AngleAxis(velocityAngle, angularVelocity) * transform.up;
		Vector3 stabilization = Vector3.Cross(predictedUp, stabilizationDirection);
		
		ProjectStabilization(ref stabilization);
		
		currentAngularAcceleration += stabilization * stabilizationSpeed * stabilizationSpeed;
	}
	
	protected virtual Vector3 CalculateStabilizationDirection() {
		Vector3 fwd = transform.forward;
		Vector3 verticalPlaneNormal = Vector3.Cross(fwd, -Vector3.up);
		Vector3 horizontalPlaneNormal = Vector3.Cross(fwd, verticalPlaneNormal);
		
		return horizontalPlaneNormal;
	}
	
	protected virtual void ProjectStabilization(ref Vector3 stabilization) {
		stabilization = Vector3.Project(stabilization, transform.forward);
	}
	
	public void ResetAiming() {
		foreach (AbstractWeaponController weapon in weapons)
			weapon.ResetAiming();
	}
	
	public void Aim(Vector3 targetPosition) {
		foreach (AbstractWeaponController weapon in weapons)
			weapon.Aim(targetPosition);
	}
	
	public void Fire() {
		foreach (AbstractWeaponController weapon in weapons)
			weapon.Fire();
	}
	
	public virtual void DetectAndAttackEnemies() {
	}
	
	protected void FireAtNearestEnemy(IEnumerable<AbstractCharacterController> enemies) {
		foreach (AbstractWeaponController weapon in weapons)
			weapon.FireAtNearestEnemy(enemies);
	}
	
	void LimitVelocity(ref Vector3 force) {
		if (rigidbody.velocity.magnitude > maxVelocity) {
			Vector3 normalizedVelocity = rigidbody.velocity.normalized;
			
			if (Vector3.Dot(normalizedVelocity, force.normalized) > 0)
				force -= Vector3.Project(force, normalizedVelocity);
		}
	}
	
	void LimitAcceleration(ref Vector3 force) {
		float forceMagnitude = force.magnitude;
		
		if (forceMagnitude > maxAcceleration)
			force *= maxAcceleration / forceMagnitude;
	}
	
	bool IsReservedTerrainPosition(Vector2 position) {
		foreach (AbstractCharacterController character in facade.allActiveCharacters)
			if (Vector2.Distance(position, new Vector2(character.transform.position.x, character.transform.position.z)) < VALID_SPAWN_DISTANCE)
				return true;
		
		return false;
	}
	
	[RPC]
	public void PlayLaserSound() {
		audio.PlayOneShot(weaponSound);
	}
}