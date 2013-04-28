using UnityEngine;
using System.Collections;

public class DefenseScript : MonoBehaviour
{
	public float power = 5.0f;
	public float hitsPerSecond = 1.0f;
	public float hitRadius = 4.0f;
	public int price = 20;
	public Transform shootExplosionPrefab;
	public GameObject _enemy;
	public ArrayList _targetEnemies;
	private double _timeSinceLastShoot;
	private Position _position;
	private ParticleSystem _shootBulletsPS;
	public bool _usable;
	
	// Use this for initialization
	void Start ()
	{
		_usable = false;
		SphereCollider sphereCollider = (SphereCollider)this.collider;
		Vector2 tileSize = MapScript.sharedInstance ().getTileSize ();
		sphereCollider.radius = (2.5f * hitRadius * (tileSize.x > tileSize.y ? tileSize.x : tileSize.y));
		
		this._targetEnemies = new ArrayList ();
		
		foreach (object c in this.transform) {
			System.Type t = c.GetType ();
			if ((t == typeof(GameObject)) || (t == typeof(Transform))) {
				GameObject child;
				if (t == typeof(Transform)) {
					child = ((Transform)c).gameObject;
				} else {
					child = (GameObject)c;
				}
				if (child.tag == "ShootBullets") {
					this._shootBulletsPS = child.particleSystem;
					this._shootBulletsPS.Pause ();
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (this._usable) {
			if (this._enemy != null) {
				this.transform.rotation = Quaternion.LookRotation (this._enemy.transform.position - this.transform.position);
			
				this._timeSinceLastShoot += Time.deltaTime;
				if (this._timeSinceLastShoot >= (1.0 / this.hitsPerSecond)) {
					this.shoot ();
					this._timeSinceLastShoot = 0.0;
				}
			}
		}
	}
	
	void shoot ()
	{
		if (this._usable) {
			this._enemy.GetComponent<EnemyScript> ().takeDamage (this.power);
			if (this.shootExplosionPrefab != null) {
				Vector3 pos = (this.transform.position + (this.transform.forward * this.transform.localScale.x) * 0.5f);
				Instantiate (this.shootExplosionPrefab, pos, Quaternion.LookRotation (this.transform.forward));
			}
			
			if (this.audio != null) {
				this.audio.Play ();
			}
		}
	}
	
	void startShooting ()
	{
		if (this._usable) {
			if (this._shootBulletsPS != null) {
				this._shootBulletsPS.Play ();
			}
		}
	}
	
	void stopShooting ()
	{
		if (this._usable) {
			if (this._shootBulletsPS != null) {
				this._shootBulletsPS.Stop ();
			}
		}
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (this.name == "foo")
		{
			Debug.Log ("Trigger enter " + other.tag);
		}
		if (other.tag == "Enemy") {
			if (this._targetEnemies.Count == 0) {
				this._enemy = other.gameObject;
				this.startShooting ();
			} else {
//				float distOther = Vector3.SqrMagnitude (other.transform.position - this.transform.position);
//				float distEnemy = Vector3.SqrMagnitude (this._enemy.transform.position - this.transform.position);
//				if (distOther < distEnemy) {
//					this._enemy = other.gameObject;
//				}
			}
			this._targetEnemies.Add (other.gameObject);
		}
	}
	
	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Enemy") {
			this.removeEnemy (other.gameObject);
		}
	}
	
	public void enemyWasDestroyed (GameObject enemy)
	{
		this.removeEnemy (enemy);
	}
	
	void removeEnemy (GameObject enemy)
	{
		this._targetEnemies.Remove (enemy);
		if (this._enemy == enemy) {
			if (this._targetEnemies.Count > 0) {
				this._enemy = (GameObject)this._targetEnemies [0];
				
				
			} else {
				this._enemy = null;
				this.stopShooting ();
			}
		}
	}
	
	public Position getPosition ()
	{
		return this._position;
	}
	
	public void setPosition (Position p)
	{
		this._position = p;
	}
	
	public void setUsable (bool usable)
	{
		if (usable != _usable) {
			this._usable = usable;
			if (usable && this._enemy != null) {
				this.startShooting ();
			}
		}
		
	}
}
