using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
	Snow,
	Water,
	Rock,
	Finish
}

public class Tile : MonoBehaviour
{

	public TileType Type;
	public int X;
	public int Y;
	public bool Active;

	public Tile Left
	{
		get { return getTile(X - 1, Y); }
	}
	
	public Tile Up
	{
		get { return getTile(X, Y + 1); }
	}
	
	public Tile Down
	{
		get { return getTile(X, Y - 1); }
	}
	
	public Tile Right
	{
		get { return getTile(X + 1, Y); }
	}

	public float[] GetTileMapping()
	{
		var mapping = new float[4];

		mapping[0] = Up != null && Up.Type != TileType.Rock ? 1 : 0;
		mapping[1] = Right != null && Right.Type != TileType.Rock ? 1 : 0;
		mapping[2] = Down != null && Down.Type != TileType.Rock ? 1 : 0;
		mapping[3] = Left != null && Left.Type != TileType.Rock ? 1 : 0;

		return mapping;
	}

	public List<Tile> GetNeighbours()
	{
		var neighbours = new List<Tile>();
		
		if(Left != null)
			neighbours.Add(Left);
		if(Up != null)
			neighbours.Add(Up);
		if(Right != null)
			neighbours.Add(Right);
		if(Down != null)
			neighbours.Add(Down);

		return neighbours;
	}
	

	private Tile getTile(int x, int y)
	{

		if (x < LevelDrawer.Instance.GridSizeX && y < LevelDrawer.Instance.GridSizeY && x >= 0 && y >= 0)
		{
			return LevelDrawer.Instance.Tiles[x, y];
		}

		return null;
	}

}
