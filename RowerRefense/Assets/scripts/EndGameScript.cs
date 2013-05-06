using UnityEngine;
using System.Collections;

public class EndGameScript : MonoBehaviour {
	
	private GUIStyle _titleLabelStyle;
	private GUIStyle _subtitleLabelStyle;
	
	void Start()
	{
		this._titleLabelStyle = new GUIStyle();
		this._titleLabelStyle.normal.textColor = Color.white;
		this._titleLabelStyle.fontSize = 30;
		this._titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		this._subtitleLabelStyle = new GUIStyle();
		this._subtitleLabelStyle.normal.textColor = Color.white;
		this._subtitleLabelStyle.fontSize = 18;
		this._subtitleLabelStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	void OnGUI()
	{
		float nextY = (Screen.height/2.0f) - 150.0f;
		GUI.Label(new Rect((Screen.width-100)/2.0f, nextY, 100, 25), "END GAME", this._titleLabelStyle);
		nextY += 50.0f;
		
		GUI.Label(new Rect((Screen.width-100)/2.0f, nextY, 100, 25), "You scored " + StaticData.score + " points", this._subtitleLabelStyle);
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
