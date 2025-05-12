using UnityEngine;
using System.Collections;


public class SinglePlayerGameEnd : MonoBehaviour {
	
	HighScoreView highScore;
	EnterNewHighScore newHighScore;
	DefenderController parent;
	public string msg;
	public int points;
	public bool won;
	GUIStyle style = new GUIStyle();
	
	public SinglePlayerGameEnd(){
	}
	
	void Start() {
		
		//this.parent = parent;
		gameObject.AddComponent<HighScoreView>();
		highScore = gameObject.GetComponent<HighScoreView>();
		highScore.percentageWidth(.5f);
		highScore.percentageHeight(.6f);	
		highScore.inGame = true;
		
		gameObject.AddComponent<EnterNewHighScore>();
		newHighScore = gameObject.GetComponent<EnterNewHighScore>();		
		newHighScore.percentageWidth(.3f);
		newHighScore.percentageHeight(.2f);
		newHighScore.points = points;
		
		if(GlobalConfig.getInstance().highScore.isNewHighScore(points)){
			newHighScore.ToggleVisibility();
		} else {
			highScore.ToggleVisibility();
		}
		
	}
	
	// Update is called once per frame
	public void Update () {
		
	}
	
	void OnGUI(){
			
			style.normal.textColor = won ? Color.green : Color.red;
			GUILayout.BeginArea(new Rect(0,0, Screen.width, Screen.height));
				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
						style.fontSize = 48;
						GUILayout.Label(won ? "Winner" : "Looser", style);
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
		
				GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
						style.fontSize = 24;
						GUILayout.Label(msg,style);
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			GUILayout.EndArea();				
	}
}
