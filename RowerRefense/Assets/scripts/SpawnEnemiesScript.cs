using UnityEngine;
using System.Collections;

public class SpawnEnemiesScript : MonoBehaviour
{
	public GameObject enemyPrefab;
	public int nEnemiesPerRound = 5;
	public double secondsPerEnemy = 1.5;
	public double timeBetweenRounds = 10.0;
	private int _nEnemiesInCurrentRound;
	private double _timeSinceLastEnemy;
	private double _timeToStartRound;
	private Vector3 _startPosition;
	private bool _startPositionGot;
	private int _nEnemies;
	private int _nRounds;
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
		this._timeToStartRound = this.timeBetweenRounds;
		GameObject map = MapScript.sharedInstance ().map;
		this._startPosition.y = (map.transform.position.y + map.transform.localScale.y * 0.5f);
		this._nEnemiesInCurrentRound = 0;
		this._nRounds = 0;
		this._nEnemies = 0;
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
			if (this.isWaiting ()) {
				this._timeToStartRound -= Time.deltaTime;
				if (!this.isWaiting ()) {
					this._nRounds++;
				}
			} else {
				bool addEnemy = false;
				if (this._nEnemiesInCurrentRound == 0) {
					addEnemy = true;
				} else {
					this._timeSinceLastEnemy += Time.deltaTime;
					if (this._timeSinceLastEnemy >= this.secondsPerEnemy) {
						addEnemy = true;
					}
				}
				if (addEnemy) {
					this.addEnemy ();
					if (this._nEnemiesInCurrentRound == this.nEnemiesPerRound) {
						this._nEnemiesInCurrentRound = 0;
						this._timeToStartRound = this.timeBetweenRounds;
					}
				}
			}
		}
	}
	
	private void addEnemy ()
	{
		this._timeSinceLastEnemy = 0.0;
		GameObject enemy = (GameObject)Instantiate (this.enemyPrefab, this._startPosition, Quaternion.identity);
		EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
		enemyScript.life *= (1 + this._nRounds / 5.0f);
		enemyScript.score += this._nRounds;
		enemyScript.velocity *= (1 + this._nRounds / 5.0f);
		
		this._nEnemiesInCurrentRound++;
		this._nEnemies++;
	}
	
	public int getNEnemies ()
	{
		return this._nEnemies;
	}
	
	public void enemyExplode (GameObject obj, bool killed)
	{
		if (killed) {
			BroadcastMessage ("enemyWasDestroyed", obj);
		} else {
			BroadcastMessage ("enemyReachedHome", obj);
		}
		this._nEnemies--;
	}
	
	public bool isWaiting ()
	{
		return (this._timeToStartRound > 0.01);
	}
	
	public double timeForNextRound ()
	{
		return this._timeToStartRound;
	}
	
	public int getCurrentRound ()
	{
		return this._nRounds;
	}
	
	public void forceSpawn()
	{
		this._timeToStartRound = 0.0;
	}
	
}
