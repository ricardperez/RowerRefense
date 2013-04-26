using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
	private float _life;
	private int _score;
	private int _nKilledEnemies;
	private int _nDefenses;
	private static PlayerScript singleton;

	public static PlayerScript sharedInstance ()
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
		this._life = 100.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public float getLife()
	{
		return this._life;
	}
	
	public int getScore()
	{
		return this._score;
	}
	
	public int getNKilledEnemies()
	{
		return this._nKilledEnemies;
	}
	
	public int getNDefenses()
	{
		return this._nDefenses;
	}
	
	
	
	void defenseWasAdded(Defense defense)
	{
		this._nDefenses++;
	}
	
	
	public void enemyWasDestroyed (GameObject enemy)
	{
		this._nKilledEnemies++;
		
		EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
		this._score += enemyScript.score;
	}
	
	public void enemyReachedHome(GameObject enemy)
	{
		EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
		this._life -= enemyScript.damage;
	}
}
