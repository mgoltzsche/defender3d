using UnityEngine;
using System;
using System.Collections.Generic;

public class Deathmatch : GameLogic {
	
	public int gameDuration = 1;
	float startTime;
	
	public override bool IsRadarEnabled() {
		return false;
	}
	
	public override void InstantiatePlayer() {
		base.InstantiatePlayer();
		player.lifes = int.MaxValue;
	}
	
	public override void InstantiateAI() {
	}
	
	public override void StartGame() {
		gameDuration = (int) GlobalConfig.getInstance().deathMatchDuration;
		
		Invoke("FinishGameInvoker", gameDuration * 60);
	}
	
	public override void OnGameStarted() {
		base.OnGameStarted();
		startTime = Time.realtimeSinceStartup;
		gameDuration = (int) GlobalConfig.getInstance().deathMatchDuration;
	}
	
	public override void ConfigureEnemyTypes(Dictionary<Type, HashSet<Type>> enemyTypes) {
		enemyTypes.Add(typeof(DefenderController), new HashSet<Type>(new Type[] {typeof(DefenderController)}));
	}
	
	void FinishGameInvoker() {
		facade.FinishGame(true, "");
	}
	
	protected override void OnGUI() {
		base.OnGUI();
		DrawRemainingTime();
	}
	
	void DrawRemainingTime() {
		GUI.color = Color.white;
		
		int timeLeft = (int) Mathf.Ceil(Time.realtimeSinceStartup - startTime);
		timeLeft = Mathf.Max(0, gameDuration * 60 - timeLeft);
		
		string remainingTimeLabel = string.Format("{0:00}:{1:00}", Mathf.Floor(timeLeft / 60f), timeLeft % 60);
		
		GUI.Label(new Rect(20, 10, 50, 20), remainingTimeLabel);
	}
	
	
	
	public override void OnLocalGameFinished() {
		// not required
	}
	
	public override void OnNetworkGameFinished(bool won, string reason, string summaryJSON) {
		ScoreBoardView end = gameObject.GetComponent<ScoreBoardView>();
		Debug.Log(summaryJSON);
		end.scores = new Utils.HighScore(summaryJSON);
		end.won = won;
		end.msg = end.scores.scores.First.Value[0]+" is the winner!";
		end.style = new GUIStyle();
		end.style.normal.textColor = Color.blue;
		end.headLine = "Game End!";
		end.enabled = true;
	}
}