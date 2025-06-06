using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Detonator))]
[AddComponentMenu("Detonator/Sound")]
public class DetonatorSound : DetonatorComponent {
	
	public AudioClip[] nearSounds;
	public AudioClip[] farSounds;
	
	public float distanceThreshold = 50f; //threshold in m between playing nearSound and farSound
	public float minVolume = .4f;
	public float maxVolume = 1f;
	public AudioRolloffMode rolloffFactor = AudioRolloffMode.Logarithmic;
	
	private AudioSource _soundComponent;
	private bool _delayedExplosionStarted = false;
	private float _explodeDelay;
	
	override public void Init()
	{
		_soundComponent = gameObject.GetComponent<AudioSource>();
		if(_soundComponent == null)
			_soundComponent = gameObject.AddComponent<AudioSource>();
	}
	
	void Awake(){
		Init();
	}
	
	void Update()
	{
		_soundComponent.pitch = Time.timeScale;
		
		if (_delayedExplosionStarted)
		{
			_explodeDelay = (_explodeDelay - Time.deltaTime);
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}
	
	private int _idx;
	override public void Explode()
	{
		if (detailThreshold > detail) return;
	
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + (Random.value * (explodeDelayMax - explodeDelayMin));
		}		
		if (_explodeDelay <= 0) 
		{	
			_soundComponent.volume = 1.0f;
			_soundComponent.minDistance = minVolume;
			_soundComponent.maxDistance = maxVolume;
			_soundComponent.rolloffMode = rolloffFactor;
			
			if (Vector3.Distance(Camera.main.transform.position, this.transform.position) < distanceThreshold)
			{
				_idx = (int)(Random.value * nearSounds.Length);
				_soundComponent.PlayOneShot(nearSounds[_idx]);
			}
			else
			{
				_idx = (int)(Random.value * farSounds.Length);
				_soundComponent.PlayOneShot(farSounds[_idx]);
			}	
			_delayedExplosionStarted = false;
			_explodeDelay = 0f;			
		}
		else
		{
			_delayedExplosionStarted = true;
		}
	}
	
	public void Reset()
	{
	}
}