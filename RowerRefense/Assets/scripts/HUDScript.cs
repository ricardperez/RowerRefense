using UnityEngine;
using System.Collections;

public class HUDScript : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			
		}
	}
	
	void OnGUI ()
	{
		if (!PauseScript.sharedInstance().isPaused())
		{
			this.infoGUI ();
		}
	}
	
	void infoGUI ()
	{
		PlayerScript player = PlayerScript.sharedInstance ();
		
		string [] labelsText = {
			"Life: " + player.getLife (),
			"Money: " + player.getMoney (),
			"",
			"Points: " + player.getScore (),
			"Round: " + SpawnEnemiesScript.sharedInstance ().getCurrentRound (),
			"Killed enemies: " + player.getNKilledEnemies (),
			"Defenses: " + player.getNDefenses (),
			"",
			"Next round in: " + (SpawnEnemiesScript.sharedInstance ().isWaiting () ? string.Format ("{0:N2}", SpawnEnemiesScript.sharedInstance ().timeForNextRound ()) : "-"),
		};
		float labelsWidth = 150.0f;
		float labelsHeight = 20.0f;
		float labelsSeparation = 5.0f;
		float startY = ((Screen.height - (labelsText.Length * labelsHeight) - (labelsText.Length - 1) * labelsSeparation) / 2.0f);
		float startX = 20.0f;
		float nextY = startY;
		
		for (int i=0; i<labelsText.Length; i++) {
			GUI.Label (new Rect (startX, nextY, labelsWidth, labelsHeight), labelsText [i]);
			nextY += (labelsHeight + labelsSeparation);
		}
		
		
		float buttonsWidth = 100.0f;
		float buttonsHeight = 30.0f;
		float buttonsSeparation = labelsSeparation;
		if (SpawnEnemiesScript.sharedInstance ().isWaiting ()) {
			if (GUI.Button (new Rect (startX, nextY, buttonsWidth, buttonsHeight), "Spawn")) {
				SpawnEnemiesScript.sharedInstance ().forceSpawn ();
			}
		}
		nextY += (buttonsHeight + buttonsSeparation);
		
		bool isPerspectiveNow = MapScript.sharedInstance().isUsing3dRendering();
		string cameraStr = (isPerspectiveNow ? "2D" : "3D");
		if (GUI.Button (new Rect (Screen.width - buttonsWidth - 20.0f, 20.0f, buttonsWidth, buttonsHeight), cameraStr)) {
			if (isPerspectiveNow)
			{
				MapScript.sharedInstance().set2dRendering();
			} else
			{
				MapScript.sharedInstance().set3dRendering();
			}
		}
		
		bool timeScaleIsNormal = (Time.timeScale < 1.01);
		string timeScaleStr = (timeScaleIsNormal ? "2x" : "1x");
		if (GUI.Button (new Rect (Screen.width - 1.5f*buttonsWidth - 30.0f, 20.0f, buttonsWidth*0.5f, buttonsHeight), timeScaleStr)) {
			if (timeScaleIsNormal)
			{
				Time.timeScale = 2.0f;
			} else
			{
				Time.timeScale = 1.0f;
			}
		}
	}
}
