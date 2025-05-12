using UnityEngine;
using System.Collections;
using State;

public class ColonistController : AbstractCharacterController {
	
	IState patrol, fall, Attached;
	ConfigurableJoint joint;
	bool _attached;
	public bool attached {
		get {
			return _attached;
		}
	}
	
	public ColonistController() {
		patrol = new Patrol(this);
		fall = new Falling(this);
	}
	
	public override void Init(GameFacade facade) {
		joint = GetComponent<ConfigurableJoint>();
		base.Init(facade);
	}
	
	public override void ResetState() {
		state = patrol;
		rigidbody.useGravity = true;
		
		joint.connectedBody = null;
		joint.xMotion = ConfigurableJointMotion.Free;
		joint.yMotion = ConfigurableJointMotion.Free;
		joint.zMotion = ConfigurableJointMotion.Free;
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
	
	[RPC]
	public void SetAttached(bool attached) {
		_attached = attached;
	}
	
	public TransportingCharacterController attachedTo {
		get {
			return joint.connectedBody == null
				? null
				: joint.connectedBody.GetComponent<TransportingCharacterController>();
		}
	}
	
	public bool attachable {
		get {
			return state.GetType() == typeof(Falling) || attached;
		}
	}
	
	public void AttachTo(TransportingCharacterController character, bool autoDetach) {
		facade.Call(this, "SetAttached", true);
		
		transform.position = character.transform.position - character.transform.up * 50;
		joint.connectedBody = character.rigidbody;
		joint.xMotion = ConfigurableJointMotion.Limited;
		joint.yMotion = ConfigurableJointMotion.Limited;
		joint.zMotion = ConfigurableJointMotion.Limited;
		
		state = new Attached(this, autoDetach, patrol);
	}
	
	public void Detach() {
		facade.Call(this, "SetAttached", false);
		
		rigidbody.velocity = Vector3.zero;
		
		joint.xMotion = ConfigurableJointMotion.Free;
		joint.yMotion = ConfigurableJointMotion.Free;
		joint.zMotion = ConfigurableJointMotion.Free;
		
		state = fall;
	}
	
	/*public override void Turn(Vector3 direction) {
		
	}*/
	
	protected override void OnDie() {
		if (joint.connectedBody != null)
			facade.DetachColonist(this);
	}
	
	public bool isRescueable() {
		ConfigurableJoint joint = GetComponent<ConfigurableJoint>();
		
		return state == fall || joint.connectedBody != null && joint.connectedBody.GetComponent<LanderController>() != null;
	}
	
	protected override void ModifySpawnPosition(ref Vector3 position) {
	}
	
	protected override void ModifyWaypointPosition(ref Vector3 position) {
	}
	
	protected override void ModifyObstacleNormal(ref Vector3 normal) {
		normal.y = 0;
		normal = normal.normalized;
	}
	
	protected override Vector3 CalculateStabilizationDirection() {
		return Vector3.up;
	}
	
	protected override void ProjectStabilization(ref Vector3 stabilization) {
	}
}
