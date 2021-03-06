using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
	public int startMoney = 100;
	public int startLife = 100;
	private int _money;
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
		this._life = this.startLife;
		this._money = this.startMoney;
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
	
	
	
	void defenseWasAdded(DefenseScript defense)
	{
		this._nDefenses++;
		this._money -= defense.price;
	}
	
	void defenseWasRemoved(DefenseScript defense)
	{
		this._nDefenses--;
		this._money += defense.price;
	}
	
	
	public void enemyWasDestroyed (GameObject enemy)
	{
		this._nKilledEnemies++;
		
		EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
		this._score += enemyScript.score;
		
		this._money += enemyScript.score;
	}
	
	public void enemyReachedHome(GameObject enemy)
	{
		EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
		this._life -= enemyScript.damage;
		
		if (this._life <= 0)
		{
			StaticData.score = this._score;
			Application.LoadLevel("EndGame");
		}
	}
	
	
	public int getMoney()
	{
		return this._money;
	}
	
	public void spendMoney(int ammount)
	{
		this._money -= ammount;
	}
}
