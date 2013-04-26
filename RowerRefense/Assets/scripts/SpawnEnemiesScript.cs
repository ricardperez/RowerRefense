using UnityEngine;
using System.Collections;

public class SpawnEnemiesScript : MonoBehaviour
{
	public GameObject enemyPrefab;
	public double secondsPerEnemy = 3.0;
	private double _timeSinceLastEnemy;
	private Vector3 _startPosition;
	private bool _startPositionGot;
	private int _nEnemies;
	private static SpawnEnemiesScript singleton;

	public static SpawnEnemiesScript sharedInstance ()
	{
		return singleton;
	}
	
	void Awake ()
	{
		singleton = this;
	}
	
	void OnDestroy ()
	{
		singleton = null;
	}
	
	// Use this for initialization
	void Start ()
	{
		this._timeSinceLastEnemy = (this.secondsPerEnemy - 1.0);		
		GameObject map = MapScript.sharedInstance ().map;
		this._startPosition.y = (map.transform.position.y + map.transform.localScale.y * 0.5f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!this._startPositionGot) {
			ArrayList checkpoints = PathScript.sharedInstance ().getPathCheckpoints ();
			if (checkpoints != null) {
				this._startPosition = (Vector3)checkpoints [0];
				this._startPositionGot = true;
			}
		} else {
			this._timeSinceLastEnemy += Time.deltaTime;
			if (this._timeSinceLastEnemy >= this.secondsPerEnemy) {
				this._timeSinceLastEnemy = 0.0;
				Instantiate (this.enemyPrefab, this._startPosition, Quaternion.identity);
			}
		}
	}
	
	public int getNEnemies ()
	{
		return this._nEnemies;
	}
	
	public void enemyExplode(GameObject obj, bool killed)
	{
		if (killed)
		{
			BroadcastMessage("enemyWasDestroyed", obj);
		} else
		{
			BroadcastMessage("enemyReachedHome", obj);
		}
		this._nEnemies--;
	}
}
