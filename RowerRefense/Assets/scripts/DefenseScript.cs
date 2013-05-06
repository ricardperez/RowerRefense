using UnityEngine;
using System.Collections;

public class DefenseScript : MonoBehaviour
{
	public float power = 5.0f;
	public float hitsPerSecond = 1.0f;
	public float hitRadius = 4.0f;
	public int price = 20;
	public Transform shootExplosionPrefab;
	public AudioClip shootAudioClip;
	private GameObject _enemy;
	private ArrayList _targetEnemies;
	private double _timeSinceLastShoot;
	private Position _position;
	private ParticleSystem _shootBulletsPS;
	private bool _usable;
	private bool _showingRadius;
	
	// Use this for initialization
	void Start ()
	{
		_usable = false;
		SphereCollider sphereCollider = (SphereCollider)this.collider;
		sphereCollider.radius = (this.getRealHitRadius () * 2.5f);
		
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
		
		this._showingRadius = true;
	}
	
	private float getRealHitRadius ()
	{
		Vector2 tileSize = MapScript.sharedInstance ().getTileSize ();
		return (hitRadius * (tileSize.x > tileSize.y ? tileSize.x : tileSize.y));
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
			} else if (this.shootAudioClip != null)
			{
				AudioSource.PlayClipAtPoint(this.shootAudioClip, this.transform.position);
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
		if (this.name == "foo") {
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
		if (!this._position.isEqualTo (p)) {
			bool positionIsValid = MapScript.sharedInstance ().positionsIsValid (p);
			
			this._position = p;
			if (positionIsValid) {
				this.gameObject.transform.position = MapScript.sharedInstance ().getPointForMapCoordinates (p);
				if (this._showingRadius) {
					this.showRadius (true);
				}
			} else {
				this.gameObject.transform.position = MapScript.sharedInstance ().hiddenPosition ();
				this.showRadius (false, true);
			}
		}
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
	
	private void showRadius (bool show, bool forced)
	{
		if (!forced) {
			this._showingRadius = show;
		}
		
		if (show) {
			float theta_scale = 0.1f;
			int size = Mathf.CeilToInt((2.0f * Mathf.PI) / theta_scale) + 1;
			
			float r = this.getRealHitRadius ();
			Color color = new Color(0.0f, 0.85f, 0.1f, 0.5f);
			LineRenderer lineRenderer = this.GetComponent<LineRenderer> ();
			if (lineRenderer == null) {
				lineRenderer = this.gameObject.AddComponent ("LineRenderer") as LineRenderer;
			}
			lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
			lineRenderer.SetColors (color, color);
			lineRenderer.SetWidth (0.1F, 0.1F);
			lineRenderer.SetVertexCount (size);
				
			float offsetX = this.gameObject.transform.position.x;
			float offsetZ = this.gameObject.transform.position.z;
				
			float theta = 0.0f;
			float yPos = MapScript.sharedInstance ().floorY ();
			for (int i=0; i<size; i++) {
				
				float x = (r * Mathf.Cos (theta) + offsetX);
				float z = (r * Mathf.Sin (theta) + offsetZ);

				Vector3 pos = new Vector3 (x, yPos, z);
				lineRenderer.SetPosition (i, pos);
				theta += theta_scale;
			}
				
		} else {
			LineRenderer lineRenderer = this.GetComponent<LineRenderer> ();
			if (lineRenderer != null) {
				lineRenderer.SetVertexCount (0);
			}
		}
	}
		
	public void showRadius (bool show)
	{
		this.showRadius (show, false);
	}
	
	public bool isShowingRadius ()
	{
		return this._showingRadius;
	}
}
