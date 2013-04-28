using UnityEngine;
using System.Collections;

public class PathScript : MonoBehaviour
{
	private ArrayList _lineObjects;
	private ArrayList _checkpoints;
	private ArrayList _checkpointsPositions;
	private static PathScript singleton;
	private ArrayList _blockedPositions;

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
		if (this.shortestCurrentPath (out this._checkpointsPositions)) {
			this.buildPathWithPositions (this._checkpointsPositions);
		}
	}
	
	private bool shortestCurrentPath (out ArrayList checkpointsPositions)
	{
		Position doorPos = MapScript.sharedInstance ().getDoorPosition ();
		Position homePos = MapScript.sharedInstance ().getHomePosition ();
		
		return this.existsPathFromPosToPos (doorPos, homePos, out checkpointsPositions);
	}
	
	private void buildPathWithPositions (ArrayList positions)
	{
		if (this._lineObjects != null) {
			foreach (GameObject lineObject in this._lineObjects) {
				Destroy (lineObject);
			}
		}
		
		this._checkpoints = new ArrayList (positions.Count);
		foreach (Position pos in positions) {
			this._checkpoints.Add (MapScript.sharedInstance ().getPointForMapCoordinates (pos));
		}
		
		this._lineObjects = new ArrayList (this._checkpoints.Count - 1);
		float width = 0.15f;
		Vector3 offsetX = new Vector3 (width * 0.5f, 0.0f, 0.0f);
		Vector3 offsetY = new Vector3 (0.0f, 0.0f, width * 0.5f);
		Color color = new Color(1.0f, 1.0f, 1.0f, 0.35f);
		for (int i=0; i<this._checkpoints.Count-1; i++) {
			GameObject go = new GameObject ("path-line");
			LineRenderer lr = go.AddComponent ("LineRenderer") as LineRenderer;
			lr.material = new Material (Shader.Find ("Particles/Additive"));
			lr.SetColors (color, color);
			lr.SetWidth (width, width);
			lr.SetVertexCount (2);
			
			Vector3 origin = (Vector3)this._checkpoints [i];
			Vector3 dest = (Vector3)this._checkpoints [i + 1];
			
			if (Mathf.Abs (dest.x - origin.x) > Mathf.Abs (dest.z - origin.z)) {
				if (dest.x > origin.x) {
					origin -= offsetX;
					dest += offsetX;
				} else {
					origin += offsetX;
					dest -= offsetX;
				}
				
			} else {
				if (dest.z > origin.z) {
					origin -= offsetY;
					dest += offsetY;
				} else {
					origin += offsetY;
					dest -= offsetY;
				}
				
			}
			
			lr.SetPosition (0, origin);
			lr.SetPosition (1, dest);
			
			this._lineObjects.Add (go);
		}
	}
	
	/**
	 * Finds the shortest path using a version of the Dijkstra algorithm.
	 * Horizontal directions will be taken before vertical.
	 */
	private bool existsPathFromPosToPos (Position start, Position end, out ArrayList positions)
	{
		ArrayList allPossiblePaths = new ArrayList ();
		
		ArrayList internPositions = new ArrayList ();
		ArrayList visitedPositions = new ArrayList ();
		int shortestPathLength = int.MaxValue;
		__nCalls = 0;
		bool exists = this.rExistsPathFromPosToPos (start, end, ref internPositions, ref visitedPositions, ref allPossiblePaths, ref shortestPathLength);
//		Debug.Log ("N calls: " + __nCalls);
		
		positions = null;
		if (exists) {
//			Debug.Log ("Found " + allPossiblePaths.Count + " paths");
			int minSteps = int.MaxValue;
			ArrayList shortestPath = null;
			foreach (ArrayList path in allPossiblePaths) {
				if (path.Count < minSteps) {
					minSteps = path.Count;
					shortestPath = path;
				}
				if (shortestPath != null) {
					positions = (ArrayList)shortestPath.Clone ();
				} else {
					positions = new ArrayList ();
				}
			}
		}
		
		return exists;
	}
	
	private int __nCalls = 0;

	private bool rExistsPathFromPosToPos (Position start, Position end, ref ArrayList positions, ref ArrayList visited, ref ArrayList allPossiblePaths, ref int shortestPathLength)
	{
		__nCalls++;
		if (positions.Count >= shortestPathLength) {
			return false;
		}
		
		bool isValid = false;
		
		// CASES TO IGNORE
		int mapRows = MapScript.sharedInstance ().getNRows ();
		int mapColumns = MapScript.sharedInstance ().getNColumns ();
		bool ignore = (!start.isInRange (mapRows, mapColumns));
		if (!ignore) {
			ignore = (this.positionIsBlocked (start));
			if (!ignore) {
				int i = 0;
				while (!ignore && i<visited.Count) {
					Position visitedPos = (Position)visited [i];
					ignore = (visitedPos.isEqualTo (start));
					i++;
				}
			}
		}
		
		int v = visited.Count;
		visited.Add (start);
		
		
		if (ignore) {
			isValid = false;
		} else {
			// POSITION MAY BE VALID
			
			int p = positions.Count;
			positions.Add (start);
			
			bool isFinished = (start.isEqualTo (end));
			
			if (isFinished) {
				isValid = true;
				if (positions.Count < shortestPathLength) {
					shortestPathLength = positions.Count;
					allPossiblePaths.Add (positions.Clone ());
				}
				
			} else {
				
				Position next;
		
				ArrayList directions = new ArrayList (4);
				directions.Add (Direction.kRight);
				directions.Add (Direction.kUp);
				directions.Add (Direction.kLeft);
				directions.Add (Direction.kDown);
				
				int i = 0;
				while (i < directions.Count) {
					Direction dir = (Direction)directions [i];
					next = start.move (dir);
					isValid |= this.rExistsPathFromPosToPos (next, end, ref positions, ref visited, ref allPossiblePaths, ref shortestPathLength);
					i++;
				}
				
			}
			
			positions.RemoveAt (p);
			
		}
		visited.RemoveAt (v);
		return isValid;
		
	}
	
	private bool positionIsBlocked (Position pos)
	{
		bool blockedBySelf = false;
		bool anyDefense = false;
		if (this._blockedPositions != null) {
		
			int i = 0;
			while (!blockedBySelf && i<this._blockedPositions.Count) {
				Position next = (Position)this._blockedPositions [i];
				blockedBySelf = pos.isEqualTo (next);
				i++;
			}
		}
		
		if (!blockedBySelf) {
			DefenseScript def;
			anyDefense = CreateDefensesScript.sharedInstance ().anyDefenseAtPosition (pos, out def);
		}
		
		return (blockedBySelf || anyDefense);
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
	public bool blockPosition (Position pos)
	{
		bool isInPath = false;
		int i = 0;
		while (!isInPath && i<(this._checkpoints.Count-1)) {
			Position origin = (Position)this._checkpointsPositions [i];
			Position dest = (Position)this._checkpointsPositions [i + 1];
			if (origin.row == dest.row && origin.row == pos.row) {
				isInPath = (((pos.column >= origin.column) && (pos.column <= dest.column)) || ((pos.column >= dest.column) && (pos.column <= origin.column)));
			} else if (origin.column == dest.column && origin.column == pos.column) {
				isInPath = (((pos.row >= origin.row) && (pos.row <= dest.row)) || ((pos.row >= dest.row) && (pos.row <= origin.row)));
			}
			i++;
		}
		
		if (isInPath) {
			this._blockedPositions = new ArrayList (1);
			this._blockedPositions.Add (pos);
			ArrayList positions;
			if (this.shortestCurrentPath (out positions)) {
				this._checkpointsPositions = positions;
				this.buildPathWithPositions (positions);
				this._blockedPositions = null;
				return true;
			} else {
				this._blockedPositions = null;
				return false;
			}
		} else {
			return true;
		}
	}
}
