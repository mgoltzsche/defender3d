using UnityEngine;
using System.Collections;

public abstract class AbstractGUIComponent : MonoBehaviour {

	protected Rect position;
	private static Vector2 screenCenter;
	protected bool render = false;
	
	void Start(){			
		screenCenter = new Rect(0,0,Screen.width, Screen.height).center;		
	}
	
	protected virtual void OnGUI(){
		if(Screen.lockCursor)
			Screen.lockCursor = false;
	}
	
	protected abstract void DrawComponent(int id);
	
	public virtual void ToggleVisibility(){
		this.render = !render;
		
		if(!render)	
			resetView();

	}	
	
	protected void CenterComponentOnScreen(){
		this.position.x = screenCenter.x - (position.width / 2);
		this.position.y = screenCenter.y - (position.height / 2);
	}
	
	public void percentageWidth(float percent){
		this.position.width = Screen.width * percent;		
	}
	
	
	public void percentageHeight(float percent){
		this.position.height = Screen.height * percent;		
	}
	
	protected abstract void resetView();	
}
