using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathScript : MonoBehaviour
{
//	private ArrayList _lineObjects;
//	private ArrayList _checkpoints;
	private ArrayList _checkpointsPositions;
	private static PathScript singleton;
	private ArrayList _blockedPositions;
	private Dictionary<Segment, Pair<int, LineRendererObject>> _pathSegments;

	public static PathScript sharedInstance ()
	{
		return singleton;
	}
	
	void Awake ()
	{
		singleton = this;
		this._pathSegments = new Dictionary<Segment, Pair<int, LineRendererObject>> ();
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
		ArrayList nextPath;
		if (this.shortestCurrentPath (out nextPath)) {
			if (this._checkpointsPositions != null) {
				this.removeSegmentsFromPositions (this._checkpointsPositions);
			}
			
			this._checkpointsPositions = nextPath;
			this.addSegmentsFromPositions (this._checkpointsPositions);
		}
	}
	
	private bool shortestCurrentPath (out ArrayList checkpointsPositions)
	{
		Position doorPos = MapScript.sharedInstance ().getDoorPosition ();
		Position homePos = MapScript.sharedInstance ().getHomePosition ();
		
		return this.existsPathFromPosToPos (doorPos, homePos, out checkpointsPositions);
	}
	
	/**
	 * Finds the shortest path using a version of the Dijkstra algorithm.
	 * Horizontal directions will be taken before vertical.
	 */
	public bool existsPathFromPosToPos (Position start, Position end, out ArrayList positions)
	{
		bool exists = this.iExistsPathFromPosToPos (start, end, out positions);
		return exists;
	}
	
	public bool existsPathFromPosToPosIfBlockingPosition (Position start, Position end, Position blockPos, out ArrayList positions)
	{
		if (this._blockedPositions == null)
		{
			this._blockedPositions = new ArrayList(1);
		}
		int n = this._blockedPositions.Count;
		this._blockedPositions.Add(blockPos);
		bool exists = this.iExistsPathFromPosToPos (start, end, out positions);
		this._blockedPositions.RemoveAt(n);
		return exists;
	}
	
	private bool iExistsPathFromPosToPos (Position source, Position target, out ArrayList path)
	{
		path = new ArrayList ();
		
		Queue queue = new Queue ();
		HashSet<Position> visited = new HashSet<Position> ();
		Dictionary<Position, Position> parents = new Dictionary<Position, Position> ();
		
		bool pathFound = false;
		queue.Enqueue (source);
		while ((queue.Count > 0) && !pathFound) {
			Position current = (Position)queue.Dequeue ();
			pathFound = (current.isEqualTo (target));
			if (!pathFound) {
				visited.Add (current);
				Position [] neighbors = {
				current.move (Direction.kRight),
				current.move (Direction.kUp),
				current.move (Direction.kLeft),
				current.move (Direction.kDown),
			};
				foreach (Position p in neighbors) {
					__nCalls++;
					if (MapScript.sharedInstance ().positionsIsValid (p)) {
						if (!visited.Contains (p)) {
							if (!this.positionIsBlocked (p)) {
								queue.Enqueue (p);
								parents.Remove (p);
								parents.Add (p, current);
							}
						}
					}
				}
			}
			
		}
		
		if (pathFound) {
			Stack pathStack = new Stack ();
			Position current = target;
			while (current.row >= 0) {
				pathStack.Push (current);
				current = (parents.ContainsKey (current) ? parents [current] : new Position (-1, -1));
			}
			
			while (pathStack.Count > 0) {
				path.Add (pathStack.Pop ());
			}
			
		}
		
		return pathFound;
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
	
//	public ArrayList getPathCheckpoints ()
//	{
//		return this._checkpoints;
//	}
	
	public ArrayList getPathCheckpointsPositions ()
	{
		return this._checkpointsPositions;
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
		while (!isInPath && i<(this._checkpointsPositions.Count-1)) {
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
			ArrayList nextPath;
			if (this.shortestCurrentPath (out nextPath)) {
				
				if (this._checkpointsPositions != null) {
					this.removeSegmentsFromPositions (this._checkpointsPositions);
				}
				this._checkpointsPositions = nextPath;
				this.addSegmentsFromPositions (this._checkpointsPositions);
//				this.buildPathWithPositions (positions);
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
	
	public void addSegment (Segment s)
	{
		if (this._pathSegments.ContainsKey (s)) {
			Pair<int, LineRendererObject> pair = this._pathSegments [s];
			pair.first += 1;
		} else {
			LineRendererObject lro = new LineRendererObject (s);
			Pair<int, LineRendererObject> pair = new Pair<int, LineRendererObject> (1, lro);
			this._pathSegments.Add (s, pair);
		}
	}
	
	public void addSegmentsFromPositions (ArrayList positions)
	{
		if (positions.Count >= 2) {
			Position origin = (Position)positions [0];
			Position dest;
			for (int i=1; i<positions.Count; i++) {
				dest = (Position)positions [i];
				Segment s = new Segment (origin, dest);
				this.addSegment (s);
				origin = dest;
			}
		}
		
	}
	
	public void removeSegment (Segment s)
	{
		if (this._pathSegments.ContainsKey (s)) {
			Pair<int, LineRendererObject> pair = this._pathSegments [s];
			if (pair.first > 1) {
				pair.first -= 1;
			} else {
				pair.second.destroy();
				this._pathSegments.Remove (s);
			}
		}
	}
	
	public void removeSegmentsFromPositions (ArrayList positions)
	{
		if (positions.Count >= 2) {
			Position origin = (Position)positions [0];
			Position dest;
			for (int i=1; i<positions.Count; i++) {
				dest = (Position)positions [i];
				Segment s = new Segment (origin, dest);
				this.removeSegment (s);
				origin = dest;
			}
		}
		
	}
}
