using UnityEngine;
using System.Collections;

public class DebugScript : MonoBehaviour
{
	private int nFrames;
	private System.DateTime lastDate;
	private double fps;
	private GUIStyle labelsStyle;

	// Use this for initialization
	void Start()
	{
		this.nFrames = 0;
		this.fps = 0;
		this.lastDate = System.DateTime.Now;
		
		this.labelsStyle = new GUIStyle();
		labelsStyle.normal.textColor = Color.white;
	}
	
	// Update is called once per frame
	void Update()
	{
		this.nFrames++;
		System.DateTime now = System.DateTime.Now;
		double nMillis = now.Subtract(this.lastDate).TotalMilliseconds;
		if (nMillis >= 1000)
		{
			this.fps = ((nFrames / nMillis) * 1000);
			this.nFrames = 0;
			this.lastDate = now;
		}
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 300, 50), "FPS: " + string.Format("{0:N2}", this.fps), this.labelsStyle);
	}
}
