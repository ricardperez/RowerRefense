using UnityEngine;
using System.Collections;


public enum Direction
{
	kRight = 0,
	kLeft,
	kUp,
	kDown,
	kQuiet,
}

public struct Position
{
	public int row;
	public int column;
	
	public Position (int r, int c)
	{
		this.row = r;
		this.column = c;
	}
	
	override public string ToString ()
	{
		return ("(" + this.row + ", " + this.column + ")");
	}
	
	public bool isEqualTo (Position pos)
	{
		return ((this.row == pos.row) && (this.column == pos.column));
	}
	
	public Position move (Direction dir)
	{
		return this.move (dir, 1);
	}
	
	public Position move (Direction dir, int steps)
	{
		int row = this.row;
		int column = this.column;
		switch (dir) {
		case Direction.kRight:
			column += steps;
			break;
		case Direction.kLeft:
			column -= steps;
			break;
		case Direction.kUp:
			row += steps;
			break;
		case Direction.kDown:
			row -= steps;
			break;
		default:
			break;
		}
		return new Position (row, column);
	}
	
	public Direction directionTo (Position pos)
	{
		if (pos.column > this.column) {
			return Direction.kRight;
		} else if (pos.column < this.column) {
			return Direction.kLeft;
		} else if (pos.row > this.row) {
			return Direction.kUp;
		} else if (pos.row < this.row) {
			return Direction.kDown;
		} else {
			return Direction.kQuiet;
		}
	}
	
	public static Direction reversedDirection (Direction dir)
	{
		Direction reversed = Direction.kQuiet;
		switch (dir) {
		case Direction.kLeft:
			reversed = Direction.kRight;
			break;
		case Direction.kRight:
			reversed = Direction.kLeft;
			break;
		case Direction.kUp:
			reversed = Direction.kDown;
			break;
		case Direction.kDown:
			reversed = Direction.kUp;
			break;
		}
		return reversed;
	}
	
	public bool isInRange (int maxRow, int maxCol)
	{
		return this.isInRange (0, 0, maxRow, maxCol);
	}
	
	public bool isInRange (int minRow, int minCol, int maxRow, int maxCol)
	{
		return ((this.row >= minRow) && (this.column >= minCol) && (this.row <= maxRow) && (this.column <= maxCol));
	}
}

public struct Size
{
	public int width;
	public int height;
	
	public Size (int w, int h)
	{
		this.width = w;
		this.height = h;
	}
	
	override public string ToString ()
	{
		return ("(" + this.width + ", " + this.height + ")");
	}
	
	public bool isEqualTo (Size s)
	{
		return ((this.width == s.width) && (this.height == s.height));
	}
}

public struct Segment
{
	public Position pos1;
	public Position pos2;
	
	public Segment(Position p1, Position p2)
	{
		this.pos1 = p1;
		this.pos2 = p2;
	}
	
	public bool isEqualTo(Segment s)
	{
		return ((this.pos1.isEqualTo(s.pos1) && this.pos2.isEqualTo(s.pos2)) || (this.pos1.isEqualTo(s.pos2) && this.pos2.isEqualTo(s.pos1)));
	}
}


public class Structs : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
