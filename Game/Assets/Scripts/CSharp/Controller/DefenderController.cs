using UnityEngine;
using System;
using System.Collections;
using State;
using State.HUD;

public class DefenderController : TransportingCharacterController {
	
	public float rotationForce = 1000f;
	public float maxBeamDistance = 50f;
	public float systemFailureDelay = 1;
	
	/** HUD config */
	public Texture2D leftInActive, leftActive, rightInActive, rightActive, crosshairTexture, friendlyDefender, playerDefender;
	public int mapViewWidth = 400, mapViewHeight = 100;
	
	public AbstractCharacterController targetedCharacter;
	public IState hudState = new NullState();
	
	
	public override void Init(GameFacade facade) {
		base.Init(facade);
		GetComponent<DerivedEngineAnimationController>().enabled = false;
	}
	
	protected override void ModifySpawnPosition(ref Vector3 position) {
		position.y += (facade.terrain.yMax - position.y) / 4;
	}
	
	public override void AnimateEngine(Vector3 acceleration, Vector3 angularAcceleration) {
		float minLifetime = 0.05f;
		float maxLifetime = 0.2f;
		float maxDeltaAcceleration = maxAcceleration * Time.fixedDeltaTime;
		//Debug.DrawRay(transform.position + Vector3.up * 20, angularAcceleration * 5, Color.cyan);
		foreach (Transform engine in engines) {
			float engineAcceleration = 0;
			
			// calculate normalized engine acceleration
			if (Vector3.Dot(acceleration, engine.forward) < 0)
				engineAcceleration = Mathf.Min(1f, Vector3.Project(acceleration, engine.forward).magnitude / maxDeltaAcceleration) * 0.7f;
			
			// animate rotation
			Vector3 cross = Vector3.Cross(angularAcceleration, transform.forward);
			//cross = Vector3.Cross (cross, engine.forward);
			//Vector3 projection = Vector3.Project(cross, engine.forward);
			float accelerationEngineDot = Vector3.Dot(cross.normalized, engine.forward);
			Vector3 engineDirection = engine.position - transform.position;
			float engineDirectionDot = Vector3.Dot(engineDirection, transform.forward);
			float dot = -accelerationEngineDot;
			
			dot *= engineDirectionDot;
			
			if (dot > 0) {
				engineAcceleration += 0.3f * Mathf.Min(1f, dot * (cross.magnitude / 7));
			}
			
			// update particle lifetime
			engine.particleSystem.startLifetime = Mathf.Min(maxLifetime, minLifetime + (maxLifetime - minLifetime) * engineAcceleration);
		}
	}
	
	public override void ResetState() {
		facade.Call(this, "DisableShield");
		rigidbody.useGravity = false;
		state = new DelegatingState(new PlayerControlled(this, maxAcceleration, rotationForce), new SystemFailureCheck(this, systemFailureDelay), new AttachNearColonist(this));
		CameraController cameraCtrl = Camera.main.GetComponent<CameraController>();
		
		if (cameraCtrl.target == null)
			cameraCtrl.target = transform;
	}
	
	public void AttachNearColonist() {
		if (attachedColonist == null) {
			foreach (ColonistController colonist in facade.colonists) {
				Vector3 direction = colonist.transform.position - transform.position;
				
				if (direction.magnitude <= maxBeamDistance && Vector3.Dot(direction.normalized, -transform.up) > -0.1) {
					facade.AttachColonist(colonist, this, true);
					return;
				}
			}
		}
	}
	
	protected override void FixedUpdate() {
		targetedCharacter = null;
		base.FixedUpdate();
	}
	
	protected override void OnDie() {
		base.OnDie();
		hudState = new DeathHUD();
	}
	
	void OnGUI() {
		hudState.Update();
	}
	
	[RPC]
	public void SetPlayerName(String name) {
		this.name = name;
	}
	
}