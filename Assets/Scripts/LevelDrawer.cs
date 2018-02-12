using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDrawer : MonoBehaviour
{
	public static LevelDrawer Instance;
	
	public int GridSizeX;
	public int GridSizeY;
	public Tile FinishTile;
	
	[SerializeField] private Sprite[] _spriteTextures;
	[SerializeField] private Tile _tile;
	

	private float _tileSize = 1;
	private Dictionary<string, Sprite> _sprites;

	public Tile[,] Tiles;


	private void Awake ()
	{
		Instance = this;

		Tiles = new Tile[GridSizeX, GridSizeY];
		_sprites = new Dictionary<string, Sprite>();
		
		foreach (var spriteTexture in _spriteTextures)
		{
			_sprites.Add(spriteTexture.name, spriteTexture);
		}
		
		CreateMap();
		CenterCamera();
	}

	public void RecreateRandomMap()
	{
		foreach (var tile in Tiles)
		{
			Destroy(tile.gameObject);
		}
		
		CreateMap();
	}

	private void CreateMap()
	{
		for (int i = 0; i < GridSizeY; i++)
		{
			for (int j = 0; j < GridSizeX; j++)
			{
				var tile = Instantiate(_tile);
				tile.transform.position = new Vector2(_tileSize * j + _tileSize / 2, _tileSize * i + _tileSize / 2);
				tile.Type = TileType.Snow;
				tile.X = j;
				tile.Y = i;
				Tiles[j, i] = tile;

				if (Random.Range(0, 100) < 20)
				{
					tile.GetComponent<SpriteRenderer>().sprite = _sprites["rock"];
					tile.Type = TileType.Rock;
					continue;
				}
//
//				if (Random.Range(0, 100) < 20)
//				{
//					tile.GetComponent<SpriteRenderer>().sprite = _sprites["water2"];
//					tile.Type = TileType.Water;
//					continue;
//				}
				
			}
		}

		var randomX = Random.Range(0, GridSizeX);
		var randomY = Random.Range(0, GridSizeY);
		var finish = Tiles[randomX, randomY];
		finish.Type = TileType.Finish;
		finish.GetComponent<SpriteRenderer>().sprite = _sprites["iglo"];
		FinishTile = finish;
	}

	private void CenterCamera()
	{
		var mid = new Vector2(GridSizeX * _tileSize / 2f, GridSizeY * _tileSize / 2f);
		
		Camera.main.transform.position = new Vector3(mid.x, mid.y, Camera.main.transform.position.z);
	}
}
