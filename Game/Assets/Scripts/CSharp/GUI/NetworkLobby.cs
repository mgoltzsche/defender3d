using UnityEngine;
using System.Collections;

public class NetworkLobby : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnEnable(){
		gameObject.AddComponent<Lobby>();
		Lobby lobby = GetComponent<Lobby>();
		lobby.percentageWidth(.5f);
		lobby.percentageHeight(.8f);		
		lobby.ToggleVisibility();		
	}
}
