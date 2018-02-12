using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBot : MonoBehaviour {

	private Tile _currentTile;

	private void Start()
	{
		_currentTile = LevelDrawer.Instance.Tiles[LevelDrawer.Instance.GridSizeX - 1, LevelDrawer.Instance.GridSizeY - 1];
		StartCoroutine(Move(RandomMovement));
	}

	public void Move(Tile tile)
	{
		
		var newPosition = tile.transform.position;
		newPosition.z -= 2;

		transform.position = newPosition;
		_currentTile = tile;
	}

	private void RandomMovement()
	{
		
		var random = Random.Range(0, 4);
		Tile newTile = null;
		if (random == 0)
			newTile = _currentTile.Up;
		if(random == 1)
			newTile = _currentTile.Right;
		if(random == 2)
			newTile = _currentTile.Down;
		if(random == 3)
			newTile = _currentTile.Left;

		if (newTile == null || newTile.Type == TileType.Rock)
		{
			RandomMovement();
			return;
		}

		Move(newTile);
	}

	private IEnumerator Move(Action moveMethod)
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);

			
			
			moveMethod();
		}
	}
}
