using UnityEngine;
using System.Collections;

public class FlashController : MonoBehaviour {
	
	public float duration;
	public float delay;
	public float areaFlashCount;
	public float areaSize;
	public float terrainOffset;
	
	public Light globalLight;
	
	
	private Terrain terrain;
	
	// Use this for initialization
	void Start () {		
		terrain = Terrain.activeTerrain;		
		InvokeRepeating("StartRandomFlash", 0, delay);
		globalLight.enabled = true;
	}
	
	
	
	void StartRandomFlash() {	
		
	    transform.position = getRandomPosition(terrain.transform.position, terrain.terrainData.size, terrainOffset);	
		
		StartCoroutine(Flash());
    }
	
	IEnumerator Flash(){
		Vector3 currentPosition = this.transform.position;
		float intens = globalLight.intensity;
		this.light.enabled = true;
		globalLight.transform.position = currentPosition;
		
		for(int i = 0; i <= areaFlashCount ; i++) {
			yield return new WaitForSeconds(duration / areaFlashCount);
			transform.position = getRandomPosition(currentPosition, new Vector3(areaSize ,0 ,areaSize), 100);	
			if (i % 3 == Mathf.Round(UnityEngine.Random.value) )
			
				globalLight.intensity *= 1.5f;
			else
				globalLight.intensity = intens;
				
		}
		
		this.light.enabled = false;
		globalLight.intensity = intens;
	}
	
	private Vector3 getRandomPosition(Vector3 position, Vector3 rect, float offset){
		
		float offsetWidth = rect.x * offset / 100;
		float offsetDepth = rect.z * offset / 100;
		
		float minX = position.x + offsetWidth;
		float maxX = position.x + rect.x - offsetWidth;
		
		float minZ = position.z + offsetDepth;
		float maxZ = position.z + rect.z - offsetDepth;
			
			
			
			
		float x = UnityEngine.Random.Range(minX , maxX);
		float y = terrain.terrainData.size.y;
		float z = UnityEngine.Random.Range(minZ ,maxZ);
		
		return new Vector3(x,y,z);
		
	}
}
