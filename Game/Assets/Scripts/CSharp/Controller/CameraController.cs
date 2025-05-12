using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public Transform target;
	public float smooth = 20f;
	public float distance = 20f;
	public float height = 5f;
	
	Vector3 velocity = Vector3.zero;

	void FixedUpdate() {
		if (target == null) {
			TerrainFacade terrain = GameFacade.instance.terrain;
			Vector3 center = terrain.center;
			transform.position = new Vector3(terrain.width / 10, terrain.center.y, terrain.height / 10);
			transform.LookAt(center);
		} else {
			Vector3 exactPosition = ExactPosition();
			Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, exactPosition, ref velocity, smooth * Time.fixedDeltaTime);
			Quaternion targetRotation = Quaternion.LookRotation(target.forward, Vector3.up);
			
			transform.position = smoothPosition;
			transform.rotation = targetRotation;
		}
	}

	Vector3 ExactPosition() {
		Vector3 relativeHeight = transform.up * height;
		Vector3 relativeDistance = -target.forward * distance;
		
		return target.position + relativeDistance + relativeHeight;
	}
}