using UnityEngine;
using System.Collections;

public class RotationController : MonoBehaviour {
	
	public float speed = 230;
	
	void Update () {
		transform.Rotate(Vector3.up * speed * Time.deltaTime);
	}
}
