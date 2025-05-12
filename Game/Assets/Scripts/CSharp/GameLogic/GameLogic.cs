using UnityEngine;
using System;
using System.Collections.Generic;
using State;

public abstract class GameLogic : MonoBehaviour {
	
	public GameObject defenderTemplate;
	protected GameFacade facade;
	protected DefenderController player;
	float pointsChangedTime;
	int lastPoints = 0;
	
	public GameFacade gameFacade {
		set {
			facade = value;
		}
	}
	
	public virtual void InstantiatePlayer() {
		player = facade.InstantiateObject(defenderTemplate).GetComponent<DefenderController>();
		facade.CallBuffered(player, "SetPlayerName", Environment.UserName);
	}
	
	public abstract bool IsRadarEnabled();
	public abstract void InstantiateAI();
	
	public abstract void ConfigureEnemyTypes(Dictionary<Type, HashSet<Type>> enemyTypes);
	public abstract void StartGame();
	public virtual void OnGameStarted() {
		facade.gameStarted = true;
	}
	public abstract void OnLocalGameFinished();
	public abstract void OnNetworkGameFinished(bool won, string reason, string summaryJSON);
	
	protected virtual void OnGUI() {
		if (player == null)
			return;
		
		DrawPoints();
	}
	
	void DrawPoints() {
		if (lastPoints != player.points) {
			pointsChangedTime = Time.time;
			lastPoints = player.points;
		}
		
		GUI.color = Time.time - pointsChangedTime < 1
			? Color.green
			: Color.white;
		
		GUI.Label(new Rect(Screen.width - 70, 10, 90, 20), string.Format("{0} points", player.points));
	}
}
