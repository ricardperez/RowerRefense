using UnityEngine;
using System.Collections;

public class DefenseScript : MonoBehaviour
{
	
	public float power = 5.0f;
	public float hitsPerSecond = 1.0f;
	public float hitRadius = 4.0f;
	private GameObject _enemy;
	private ArrayList _targetEnemies;
	private double _timeSinceLastShoot;
	
	// Use this for initialization
	void Start ()
	{
		SphereCollider sphereCollider = (SphereCollider)this.collider;
		Vector2 tileSize = MapScript.sharedInstance ().getTileSize ();
		sphereCollider.radius = (2.5f * hitRadius * (tileSize.x > tileSize.y ? tileSize.x : tileSize.y));
		
		this._targetEnemies = new ArrayList ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (this._enemy != null) {
			this.transform.rotation = Quaternion.LookRotation (this._enemy.transform.position - this.transform.position);
			
			this._timeSinceLastShoot += Time.deltaTime;
			if (this._timeSinceLastShoot >= (1.0 / this.hitsPerSecond)) {
				this._enemy.GetComponent<EnemyScript> ().takeDamage (this.power);
				this._timeSinceLastShoot = 0.0;
			}
		}
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Enemy") {
			if (this._targetEnemies.Count == 0) {
				this._enemy = other.gameObject;
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
			}
		}
	}
}
