using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using DefaultNamespace;
using MachineLearning.NeuralNetwork;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerBot : MonoBehaviour
{
    public Tile CurrentTile;
    private NeuralNetwork _neuralNetwork;
    private List<Tile> _path;
    private Data _data;
    private int _amountOfData = 5000;

    private void Start()
    {
        _data = new Data();
//		_neuralNetwork = new NeuralNetwork(new[] {6, 5, 4});
        CurrentTile = LevelDrawer.Instance.Tiles[Random.Range(0, 10), Random.Range(0, 10)];
//		DisplayWeigths();
//		var xor = new Xor();
        _path = new AStar().FindPath(CurrentTile, LevelDrawer.Instance.FinishTile);
//        StartCoroutine(Move(Astar));
        _data = GetData();
        var dataSize = 30000;
        _neuralNetwork = new NeuralNetwork(new[] {101, 40, 4});
        _neuralNetwork.Learn(_data.Input.Take(dataSize).Select(x => x.Select(y => (float) y).ToArray()).ToArray(), _data.Output.Take(dataSize).Select(x => x.Select(y => (float) y).ToArray()).ToArray(), 1.3f);
        var accu = _neuralNetwork.Test(_data.Input.Skip(dataSize).Select(x => x.Select(y => (float) y).ToArray()).ToArray(),
            _data.Output.Skip(dataSize).Select(x => x.Select(y => (float) y).ToArray()).ToArray());
        Debug.Log(accu);
//        _neuralNetwork.PrintWeigths();
        StartCoroutine(Move(NeuralMovement));
    }

    public void Move(Tile tile)
    {
        var newPosition = tile.transform.position;
        newPosition.z -= 1;
        GenerateData(tile);
        transform.position = newPosition;
        CurrentTile.Active = false;
        CurrentTile = tile;
        CurrentTile.Active = true;
    }

    private void RandomMovement()
    {
        var random = Random.Range(0, 4);
        Tile newTile = null;
        if (random == 0)
            newTile = CurrentTile.Up;
        if (random == 1)
            newTile = CurrentTile.Right;
        if (random == 2)
            newTile = CurrentTile.Down;
        if (random == 3)
            newTile = CurrentTile.Left;

        if (newTile == null || newTile.Type == TileType.Rock)
        {
            RandomMovement();
            return;
        }

        Move(newTile);
    }

    private void NeuralMovement()
    {

        var input = SerializeMap();
        var movement = _neuralNetwork.Calculate(input.Select(x => (float) x).ToArray());
        var output = string.Empty;
        foreach (var move in movement)
        {
            output += move + " ";
        }
        Debug.Log(output);
        var bestValue = movement.Max();
        var bestIndex = Array.IndexOf(movement, bestValue);

//		Debug.Log(bestIndex + " " + bestValue);

        Tile newTile = null;

        if (bestIndex == 0)
            newTile = CurrentTile.Up;
        if (bestIndex == 1)
            newTile = CurrentTile.Right;
        if (bestIndex == 2)
            newTile = CurrentTile.Down;
        if (bestIndex == 3)
            newTile = CurrentTile.Left;

        if (newTile == null || newTile.Type == TileType.Rock)
        {
            //DIE WHEN LEARNING
            return;
        }

        Move(newTile);
    }

    private void Astar()
    {
        if (_path == null || !_path.Any())
        {
            Debug.Log("Not path to find");
            
            return;
        }

        Move(_path.Last());
        _path.RemoveAt(_path.Count - 1);
    }

//	private void DisplayWeigths()
//	{
//		for (int i = 0; i < _neuralNetwork.Weigths.Length; i++)
//		{
//			for (int j = 0; j < _neuralNetwork.Weigths[i].Length; j++)
//			{
//				var row = string.Empty;
//				for (int k = 0; k < _neuralNetwork.Weigths[i][j].Length; k++)
//				{
//					row += " " + _neuralNetwork.Weigths[i][j][k];
//				}
//				Debug.Log(row);
//			}
//		}
//	}

    private IEnumerator Move(Action moveMethod)
    {
        while (true)
        {
			yield return new WaitForSeconds(0.5f);
//            yield return new WaitForEndOfFrame();
//            yield return null;
            if (CurrentTile.Type == TileType.Finish)
            {
                LevelDrawer.Instance.RecreateRandomMap();
                _path = new AStar().FindPath(LevelDrawer.Instance.Tiles[Random.Range(0, 10), Random.Range(0, 10)], LevelDrawer.Instance.FinishTile);
                _amountOfData--;

                if (_amountOfData <= 0) {
                    DumpData();
                    Debug.Log("Done dumping");
                    yield break;
                    }

                Debug.Log("finished");
            }

            moveMethod();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("I died");
    }

    private int[] SerializeMap()
    {
        var map = new List<int>();
        foreach (var tile in LevelDrawer.Instance.Tiles)
        {
            if (tile.Active)
                map.Add(8);
            else
                map.Add((int) tile.Type);
        }

        return map.ToArray();
    }

    private int[] GetOutputValue(Tile newTile)
    {
        var output = new int[4];
        if (CurrentTile.Up == newTile)
            output[0] = 1;
        if (CurrentTile.Right == newTile)
            output[1] = 1;
        if (CurrentTile.Down == newTile)
            output[2] = 1;
        if (CurrentTile.Left == newTile)
            output[3] = 1;

        return output;
    }

    private void GenerateData(Tile newTile)
    {
        _data.Input.Add(SerializeMap());
        _data.Output.Add(GetOutputValue(newTile));
    }

    private Data GetData()
    {
        var path = Application.dataPath + "/data.xml";
        var serializer = new XmlSerializer(typeof(Data));

        using (var stream = new FileStream(path, FileMode.Open))
        {
            var data = serializer.Deserialize(stream);
            return (Data) data;
        }
    }

    private void DumpData()
    {
        var path = Application.dataPath + "/data.xml";
        var serializer = new XmlSerializer(typeof(Data));

        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, _data);
        }
    }
}