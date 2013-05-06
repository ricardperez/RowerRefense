using UnityEngine;
using System.Collections;


public class LineRendererObject : Object
{
	private GameObject go;
	private LineRenderer lr;
	private Segment s;

	public LineRendererObject (Segment segment) : base()
	{
		this.s = segment;
		this.go = new GameObject ();
		this.lr = this.go.AddComponent ("LineRenderer") as LineRenderer;
		
		float width = 0.15f;
		Vector3 offsetX = new Vector3 (width * 0.5f, 0.0f, 0.0f);
		Vector3 offsetY = new Vector3 (0.0f, 0.0f, width * 0.5f);
		Color color = new Color (1.0f, 1.0f, 1.0f, 0.35f);
		lr.material = new Material (Shader.Find ("Particles/Additive"));
		lr.SetColors (color, color);
		lr.SetWidth (width, width);
		
		this.lr.SetVertexCount (2);
		Vector3 origin = MapScript.sharedInstance ().getPointForMapCoordinates (this.s.pos1);
		Vector3 dest = MapScript.sharedInstance ().getPointForMapCoordinates (this.s.pos2);
		
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
		
		this.lr.SetPosition (0, origin);
		this.lr.SetPosition (1, dest);
	}
	
	public void destroy()
	{
		Destroy (this.go);
	}
}

public class Pair<T, U>
{
	public Pair ()
	{
	}

	public Pair (T first, U second)
	{
		this.first = first;
		this.second = second;
	}

	public T first { get; set; }

	public U second { get; set; }
};

public class Classes : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}


public class StaticData : Object
{
	public static int score;
}
