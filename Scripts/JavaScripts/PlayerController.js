function Start() {
}

function Update() {
	//transform.Translate(0, Input.GetAxis("Horizontal"), 0);
	//transform.Rotate(Input.GetAxis("Vertical"),Input.GetAxis("Horizontal"),0);
	
	//transform.Rotate(Input.GetAxis("Vertical"),Input.GetAxis("Horizontal"),0);
	
	/*var controller : CharacterController = GetComponent(CharacterController);
	
    // Rotate around y - axis
    //transform.Rotate(0, Input.GetAxis("Horizontal") * 5.0, 0);
    
    // Move forward / backward
    var relativeVelocity: Vector3 = transform.InverseTransformDirection(crigidbody.velocity);
    var move : Vector3 = Vector3(0, 5.0 * Input.GetAxis("Vertical"), 0);
    move += Vector3(0, 5.0 * Input.GetAxis("Vertical"), 0);
    
    controller.Move(move);*/
    
    
}

//@script RequireComponent(CharacterController)

function FixedUpdate() {
	var acceleration: float = 0.0;
	var relativePosition: Vector3 = rigidbody.position;
	var relativeRotation: Quaternion = rigidbody.rotation;
	relativeRotation.
	var relativeVelocity: Vector3 = transform.InverseTransformDirection(rigidbody.velocity);
	
	if (Input.GetKey(KeyCode.Q)) {
		relativeVelocity.z += 3;
	}
	
	if (Input.GetKey(KeyCode.Y)) {
		relativeVelocity.z -= 3;
	}
	
	relativePosition.z += relativeVelocity.z;
	rigidbody.MovePosition(relativePosition);
}