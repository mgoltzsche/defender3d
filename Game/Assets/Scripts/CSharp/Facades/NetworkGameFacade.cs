using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using ServiceStack.Text;
using Config;
using State;

public class NetworkGameFacade : GameFacade {
	
	public GlobalConfig globalCFG;
	public int gameDuration = 1;
	int playerCount = 0;
	
	protected override ILifecycleStrategy CreateLifecycleStrategy() {
		return new NetworkLifecycleStrategy();
	}
	
	protected override void InitializeGame() {
		globalCFG = GlobalConfig.getInstance();
		
		if (globalCFG.serverIP.Equals("")) {
			Network.InitializeServer(50, 25000, false);
		} else {
			Network.Connect(globalCFG.serverIP, 25000);
		}
	}
	
	public override void StartGame() {
		Call(this, "StartNetworkGame", (int) globalCFG.deathMatchDuration);
	}

	[RPC]
	public void StartNetworkGame(int duration) {
		globalCFG.deathMatchDuration = duration;
		GetComponent<Lobby>().enabled = false;
		base.StartGame();
	}
	
	void OnServerInitialized() {	
		gameLogic.InstantiateAI();
		gameLogic.InstantiatePlayer();
		
		CreateCharacterIndex();
		ActivateLobby();
	}
	
	void OnConnectedToServer() {		
		gameLogic.InstantiatePlayer();
		
		CreateCharacterIndex();
		ActivateLobby();
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
		Debug.Log(player.ToString());
		playerCount++;
		CreateCharacterIndex();
	}
	
	void OnNetworkInstantiate(NetworkMessageInfo info) {
		CreateCharacterIndex();
		
        if (networkView.isMine) {
            Debug.Log("New object instanted by me");
		} else {
			// How does this make sense?
            Debug.Log("New object instantiated by " + info.sender);
			DisableAll<MonoBehaviour>(gameObject);
			DisableAll<Camera>(gameObject);
			DisableAll<AudioListener>(gameObject);
    	}
	}
	
	void DisableAll<T>(GameObject go) where T : Behaviour {
		foreach (Behaviour c in go.GetComponentsInChildren<T>())
			c.enabled = false;
	}
	
	
	public override void AttachColonist(ColonistController colonist, TransportingCharacterController transporter, bool isDefender) {
		if (Network.isServer) {
			if (!isDefender || colonist.attachable) {
				if (colonist.attachedTo != null)
					DetachColonist(colonist);
				
				colonist.AttachTo(transporter, isDefender);
				Call(this, "CallOnAttach", transporter.networkView.viewID, colonist.networkView.viewID);
			}
		} else
			CallOnServer(this, "AttachColonistOnServer", colonist.networkView.viewID, transporter.networkView.viewID, isDefender);
	}
	
	[RPC]
	public void AttachColonistOnServer(NetworkViewID colonistId, NetworkViewID transporterId, bool isDefender) {
		ColonistController colonist = NetworkView.Find(colonistId).gameObject.GetComponent<ColonistController>();
		TransportingCharacterController transporter = NetworkView.Find(transporterId).gameObject.GetComponent<TransportingCharacterController>();
		
		AttachColonist(colonist, transporter, isDefender);
	}
	
	[RPC]
	public void CallOnAttach(NetworkViewID transporterId, NetworkViewID colonistId) {
		ColonistController colonist = NetworkView.Find(colonistId).gameObject.GetComponent<ColonistController>();
		TransportingCharacterController transporter = NetworkView.Find(transporterId).gameObject.GetComponent<TransportingCharacterController>();
		
		transporter.OnAttach(colonist);
	}
	
	public override void DetachColonist(ColonistController colonist) {
		if (Network.isServer)
			base.DetachColonist(colonist);
		else
			CallOnServer(this, "DetachColonistOnServer", colonist.networkView.viewID);
	}
	
	[RPC]
	public void DetachColonistOnServer(NetworkViewID colonistId) {
		DetachColonist(NetworkView.Find(colonistId).gameObject.GetComponent<ColonistController>());
	}
	
	public override void CallOnServer(MonoBehaviour behaviour, string methodName, params object[] parameters) {
		if (Network.isServer)
			base.CallOnServer(behaviour, methodName, parameters);
		else
			behaviour.networkView.RPC(methodName, RPCMode.Server, parameters);
	}
	
	public override void CallBuffered(MonoBehaviour behaviour, string methodName, params object[] parameters) {
		behaviour.networkView.RPC(methodName, RPCMode.AllBuffered, parameters);
	}
	
	void ActivateLobby() {
		GetComponent<NetworkLobby>().enabled = true;
	}
	
	public override void FinishGame(bool won, string reason) {
		LinkedList<object[]> summary = new LinkedList<object[]>();
		
		foreach (DefenderController player in _defenders.OrderByDescending(p => p.points))
			summary.AddLast(new object[] {player.name, player.points});
		
		Call(this, "FinishNetworkGame", won, reason, summary.ToJson());
	}
	
	[RPC]
	public void FinishNetworkGame(bool won, string reason, string summary) {
		SetAllObjectsInactive();
		gameLogic.OnNetworkGameFinished(won, reason, summary);
	}
	
	void OnPlayerDisconnected( NetworkPlayer player){
	Debug.Log("Player Disconnected");
	}
}
