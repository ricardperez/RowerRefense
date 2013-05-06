using UnityEngine;
using System.Collections;

public class MapScript : MonoBehaviour
{
	public GameObject map;
	public Camera camera2d;
	public Camera camera3d;
	public GameObject selection;
	public GameObject door;
	public GameObject home;
	public int mapWidth = 11;
	public int mapHeight = 11;
	public Material selectionMaterialGood;
	public Material selectionMaterialBad;
	private Vector2 _mapOrigin;
	private Vector2 _tileSize;
	private Plane _floorPlane;
	private bool _selectionVisible;
	private Position _selectedPosition;
	private Position _doorPosition;
	private Position _homePosition;
	private static MapScript singleton;
	private bool _isRendering2d;
	private bool _selectionIsAvailable;

	public static MapScript sharedInstance ()
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
		this._floorPlane = new Plane (this.map.transform.up, (this.map.transform.position + Vector3.up * this.map.transform.localScale.y * 0.5f));
		this._tileSize = new Vector2 ((this.map.transform.localScale.x / this.mapWidth), (this.map.transform.localScale.z / this.mapHeight));
		this._mapOrigin = this.map.transform.position;
		this._mapOrigin.x -= (this._tileSize.x * this.mapWidth * 0.5f - this._tileSize.x * 0.5f);
		this._mapOrigin.y -= (this._tileSize.y * this.mapHeight * 0.5f - this._tileSize.y * 0.5f);
		
		Vector3 selScale = this.selection.transform.localScale;
		selScale.x = this._tileSize.x;
		selScale.z = this._tileSize.y;
		this.selection.transform.localScale = selScale;
		
		Vector3 doorScale = this.door.transform.localScale;
		doorScale.x = this._tileSize.x;
		doorScale.z = this._tileSize.y;
		this.door.transform.localScale = doorScale;
		this._doorPosition = new Position (0, 0);
		
		Vector3 homeScale = this.home.transform.localScale;
		homeScale.x = this._tileSize.x;
		homeScale.z = this._tileSize.y;
		this.home.transform.localScale = homeScale;
		this._homePosition = new Position (this.mapWidth / 2, this.mapHeight / 2);
		
		this.setPositionForTile (this.door, this._doorPosition);
		this.setPositionForTile (this.home, this._homePosition);
		
		this.map.renderer.material.mainTextureScale = new Vector2 (this.mapWidth, this.mapHeight);
		
		
		this._selectedPosition = new Position (-1, -1);
		_selectionVisible = true;
		this.setSelectionPosition (new Position (0, 0));
		this.setSelectionVisible (false);
		
		PathScript.sharedInstance ().recalculatePath ();
		
		_isRendering2d = false;
		this.set2dRendering ();
		
