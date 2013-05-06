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
	
	private ArrayList _pathPositions;
	private int _pathPositionIndex;
	private Vector3 _nextPosition;
	private Vector3 _movingVect;
	
	// Use this for initialization
	void Start ()
	{
		this._pathPositions = (ArrayList)PathScript.sharedInstance ().getPathCheckpointsPositions().Clone();
		PathScript.sharedInstance().addSegmentsFromPositions(this._pathPositions);
		
		Position firstPosition = (Position)this._pathPositions[0];
		this._pathPositionIndex = 0;
		this.transform.position = MapScript.sharedInstance().getPointForMapCoordinates(firstPosition);
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
		if (movingVect.sqrMagnitude > 0.005f) {
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
		this._pathPositionIndex++;
		if (this._pathPositionIndex >= this._pathPositions.Count) {
			return false;
		} else {
			Position nextPos = (Position)this._pathPositions [this._pathPositionIndex];
			this._nextPosition = MapScript.sharedInstance().getPointForMapCoordinates(nextPos);
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
	
	public void blockPathPosition(Position pos)
	{
		bool positionIsInPath = false;
		int i = this._pathPositionIndex;
		while (!positionIsInPath && i<this._pathPositions.Count)
		{
			Position p = (Position)this._pathPositions[i];
			positionIsInPath = (pos.isEqualTo(p));
			i++;
		}
		
		if (positionIsInPath)
		{
			Position startPos = (Position)this._pathPositions[this._pathPositionIndex-1];
			Position endPos = MapScript.sharedInstance().getHomePosition();
			ArrayList positions;
			if (PathScript.sharedInstance().existsPathFromPosToPos(startPos, endPos, out positions))
			{
				PathScript.sharedInstance().addSegmentsFromPositions(positions);
				PathScript.sharedInstance().removeSegmentsFromPositions(this._pathPositions);
				this._pathPositions = positions;
				this._pathPositionIndex = 0;
				this.getNextPosition();
			}
		}
	}
	
	
	void OnDestroy()
	{
		if ((this._pathPositions != null) && (PathScript.sharedInstance() != null))
		{
			PathScript.sharedInstance().removeSegmentsFromPositions(this._pathPositions);
		}
	}
}
