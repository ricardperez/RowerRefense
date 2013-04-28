using UnityEngine;
using System.Collections;

//public struct Defense
//{
//	public GameObject gameObject;
//	public Position position;
//	
//	public Defense (GameObject go, Position pos)
//	{
//		this.gameObject = go;
//		this.position = pos;
//	}
//}

public class CreateDefensesScript : MonoBehaviour
{
	public GameObject[] defensesPrefabs;
	private ArrayList _allDefenses;
	private GameObject _defenseInstanceToAdd;
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
			if (this._defenseInstanceToAdd != null) {
				
				MapScript mapScript = MapScript.sharedInstance ();
				if (mapScript.selectionIsVisible ()) {
					
					DefenseScript def = this._defenseInstanceToAdd.GetComponent<DefenseScript> ();
					if (PlayerScript.sharedInstance ().getMoney () >= def.price) {
						Position selectionPos = mapScript.getSelectionPosition ();
						if (mapScript.tryToBlockPosition (selectionPos)) {
							Vector3 position = mapScript.getPointForMapCoordinates (selectionPos);
							this._defenseInstanceToAdd.transform.position = position;
							DefenseScript defenseScript = this._defenseInstanceToAdd.GetComponent<DefenseScript> ();
							defenseScript.setPosition (selectionPos);
							defenseScript.setUsable (true);
							this._allDefenses.Add (defenseScript);
							BroadcastMessage ("defenseWasAdded", defenseScript);
							
							this._defenseInstanceToAdd = (GameObject)Instantiate (this._defenseInstanceToAdd, MapScript.sharedInstance ().hiddenPosition (), Quaternion.identity);
						}
					}
				}
			}
		}
		
		if (this._defenseInstanceToAdd != null) {
			MapScript mapScript = MapScript.sharedInstance ();
			if (mapScript.selectionIsVisible ()) {
				Position selectionPos = mapScript.getSelectionPosition ();
				Vector3 position = mapScript.getPointForMapCoordinates (selectionPos);
				this._defenseInstanceToAdd.transform.position = position;
			} else {
				this._defenseInstanceToAdd.transform.position = MapScript.sharedInstance ().hiddenPosition ();
			}
		}
	}
	
	public ArrayList getAllDefenses ()
	{
		return _allDefenses;
	}
	
	public bool anyDefenseAtPosition (Position pos, out DefenseScript defense)
	{
		defense = null;
		if (this._allDefenses == null) {
			return false;
		}
			
		bool found = false;
		int i = 0;
		while (!found && i<this._allDefenses.Count) {
			DefenseScript next = (DefenseScript)this._allDefenses [i];
			found = next.getPosition ().isEqualTo (pos);
			if (found) {
				defense = next;
			}
			i++;
		}
		return found;
	}
	
	void enemyWasDestroyed (GameObject obj)
	{
		foreach (DefenseScript defense in this._allDefenses) {
			defense.enemyWasDestroyed (obj);
		}
		if (this._defenseInstanceToAdd != null) {
			DefenseScript defense = this._defenseInstanceToAdd.GetComponent<DefenseScript> ();
			defense.enemyWasDestroyed (obj);
		}
	}
	
	void OnGUI ()
	{
		if (!PauseScript.sharedInstance ().isPaused ()) {
			int nButtons = this.defensesPrefabs.Length;
			float buttonsWidth = 120.0f;
			float startX = Screen.width - buttonsWidth - 20.0f;
			float buttonsHeight = 35.0f;
			float buttonsSep = 10.0f;
			float totalHeight = (nButtons * buttonsHeight + (nButtons - 1) * buttonsSep);
			float startY = (Screen.height - totalHeight) / 2.0f;
		
			float nextY = startY;
			for (int i=0; i<nButtons; i++) {
				GameObject defensePrefab = this.defensesPrefabs [i];
				if (GUI.Button (new Rect (startX, nextY, buttonsWidth, buttonsHeight), defensePrefab.name)) {
					if (this._defenseInstanceToAdd != null) {
						Destroy (this._defenseInstanceToAdd);
						this._defenseInstanceToAdd = null;
					}
					this._defenseInstanceToAdd = (GameObject)Instantiate (defensePrefab, MapScript.sharedInstance ().hiddenPosition (), Quaternion.identity);
				
				}
				nextY += (buttonsHeight + buttonsSep);
			}
		}
	}
}
