using UnityEngine;
using System.Collections;

public abstract class TransportingCharacterController : AbstractCharacterController {
	
	public Transform tractorBeam;
	ColonistController _attachedColonist;
	protected ColonistController attachedColonist {
		get {
			return _attachedColonist;
		}
	}
	
	public bool transporting {
		get {
			return tractorBeam.particleSystem.enableEmission;
		}
	}
	
	[RPC]
	public void OnAttach(ColonistController colonist) {
		_attachedColonist = colonist;
		tractorBeam.particleSystem.enableEmission = true;
	}
	
	[RPC]
	public virtual void OnDetach() {
		_attachedColonist = null;
		tractorBeam.particleSystem.enableEmission = false;
	}
	
	protected override void OnDie() {
		if (attachedColonist != null)
			facade.DetachColonist(attachedColonist);
	}
}
