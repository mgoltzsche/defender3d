using UnityEngine;
using State.HUD;

namespace State {

	public class PlayerControlled : IState {
	
		DefenderController defender;
		float moveForce;
		float rotationForce;
		float lastFireTime;
		
		public PlayerControlled(DefenderController defender, float moveForce, float rotationForce) {
			this.defender = defender;
			this.moveForce = moveForce;
			this.rotationForce = rotationForce;
		}
		
		public void Init() {
			Screen.lockCursor = true;
			defender.rigidbody.useGravity = false;
			defender.hudState = GameFacade.instance.GetDefaultHUD(defender);
		}
		
		public void Update() {
			// calculate torque force
			float torqueHor =  Input.GetAxis("Mouse X") * rotationForce * Time.fixedDeltaTime;
			float torqueVer = -Input.GetAxis("Mouse Y") * rotationForce * Time.fixedDeltaTime;
			
			// calculate strafe forces
			Transform camera = Camera.main.transform;
			Vector3 curMoveForce = GetKeyFactor(KeyCode.W, KeyCode.S) * moveForce * Time.fixedDeltaTime * camera.forward;
			Vector3 strafeHorForce = GetKeyFactor(KeyCode.A, KeyCode.D) * moveForce * Time.fixedDeltaTime * -camera.right;
			Vector3 strafeVerForce = GetKeyFactor(KeyCode.Q, KeyCode.Y) * moveForce * Time.fixedDeltaTime * camera.up;
			
			// apply forces
			defender.Accelerate(curMoveForce + strafeVerForce + strafeHorForce);
			defender.Turn(defender.transform.localToWorldMatrix.MultiplyVector(new Vector3(torqueVer, torqueHor, 0f)));
			
			Aim();
			Fire();
		}
		
		void Fire() {
			if (Input.GetButton("Fire1"))
				defender.Fire();
		}
		
		void Aim() {
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane + 20));
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, GameFacade.instance.terrain.diagonalLength, 1 << 0) && // ray hits character/environment
					Vector3.Dot(defender.transform.forward, (hit.point - defender.transform.position).normalized) > 0) { // hit point is in front
				if (hit.rigidbody != null)
					defender.targetedCharacter = hit.rigidbody.GetComponent<AbstractCharacterController>();
				
				defender.Aim(hit.point);
			} else
				defender.ResetAiming();
		}
		
		float GetKeyFactor(KeyCode keyPlus, KeyCode keyMinus) {
			if (Input.GetKey(keyPlus) && !Input.GetKey(keyMinus)) {
				return 1f;
			} else if (Input.GetKey(keyMinus) && !Input.GetKey(keyPlus)) {
				return -1f;
			} else
				return 0f;
		}
	}
}