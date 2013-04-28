using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {
	
	private bool _wasPaused;
	private double timeSincePaused;
	private bool _paused;
	private GUIStyle _labelStyle;
	
	private static PauseScript singleton;
	void Awake()
	{
		PauseScript.singleton = this;
	}
	
	void OnDestroy()
	{
		PauseScript.singleton = null;
	}
	
	public static PauseScript sharedInstance()
	{
		return PauseScript.singleton;
	}
	
	void Start()
	{
		this._labelStyle = new GUIStyle();
		this._labelStyle.normal.textColor = Color.white;
		this._labelStyle.fontSize = 40;
		this._labelStyle.alignment = TextAnchor.MiddleCenter;
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (this._paused)
			{
				this.resume();
			} else
			{
				this.pause();
			}
		}
		
		if (this._wasPaused && !this._paused)
		{
			this.timeSincePaused += Time.deltaTime;
			if (this.timeSincePaused > 0.25)
			{
				Debug.Log("changing paused");
				this._wasPaused = false;
			}
		}
	}
	
	void OnGUI()
	{
		if (this._paused)
		{
			this.displayPauseMenu();
		}
	}
	
	void displayPauseMenu()
	{
		float buttonsHeight = 50.0f;
		float buttonsSep = 10.0f;
		int nButtons = 3;
		
		
		GUI.Label(new Rect((Screen.width-100)/2.0f, 20.0f, 100, 50), "Pause", this._labelStyle);
		
		float currY = (Screen.height - nButtons * buttonsHeight - (nButtons - 1) * buttonsSep) / 2;
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Resume"))
		{
			this.resume();
		}
		currY += (buttonsSep + buttonsHeight);
		
		if (GUI.Button(new Rect((Screen.width - 150) / 2, currY, 150, buttonsHeight), "Exit"))
		{
			this.resume();
			Application.LoadLevel("MainMenu");
		}
	}
	
	
	public void pause()
	{
		this._paused = true;
		AudioListener.pause = true;
		Time.timeScale = 0.000001f;
		
		this.timeSincePaused = 0.0;
		this._wasPaused = true;
	}
	
	public void resume()
	{
		this._paused = false;
		AudioListener.pause = false;
		Time.timeScale = 1.0f;
		this.timeSincePaused = 0.0;
	}
	
	public bool isPaused()
	{
		return (this._paused || this._wasPaused);
	}
}