		this._selectionIsAvailable = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!PauseScript.sharedInstance ().isPaused ()) {
			this.updateSelection ();
		}
	}
	
	void updateSelection ()
	{
		Vector3 screenPos = Input.mousePosition;
		Ray ray = this.currentCamera ().ScreenPointToRay (screenPos);
		float distance;
		if (this._floorPlane.Raycast (ray, out distance)) {
			Vector3 point = ray.GetPoint (distance);
			Position position;
			this.getMapCoordinatesForPoint (point, out position);
			if (!this._selectedPosition.isEqualTo (position)) {
				if (this.positionsIsValid (position)) {
					this.setSelectionPosition (position);
					this.setSelectionVisible (true);
					this.updateSelectionIsAvailable ();
					
				} else {
					this.setSelectionVisible (false);
				}
			}
			
		} else {
			this.setSelectionVisible (false);
		}
	}
	
	void updateSelectionIsAvailable ()
	{
		if (this._selectedPosition.isEqualTo (this._homePosition) || this._selectedPosition.isEqualTo (this._doorPosition)) {
			this.setSelectionIsAvailable (false);
		} else {
			DefenseScript d;
			if (CreateDefensesScript.sharedInstance ().anyDefenseAtPosition (this._selectedPosition, out d)) {
				this.setSelectionIsAvailable (false);
			} else {
				ArrayList pathPositions;
				if (PathScript.sharedInstance ().existsPathFromPosToPosIfBlockingPosition (this.getDoorPosition (), this.getHomePosition (), this._selectedPosition, out pathPositions)) {
					this.setSelectionIsAvailable (true);
				} else {
					this.setSelectionIsAvailable (false);
				}
			}
		}
	}
	
	
	/**
	 * See also selectionIsVisible()
	 */
	public void setSelectionVisible (bool show)
	{
		if (show != _selectionVisible) {
			_selectionVisible = show;
			Vector3 selPos = this.selection.transform.position;
			if (show) {
				selPos.y = (this.map.transform.position.y + this.map.transform.localScale.y * 0.5f);
			} else {
				selPos.y = 5.0f;
			}
			this.selection.transform.position = selPos;
			
			if (!show) {
				this.setSelectionPosition (new Position (-1, -1));
			}
		}
	}
	
	/**
	 * See also setSelectionVisible(bool)
	 */
	public bool selectionIsVisible ()
	{
		return _selectionVisible;
	}
	
	/**
	 * (0,0) is the lower left corner.
	 * In order to see the selection, it has to be set visible with the setSelectionVisible(true) invocation.
	 */
	public void setSelectionPosition (Position position)
	{
		if ((position.row != this._selectedPosition.row) || (position.column != this._selectedPosition.column)) {
			this._selectedPosition = position;
			this.setPositionForTile (this.selection, this._selectedPosition);
		}
	}
	
	public Position getSelectionPosition ()
	{
		return this._selectedPosition;
	}
	
	public void getMapCoordinatesForPoint (Vector3 point, out Position pos)
	{
		float diffX = point.x - this._mapOrigin.x + this._tileSize.x * 0.5f;
		float diffY = point.z - this._mapOrigin.y + this._tileSize.y * 0.5f;
		
		int column = Mathf.FloorToInt (diffX / this._tileSize.x);
		int row = Mathf.FloorToInt (diffY / this._tileSize.y);
		
		pos = new Position (row, column);
	}
	
	public Position getDoorPosition ()
	{
		return this._doorPosition;
	}
	
	public Position getHomePosition ()
	{
		return this._homePosition;
	}
	
	public int getNRows ()
	{
		return (this.mapHeight - 1);
	}
	
	public int getNColumns ()
	{
		return (this.mapWidth - 1);
	}
	
	public Vector3 getPointForMapCoordinates (Position position)
	{
		int row = position.row;
		int column = position.column;
		return new Vector3 (this._mapOrigin.x + column * this._tileSize.x, this.map.transform.position.y + this.map.transform.localScale.y * 0.5f + 0.01f, this._mapOrigin.y + row * this._tileSize.y);
	}
	
	private void setPositionForTile (GameObject tile, Position position)
	{
		Vector3 pos = new Vector3 (this._mapOrigin.x, tile.transform.position.y, this._mapOrigin.y);
		pos.x += (position.column * this._tileSize.x);
		pos.z += (position.row * this._tileSize.y);
		
		tile.transform.position = pos;
	}
	
	public bool tryToBlockPosition (Position pos)
	{
		DefenseScript defense;
		bool anyDefense = CreateDefensesScript.sharedInstance ().anyDefenseAtPosition (pos, out defense);
		if (anyDefense) {
			return false;
		} else {
			bool pathCanBlockPosition = PathScript.sharedInstance ().blockPosition (pos);
			return pathCanBlockPosition;
		}
	}
	
	public Vector2 getTileSize ()
	{
		return this._tileSize;
	}
	
	public Vector3 hiddenPosition ()
	{
		return new Vector3 (0.0f, 100.0f, 0.0f);
	}
	
	public Camera currentCamera ()
	{
		return (this._isRendering2d ? this.camera2d : this.camera3d);
	}
	
	public bool isUsing2dRendering ()
	{
		return this._isRendering2d;
	}
	
	public bool isUsing3dRendering ()
	{
		return (!this._isRendering2d);
	}
	
	public void set2dRendering ()
	{
		if (!this._isRendering2d) {
			this._isRendering2d = true;
			
			this.camera2d.enabled = true;
			AudioListener camera2dAudioListener = this.camera2d.gameObject.GetComponent<AudioListener> ();
			camera2dAudioListener.enabled = true;
				
			this.camera3d.enabled = false;
			AudioListener camera3dAudioListener = this.camera3d.gameObject.GetComponent<AudioListener> ();
			camera3dAudioListener.enabled = false;
		}
	}
	
	public void set3dRendering ()
	{
		if (this._isRendering2d) {
			this._isRendering2d = false;
			
			this.camera2d.enabled = false;
			AudioListener camera2dAudioListener = this.camera2d.gameObject.GetComponent<AudioListener> ();
			camera2dAudioListener.enabled = false;
				
			this.camera3d.enabled = true;
			AudioListener camera3dAudioListener = this.camera3d.gameObject.GetComponent<AudioListener> ();
			camera3dAudioListener.enabled = true;
		}
	}
	
	public bool positionsIsValid (Position pos)
	{
		return pos.isInRange (this.getNRows (), this.getNColumns ());
	}
	
	public float floorY ()
	{
		return (this.map.transform.position.y + this.map.transform.localScale.y * 0.5f + 0.01f);
	}
	
	private void setSelectionIsAvailable (bool available)
	{
		if (available != this._selectionIsAvailable) {
			this._selectionIsAvailable = available;
			this.selection.renderer.material = (available ? this.selectionMaterialGood : this.selectionMaterialBad);
		}
	}
}
