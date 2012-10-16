var maxTorque : float = 1.0f;
var forwardForce : float = 10.0f;

var currentTorqueHor : float = 0.0f;
var currentTorqueVer : float = 0.0f;

function Update() {
   currentTorqueHor = maxTorque * Input.GetAxis("Horizontal");
   currentTorqueVer = maxTorque * Input.GetAxis("Vertical");
}

function FixedUpdate() {
   rigidbody.AddForce(transform.forward * forwardForce);
   rigidbody.AddRelativeTorque(currentTorqueVer, 0.0f, -currentTorqueHor);
}