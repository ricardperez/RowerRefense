using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{
	
	public float life = 100.0f;
	public float damage = 10.0f;
	public float velocity = 1.0f;
	public int score = 10;
	public Transform explosionPrefab;
	public AudioClip explosionAudio;
	private float _initialLife;
	private GameObject _lifeBar;
	private Vector3 _nextPosition;
	private Vector3 _movingVect;
	private int _pathPointIndex;
	
	// Use this for initialization
	void Start ()
	{
		
		ArrayList pathPoints = PathScript.sharedInstance ().getPathCheckpoints ();
		this.transform.position = (Vector3)pathPoints [0];
		this._pathPointIndex = 0;
		this.getNextPosition ();
		
		foreach (Transform child in this.transform) {
			if (child.tag == "LifeBar") {
				this._lifeBar = child.gameObject;
				break;
			}
		}
		
		this._initialLife = this.life;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 movingVect = (this._nextPosition - this.transform.position);
		if (movingVect.sqrMagnitude > 0.001f) {
			Vector3 pos = this.transform.position;
			pos += (this._movingVect * this.velocity * Time.deltaTime);
			this.transform.position = pos;
		} else {
			if (!this.getNextPosition ()) {
				this.explode (false);
			}
		}
	}
	
	private void explode (bool killed)
	{
		if (this.explosionPrefab != null) {
			Instantiate (this.explosionPrefab, this.transform.position, Quaternion.identity);
			if (this.explosionAudio != null) {
				AudioSource.PlayClipAtPoint(this.explosionAudio, this.transform.position);
			}
		}
		SpawnEnemiesScript.sharedInstance ().enemyExplode (this.gameObject, killed);
		Destroy (this.gameObject);
	}
	
	private bool getNextPosition ()
	{
		ArrayList pathPoints = PathScript.sharedInstance ().getPathCheckpoints ();
		this._pathPointIndex++;
		if (this._pathPointIndex >= pathPoints.Count) {
			return false;
		} else {
			this._nextPosition = (Vector3)pathPoints [this._pathPointIndex];
			this._nextPosition.y = this.transform.position.y;
			this._movingVect = (this._nextPosition - this.transform.position).normalized;
			
			this.transform.rotation = Quaternion.LookRotation (this._movingVect);
			
			return true;
		}
	}
	
	public void takeDamage (float damagePoints)
	{
		this.life -= damagePoints;
		if (this.life <= 0.0) {
			this.explode (true);
		} else {
			float initialWidth = 3.0f;
			
			Vector3 lifeScale = this._lifeBar.transform.localScale;
			lifeScale.x = initialWidth * (this.life / this._initialLife);
			this._lifeBar.transform.localScale = lifeScale;
			
			Vector3 lifeBarPos = this._lifeBar.transform.localPosition;
			lifeBarPos.x = ((lifeScale.x - initialWidth) / 2.0f);
			this._lifeBar.transform.localPosition = lifeBarPos;
		}
		
	}
	
	void OnParticleCollision (GameObject other)
	{
		Debug.Log ("Particle collision in enemy");
	}
}
