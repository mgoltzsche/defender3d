using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ServiceStack.Text;
using Utils;

public class GlobalConfig {
	
	public string serverIP = "";
	public string userName = "";
	public float deathMatchDuration;
	
	private static GlobalConfig instance;
	
	public HighScore highScore;
	
	private GlobalConfig(){
		string highScoreString = PlayerPrefs.GetString("HighScore");
		if(highScoreString.Length == 0 && highScoreString.Equals(""))
			highScore = new HighScore(HighScore.CreateDefaultHighScore());
		else
			highScore = new HighScore(highScoreString);		
		
		
		
	}
	
	public static GlobalConfig getInstance(){
		if(instance == null)
			instance = new GlobalConfig();
		return instance;
	}
	
	public void PersistHighScore(){
		Debug.Log("Persist" + highScore.ToJSON()); TODO:
		PlayerPrefs.SetString("HighScore", highScore.ToJSON());		
	}
}
