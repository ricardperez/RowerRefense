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
	
	}
	
	void OnGUI ()
	{
		PlayerScript player = PlayerScript.sharedInstance ();
		
		string [] labelsText = {
			"Life: " + player.getLife (),
			"Points: " + player.getScore (),
			"Killed enemies: " + player.getNKilledEnemies (),
			"Defenses: " + player.getNDefenses ()
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
	}
}
