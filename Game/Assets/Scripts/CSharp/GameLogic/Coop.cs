using UnityEngine;
using System;
using System.Collections.Generic;

public class Coop : GameLogic {
	
	public GameObject colonistTemplate;
	public int colonistCount = 15;
	public GameObject landerTemplate;
	public int landerCount = 3;
	public GameObject saucererTemplate;
	public int saucererCount = 3;
	int lastLifesLeft;
	float lifesChangedTime;
	
	public Texture2D playerDefender, friendlyDefender;
	
	string finishReason;
	bool won;
	
	public override bool IsRadarEnabled() {
		return true;
	}
	
	public override void InstantiatePlayer() {
		base.InstantiatePlayer();
		lastLifesLeft = player.lifes;
	}
	
	public override void InstantiateAI() {
		for (int i = 0; i < colonistCount; i++)
			facade.InstantiateObject(colonistTemplate);
		for (int i = 0; i < landerCount; i++)
			facade.InstantiateObject(landerTemplate);
		for (int i = 0; i < saucererCount; i++)
			facade.InstantiateObject(saucererTemplate);
	}
	
	// called on server
	public override void StartGame() {
		InvokeRepeating("CheckIfGameFinished", 10, 5);
	}
	
	// called on clients
	public override void OnGameStarted() {
		base.OnGameStarted();
	}
	
	public override void ConfigureEnemyTypes(Dictionary<Type, HashSet<Type>> enemyTypes) {
		HashSet<Type> defenderEnemies = new HashSet<Type>();
		defenderEnemies.Add(typeof(LanderController));
		defenderEnemies.Add(typeof(SaucererController));
		defenderEnemies.Add(typeof(MutantController));
		HashSet<Type> defenderType = new HashSet<Type>(new Type[] {typeof(DefenderController)});
		
		enemyTypes.Add(typeof(DefenderController), defenderEnemies);
		enemyTypes.Add(typeof(LanderController), defenderType);
		enemyTypes.Add(typeof(SaucererController), defenderType);
		enemyTypes.Add(typeof(MutantController), defenderType);
	}
	
	void CheckIfGameFinished() {
		if (!facade.ExistsActiveDefender())
			// no defender left
			FinishGame(false, "You are dead!");
		else if (!facade.colonists.GetEnumerator().MoveNext())
			// all colonists mutated
			FinishGame(false, "All colonists have been mutated. The whole planet is populated with mutants now. You are doomed!");
		else if (!facade.ExistsActiveEnemyAI())
			FinishGame(true, "You defeated all aliens and rescued the colonists!");
	}
	
	void FinishGame(bool won, string reason) {
		this.won = won;
		finishReason = reason;
		
		Debug.Log((won ? "You win: " : "You loose: ") + reason);
		
		facade.FinishGame(won, reason);		
		CancelInvoke();
	}
	
	public override void OnLocalGameFinished() {
		SinglePlayerGameEnd end = gameObject.GetComponent<SinglePlayerGameEnd>();
		end.msg = finishReason;
		end.points = player.points;
		end.won = won;
		end.enabled = true;
	}
	
	public override void OnNetworkGameFinished(bool won, string reason, string summaryJSON) {
		Debug.Log ("Coop game finished: " + summaryJSON);
		ScoreBoardView end = gameObject.GetComponent<ScoreBoardView>();
		end.scores = new Utils.HighScore(summaryJSON);
		end.won = won;
		end.msg = reason;
		end.style = new GUIStyle();
		end.style.normal.textColor = won ? Color.green : Color.red;
		end.headLine = won ? "You win" : "You loose";
		end.enabled = true;
	}
	
	void DrawLife() {
		GUI.color = Color.white;
		int rightHUDX = Screen.width - 200;
		GUI.Box(new Rect(rightHUDX, 0, 200, 100), "");
		GUI.Label(new Rect(rightHUDX, 0, 200, 20), string.Format("{0} lifes remaining", player.lifes));
	}
	
	protected override void OnGUI() {
		if (player == null)
			return;
		
		base.OnGUI();
		DrawLifesLeft();
	}
	
	void DrawLifesLeft() {
		if (lastLifesLeft != player.lifes) {
			lifesChangedTime = Time.time;
			lastLifesLeft = player.lifes;
		}
		
		GUI.color = Time.time - lifesChangedTime < 1
			? Color.green
			: Color.white;
		
		GUI.Label(new Rect(Screen.width - 70, 30, 90, 20), string.Format("{0} lifes", player.lifes));
	}
}