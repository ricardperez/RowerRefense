using UnityEngine;
using System.Collections;

public struct Defense
{
	public GameObject gameObject;
	public Position position;
	
	public Defense (GameObject go, Position pos)
	{
		this.gameObject = go;
		this.position = pos;
	}
}

public class CreateDefensesScript : MonoBehaviour
{
	public GameObject towerPrefab;
	private ArrayList _allDefenses;
	private static CreateDefensesScript singleton;

	public static CreateDefensesScript sharedInstance ()
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
		this._allDefenses = new ArrayList ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			MapScript mapScript = MapScript.sharedInstance ();
			if (mapScript.selectionIsVisible ()) {
				Position selectionPos = mapScript.getSelectionPosition ();
				if (mapScript.canAddEnemyAtPosition (selectionPos)) {
					Vector3 position = mapScript.getPointForMapCoordinates (selectionPos);
					GameObject gameObject = (GameObject)Instantiate (this.towerPrefab, position, Quaternion.identity);
					Defense defense = new Defense (gameObject, selectionPos);
					this._allDefenses.Add (defense);
					BroadcastMessage("defenseWasAdded", defense);
				}
			}
		}
	}
	
	public ArrayList getAllDefenses ()
	{
		return _allDefenses;
	}
	
	public bool anyDefenseAtPosition(Position pos, out Defense defense)
	{
		defense = new Defense(null, new Position(-1,-1));
		bool found = false;
		int i = 0;
		while (!found && i<this._allDefenses.Count)
		{
			Defense next = (Defense)this._allDefenses[i];
			found = ((next.position.row == pos.row) && (next.position.column == pos.column));
			if (found)
			{
				defense = next;
			}
			i++;
		}
		return found;
	}
	
	
	void enemyWasDestroyed(GameObject obj)
	{
		foreach (Defense defense in this._allDefenses)
		{
			defense.gameObject.GetComponent<DefenseScript>().enemyWasDestroyed(obj);
		}
	}
}
