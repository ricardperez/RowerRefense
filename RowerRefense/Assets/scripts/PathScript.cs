using UnityEngine;
using System.Collections;

public class PathScript : MonoBehaviour
{
	private ArrayList _lineObjects;
	private ArrayList _checkpoints;
	private ArrayList _checkpointsPositions;
	private static PathScript singleton;

	public static PathScript sharedInstance ()
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
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	public void recalculatePath ()
	{
		if (this._lineObjects != null) {
			foreach (GameObject lineObject in this._lineObjects) {
				Destroy (lineObject);
			}
		}
		
		
		Position doorPos = MapScript.sharedInstance ().getDoorPosition ();
		Position homePos = MapScript.sharedInstance ().getHomePosition ();
		Position midPos = new Position (doorPos.row, homePos.column);
		
		this._checkpointsPositions = new ArrayList(3);
		this._checkpointsPositions.Add (doorPos);
		this._checkpointsPositions.Add (midPos);
		this._checkpointsPositions.Add (homePos);
		
		this._checkpoints = new ArrayList (this._checkpointsPositions.Count);
		foreach (Position pos in this._checkpointsPositions)
		{
			this._checkpoints.Add (MapScript.sharedInstance ().getPointForMapCoordinates (pos));
		}
		
		this._lineObjects = new ArrayList (this._checkpoints.Count - 1);
		for (int i=0; i<this._checkpoints.Count-1; i++) {
			GameObject go = new GameObject ();
			LineRenderer lr = go.AddComponent ("LineRenderer") as LineRenderer;
			lr.SetWidth (0.15f, 0.15f);
			lr.SetVertexCount (2);
			lr.SetPosition (0, (Vector3)this._checkpoints [i]);
			lr.SetPosition (1, (Vector3)this._checkpoints [i + 1]);
			
			this._lineObjects.Add (go);
		}
	}
	
	public ArrayList getPathCheckpoints ()
	{
		return this._checkpoints;
	}
	
	/**
	 * Will try to block that position and return true if possible and false if not.
	 * If the position is not in the current path, then it is blockable. Otherwise,
	 * it only is blockable if an alternative path can be found.
	 */
	public bool blockPosition(Position pos)
	{
		bool isInPath = false;
		int i = 0;
		while (!isInPath && i<(this._checkpoints.Count-1))
		{
			Position origin = (Position)this._checkpointsPositions[i];
			Position dest = (Position)this._checkpointsPositions[i+1];
			if (origin.row == dest.row && origin.row == pos.row)
			{
				isInPath = (((pos.column >= origin.column) && (pos.column <= dest.column)) || ((pos.column >= dest.column) && (pos.column <= origin.column)));
			} else if (origin.column == dest.column && origin.column == pos.column)
			{
				isInPath = (((pos.row >= origin.row) && (pos.row <= dest.row)) || ((pos.row >= dest.row) && (pos.row <= origin.row)));
			}
			i++;
		}
		
		return (!isInPath);
	}
}
