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
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class PlayerBot : MonoBehaviour
{
    public Tile CurrentTile;
    private NeuralNetwork _neuralNetwork;
    private List<Tile> _path;
    private Data _data;
    private int _amountOfData = 5000;
    private int _learnAmount = 500;
    private Tile _prevTile;

    private void Start()
    {
        _data = new Data();
//		_neuralNetwork = new NeuralNetwork(new[] {6, 5, 4});
        CurrentTile = LevelDrawer.Instance.Tiles[Random.Range(0, 10), Random.Range(0, 10)];
//		DisplayWeigths();
//		var xor = new Xor();
//        _path = new AStar().FindPath(CurrentTile, LevelDrawer.Instance.FinishTile);
//        StartCoroutine(Move(Astar));
//        _data = GetData();
//        var dataSize = 30000;
//        _neuralNetwork = new NeuralNetwork(new[] {101, 40, 4});
//        _neuralNetwork.Learn(_data.Input.Take(dataSize).Select(x => x.Select(y => (float) y).ToArray()).ToArray(), _data.Output.Take(dataSize).Select(x => x.Select(y => (float) y).ToArray()).ToArray(), 1.3f);
//        var accu = _neuralNetwork.Test(_data.Input.Skip(dataSize).Select(x => x.Select(y => (float) y).ToArray()).ToArray(),
//            _data.Output.Skip(dataSize).Select(x => x.Select(y => (float) y).ToArray()).ToArray());
//        Debug.Log(accu);
//        _neuralNetwork.PrintWeigths();
//        StartCoroutine(Move(NeuralMovement));

        _neuralNetwork = new NeuralNetwork(new[] {7, 5, 4});
        StartCoroutine(Move(LearnRealTime));
    }

    public void Move(Tile tile)
    {
        if (tile == null)
            return;

        var newPosition = tile.transform.position;
        newPosition.z -= 1;
//        GenerateData(tile);
        transform.position = newPosition;
        CurrentTile.Active = false;
        _prevTile = CurrentTile;
        CurrentTile = tile;
        CurrentTile.Active = true;
    }

    private void LearnRealTime()
    {
        var tiles = CurrentTile.GetTileMapping();
        var finish = LevelDrawer.Instance.FinishTile;
        var x = CurrentTile.X - finish.X;
        var y = CurrentTile.Y - finish.Y;
        var input = new float[6];
        input[0] = x;
        input[1] = y;
        for (int i = 0; i < tiles.Length; i++)
        {
            input[i + 2] = tiles[i];
        }

        var output = GetBestTile(input);

        if (output == null)
            LevelDrawer.Instance.RecreateRandomMap();

        _neuralNetwork.Learn(new float[][] {input}, new float[][] {output}, 0.3f);

        if (_learnAmount > 0)
        {
            Move(GetNextTile(output));
        }
        else
        {
            Debug.Log("Machine learning does this move!");
            var next = _neuralNetwork.Calculate(input);
            Move(GetNextTile(next));
        }
    }

    private float[] GetBestTile(float[] input)
    {
//        Tile bestNeighbour;
//        var bestNeighbourX = 0;
//        var bestNeighbourY = 0;
//        
//        for (int i = 2; i < input.Length - 2; i++)
//        {
//            if(input[i] == 0)
//                continue;
//
//            var neighbour = CurrentTile.NeighbourByIndex(i - 2);
//            var x = neighbour.X - LevelDrawer.Instance.FinishTile.X;
//        }
        var x = input[0];
        var y = input[1];
        var output = new float[4];
        //Try to get the best tile
        if (x > 0 && input[5] != 0)
            output = new float[] {0, 0, 0, 1};
        if (x < 0 && input[3] != 0)
            output = new float[] {0, 1, 0, 0};
        if (y > 0 && input[4] != 0)
            output = new float[] {0, 0, 1, 0};
        if (y < 0 && input[2] != 0)
            output = new float[] {1, 0, 0, 0};

        var tile = GetNextTile(output);

        //If we can't find a better Tile, get a random tile
        if (tile != null && tile != _prevTile)
            return output;
        
        return GetRandomTile(input);
    }

    private float[] GetRandomTile(float[] input)
    {
        var output = new float[4];
        var random = Random.Range(2, 6);
        if (input[random] != 0)
        {
            output[random - 2] = 1;
            return output;
        }

        return GetRandomTile(input);
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
            return;
        }

        Move(newTile);
    }

    private Tile GetNextTile(float[] output)
    {
        var bestValue = output.Max();
        var bestIndex = Array.IndexOf(output, bestValue);


        Tile newTile = null;

        if (bestIndex == 0)
            newTile = CurrentTile.Up;
        if (bestIndex == 1)
            newTile = CurrentTile.Right;
        if (bestIndex == 2)
            newTile = CurrentTile.Down;
        if (bestIndex == 3)
            newTile = CurrentTile.Left;

        return newTile;
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

    private IEnumerator Move(Action moveMethod)
    {
        while (true)
        {
            if (_learnAmount <= 0)
                yield return new WaitForSeconds(0.5f);
            else
            {
                yield return new WaitForEndOfFrame();
            }

//            yield return null;
            if (CurrentTile.Type == TileType.Finish)
            {
                LevelDrawer.Instance.RecreateRandomMap();
                _path = new AStar().FindPath(LevelDrawer.Instance.Tiles[Random.Range(0, 10), Random.Range(0, 10)],
                    LevelDrawer.Instance.FinishTile);
                if(_path == null)
                    LevelDrawer.Instance.RecreateRandomMap();
//                _amountOfData--;
                _learnAmount--;
//
//                if (_amountOfData <= 0) {
//                    DumpData();
//                    Debug.Log("Done dumping");
//                    yield break;
//                    }

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