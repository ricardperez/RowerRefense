using UnityEngine;
using System.Collections;

public class EndGameScript : MonoBehaviour {
	
	private GUIStyle _labelStyle;
	
	void Start()
	{
		this._labelStyle = new GUIStyle();
		this._labelStyle.normal.textColor = Color.white;
		this._labelStyle.fontSize = 30;
		this._labelStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	void OnGUI()
	{
		float nextY = (Screen.height/2.0f) - 150.0f;
		GUI.Label(new Rect((Screen.width-100)/2.0f, nextY, 100, 25), "END GAME", this._labelStyle);
		nextY += 80.0f;
		
		if (GUI.Button(new Rect((Screen.width-100)/2.0f, nextY, 100, 35), "Play again"))
		{
			Application.LoadLevel("Game");
		}
		nextY += 55.0f;
		
		if (GUI.Button(new Rect((Screen.width-100)/2.0f, nextY, 100, 35), "Exit"))
		{
			Application.LoadLevel("MainMenu");
		}
		nextY += 55.0f;
	}
}
