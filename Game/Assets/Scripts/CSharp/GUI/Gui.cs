using UnityEngine;
using System.Collections;

public class Gui : MonoBehaviour {
	
	public MainMenu mainMenu;
	public NetworkWindow networkWindow;
	
	
	// Use this for initialization
	void Start () {
		
		gameObject.AddComponent(typeof(MainMenu));
		mainMenu = (MainMenu) GetComponent(typeof(MainMenu));
		mainMenu.percentageWidth(.5f);
		mainMenu.percentageHeight(.8f);		
		mainMenu.ToggleVisibility();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI(){
				
	}
}
