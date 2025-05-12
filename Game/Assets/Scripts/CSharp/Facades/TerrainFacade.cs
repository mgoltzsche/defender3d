using UnityEngine;
using System.Collections;

public class TerrainFacade {
	
	static float PADDING = 300;
	
	Terrain terrain;
	Vector3 _center;
	float _diagonalLength, _yMax, _xMin, _xMax, _zMin, _zMax, _width, _height, _altitude;
	
	public Vector3 center {
		get {
			return _center;
		}
	}
	public float diagonalLength {
		get {
			return _diagonalLength;
		}
	}
	public float yMax {
		get {
			return _yMax;
		}
	}
	public float xMin {
		get {
			return _xMin;
		}
	}
	public float xMax {
		get {
			return _xMax;
		}
	}
	public float zMin {
		get {
			return _zMin;
		}
	}
	public float zMax {
		get {
			return _zMax;
		}
	}
	public float width {
		get {
			return _width;
		}
	}
	public float height {
		get {
			return _height;
		}
	}
	
	public Vector3 randomTerrainPosition {
		get {
			Vector3 position;
			
			do {
				position = new Vector3(UnityEngine.Random.Range(_xMin, _xMax),
					0f,
					UnityEngine.Random.Range(_zMin, _zMax));
				position.y = GetMinY(position);
			} while (position.y > _yMax - _altitude / 3f);
			
			return position;
		}
	}
	
	public TerrainFacade() {
		terrain = Terrain.activeTerrain;
		Vector3 size = terrain.terrainData.size;
		
		_center = terrain.transform.position + terrain.terrainData.size / 2;
		_yMax = terrain.transform.position.y + terrain.terrainData.size.y;
		_diagonalLength = terrain.terrainData.size.magnitude;
		_xMin = terrain.transform.position.x;
		_zMin = terrain.transform.position.z;
		_xMax = _xMin + size.x;
		_zMax = _zMin + size.z;
		_xMin += PADDING;
		_zMin += PADDING;
		_xMax -= PADDING;
		_zMax -= PADDING;
		_yMax -= PADDING;
		_width = size.x;
		_height = size.z;
		_altitude = size.y;
	}
	
	public float GetMinY(Vector3 position) {
		return terrain.SampleHeight(position) + 1;
	}
}
