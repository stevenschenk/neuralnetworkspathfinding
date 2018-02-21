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
    private int _learnAmount = 100000000;
    private Tile _prevTile;

    private void Start()
    {
        Application.runInBackground = true;
        CurrentTile = RandomWalkableTile();
        _path = new AStar().FindPath(CurrentTile, LevelDrawer.Instance.FinishTile);
        _neuralNetwork = new NeuralNetwork(new[] {11, 7, 4});
        StartCoroutine(Move(LearnRealTime));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            _learnAmount = 0;
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
        CurrentTile.Counter++;
    }

    private float GetTileWeigth(Tile tile)
    {
        if (tile == null)
            return 10f;
        
        var weigth = 0f;
        if (Vector2.Distance(CurrentTile.transform.position, LevelDrawer.Instance.FinishTile.transform.position) <
            Vector2.Distance(tile.transform.position, LevelDrawer.Instance.FinishTile.transform.position))
        {
            weigth += 0.2f;
        }

        if (tile.Counter > 0)
        {
            weigth += 0.1f * tile.Counter;
        }
        

        if (tile.Type == TileType.Rock)
            weigth += 10f;

        if (tile.Type == TileType.Finish)
            weigth -= 1;

        return weigth;
    }

    private void LearnRealTime()
    {
//        var tiles = CurrentTile.GetTileMapping();
//        var finish = LevelDrawer.Instance.FinishTile;
//        var x = CurrentTile.X - finish.X;
//        var y = CurrentTile.Y - finish.Y;
//        var input = new float[6];
//        input[0] = x;
//        input[1] = y;
//        input[2] = CurrentTile.Up != null ? CurrentTile.Up.Counter : -1f;
//        input[3] = CurrentTile.Right != null ? CurrentTile.Right.Counter : -1f;
//        input[4] = CurrentTile.Down != null ? CurrentTile.Down.Counter : -1f;
//        input[5] = CurrentTile.Left != null ? CurrentTile.Left.Counter : -1f;


//        input[10] = CurrentTile.Up == _prevTile ? 0 : input[10];
//        input[10] = CurrentTile.Right == _prevTile ? 1 : input[10];
//        input[10] = CurrentTile.Down == _prevTile ? 2 : input[10];
//        input[10] = CurrentTile.Left == _prevTile ? 3 : input[10];
////        for (int i = 0; i < tiles.Length; i++)
////        {
////            input[i + 2] = tiles[i];
////        }
//
//        input[2] = CurrentTile.Up != null && CurrentTile.Up.Type == TileType.Snow ? 1 : input[2];
//        input[2] = CurrentTile.Up != null && CurrentTile.Up.Type == TileType.Finish ? 1 : input[2];
//        input[2] = CurrentTile.Up != null && CurrentTile.Up.Type == TileType.Visited ? 2 : input[2];
//        input[3] = CurrentTile.Right != null && CurrentTile.Right.Type == TileType.Finish ? 1 : input[3];
//        input[3] = CurrentTile.Right != null && CurrentTile.Right.Type == TileType.Snow ? 1 : input[3];
//        input[3] = CurrentTile.Right != null && CurrentTile.Right.Type == TileType.Visited ? 2 : input[3];
//        input[4] = CurrentTile.Down != null && CurrentTile.Down.Type == TileType.Finish ? 1 : input[4];
//        input[4] = CurrentTile.Down != null && CurrentTile.Down.Type == TileType.Snow ? 1 : input[4];
//        input[4] = CurrentTile.Down != null && CurrentTile.Down.Type == TileType.Visited ? 2 : input[4];
//        input[5] = CurrentTile.Left != null && CurrentTile.Left.Type == TileType.Finish ? 1 : input[5];
//        input[5] = CurrentTile.Left != null && CurrentTile.Left.Type == TileType.Snow ? 1 : input[5];
//        input[5] = CurrentTile.Left != null && CurrentTile.Left.Type == TileType.Visited ? 2 : input[5];
//
//        input[6] = CurrentTile.Up != null && CurrentTile.Up.Up != null && CurrentTile.Up.Up.Type == TileType.Snow
//            ? 1
//            : input[6];
//        input[6] = CurrentTile.Up != null && CurrentTile.Up.Up != null && CurrentTile.Up.Up.Type == TileType.Finish
//            ? 1
//            : input[6];
//        input[6] = CurrentTile.Up != null && CurrentTile.Up.Up != null && CurrentTile.Up.Up.Type == TileType.Visited
//            ? 2
//            : input[6];
//        input[7] = CurrentTile.Right != null && CurrentTile.Right.Right != null &&
//                   CurrentTile.Right.Right.Type == TileType.Finish
//            ? 1
//            : input[7];
//        input[7] = CurrentTile.Right != null && CurrentTile.Right.Right != null &&
//                   CurrentTile.Right.Right.Type == TileType.Snow
//            ? 1
//            : input[7];
//        input[7] = CurrentTile.Right != null && CurrentTile.Right.Right != null &&
//                   CurrentTile.Right.Right.Type == TileType.Visited
//            ? 2
//            : input[7];
//        input[8] = CurrentTile.Down != null && CurrentTile.Down.Down != null &&
//                   CurrentTile.Down.Down.Type == TileType.Finish
//            ? 1
//            : input[8];
//        input[8] = CurrentTile.Down != null && CurrentTile.Down.Down != null &&
//                   CurrentTile.Down.Down.Type == TileType.Snow
//            ? 1
//            : input[8];
//        input[8] = CurrentTile.Down != null && CurrentTile.Down.Down != null &&
//                   CurrentTile.Down.Down.Type == TileType.Visited
//            ? 2
//            : input[8];
//        input[9] = CurrentTile.Left != null && CurrentTile.Left.Left != null &&
//                   CurrentTile.Left.Left.Type == TileType.Finish
//            ? 1
//            : input[9];
//        input[9] = CurrentTile.Left != null && CurrentTile.Left.Left != null &&
//                   CurrentTile.Left.Left.Type == TileType.Snow
//            ? 1
//            : input[9];
//        input[9] = CurrentTile.Left != null && CurrentTile.Left.Left != null &&
//                   CurrentTile.Left.Left.Type == TileType.Visited
//            ? 2
//            : input[9];


//        if (output == null)
//        {
//            LevelDrawer.Instance.RecreateRandomMap();
//            CurrentTile = RandomWalkableTile();
//            _path = new AStar().FindPath(CurrentTile, LevelDrawer.Instance.FinishTile);
//            return;
//        }


//        var a = string.Empty;
//        foreach (var f in output)
//        {
//            a += f + " ";
//        }
//        Debug.Log(a);

        if (CurrentTile.Type == TileType.Rock)
            CurrentTile = RandomWalkableTile();

        var finish = LevelDrawer.Instance.FinishTile;
        var x = CurrentTile.X - finish.X;
        var y = CurrentTile.Y - finish.Y;
        var input = SerializeMapRange(1).ToList();

        if (x > 1 || x < -1)
            x = x / Mathf.Abs(x);
        if (y > 1 || y < -1)
            y = y / Mathf.Abs(y);

        input.Add(x);
        input.Add(y);
        input.Add(CurrentTile.Up != null && CurrentTile.Up.Counter > 0 ? Mathf.Clamp(0.1f * CurrentTile.Up.Counter, 0f, 1f) : 0);
        input.Add(CurrentTile.Right != null && CurrentTile.Right.Counter > 0 ? Mathf.Clamp(0.1f * CurrentTile.Right.Counter, 0f, 1f) : 0);
        input.Add(CurrentTile.Down != null && CurrentTile.Down.Counter > 0 ? Mathf.Clamp(0.1f * CurrentTile.Down.Counter, 0f, 1f) : 0);
        input.Add(CurrentTile.Left != null && CurrentTile.Left.Counter > 0 ? Mathf.Clamp(0.1f * CurrentTile.Left.Counter, 0f, 1f) : 0);


        if (_learnAmount > 0)
        {
//            var output = GetBestTile(input.ToArray());
            var output = new float[4];

            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                    output[i] = GetTileWeigth(CurrentTile.Up);
                if (i == 1)
                    output[i] = GetTileWeigth(CurrentTile.Right);
                if (i == 2)
                    output[i] = GetTileWeigth(CurrentTile.Down);
                if (i == 3)
                    output[i] = GetTileWeigth(CurrentTile.Left);
            }

            var min = output.Min();
            var outputVal = output.Select(m => m = m == min ? 1 : 0);
            
            if (outputVal == null)
            {
                Debug.Log("output is null");
                return;
            }

            var outp = string.Empty;

            foreach (var o in outputVal)
            {
                outp += o + " ";
            }
            Debug.Log(outp);

            _neuralNetwork.Learn(new float[][] {input.ToArray()}, new float[][] {outputVal.ToArray()}, 0.3f);
            Move(GetNextTile(outputVal.ToArray()));
        }
        else
        {
            Debug.Log("Machine learning does this move!");
            var next = _neuralNetwork.Calculate(input.ToArray());
            var a = string.Empty;
            foreach (var f in next)
            {
                a += f + " ";
            }

            Debug.Log(a);
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
//
//        float[] output;
//
//        //Environment info
//        var x = input[0];
//        var y = input[1];
//        var tileInfo = input.Skip(2).ToArray();
//        var neighbours = tileInfo.Take(4).ToArray();
//        var surrounding = tileInfo.Skip(4).ToArray();
//        var prev = input[10];
////
////        for (int i = 0; i < surrounding.Length; i++)
////        {
////            Debug.Log(i + " = " + surrounding[i]);
////        }
//
//        var walkableTiles = new HashSet<int>();
//        var visitedTiles = new HashSet<int>();
//
//        for (int i = 0; i < neighbours.Length; i++)
//        {
//            var neighbour = neighbours[i];
//            if (neighbour == 1)
//                walkableTiles.Add(i);
//            if (neighbour == 2)
//                visitedTiles.Add(i);
//        }
//
//        var tileDistanceToIgloo = new Dictionary<int, float>();
//        var tileDistanceToIglooVisited = new Dictionary<int, float>();
//
//        //Get tiles that are closer
//        foreach (var walkableTile in walkableTiles)
//        {
//            if (walkableTile == 0)
//                tileDistanceToIgloo[walkableTile] = Mathf.Abs(x) + Mathf.Abs(y + 1);
//            if (walkableTile == 2)
//                tileDistanceToIgloo[walkableTile] = Mathf.Abs(x) + Mathf.Abs(y - 1);
//            if (walkableTile == 1)
//                tileDistanceToIgloo[walkableTile] = Mathf.Abs(x + 1) + Mathf.Abs(y);
//            if (walkableTile == 3)
//                tileDistanceToIgloo[walkableTile] = Mathf.Abs(x - 1) + Mathf.Abs(y);
//        }
//
//        foreach (var visitedTile in visitedTiles)
//        {
//            if (visitedTile == 0)
//                tileDistanceToIglooVisited[visitedTile] = Mathf.Abs(x) + Mathf.Abs(y + 1);
//            if (visitedTile == 2)
//                tileDistanceToIglooVisited[visitedTile] = Mathf.Abs(x) + Mathf.Abs(y - 1);
//            if (visitedTile == 1)
//                tileDistanceToIglooVisited[visitedTile] = Mathf.Abs(x + 1) + Mathf.Abs(y);
//            if (visitedTile == 3)
//                tileDistanceToIglooVisited[visitedTile] = Mathf.Abs(x - 1) + Mathf.Abs(y);
//        }
//
//        var tilesOrdered = tileDistanceToIgloo.OrderBy(t => t.Value).Select(t => t.Key).ToList();
//        var index = 0;
//        while (index < tilesOrdered.Count)
//        {
//            var nextTile = tilesOrdered[index++];
//
//            if (visitedTiles.Contains(nextTile))
//                continue;
//
//            output = new float[4];
//            output[nextTile] = 1;
//            return output;
//        }
//
//        Debug.Log("No good tile found");
//
//        if (!visitedTiles.Any())
//        {
//            LevelDrawer.Instance.RecreateRandomMap();
//            return null;
//        }
//
//        //All possible tiles are already visited
//        var nextBests = tileDistanceToIglooVisited.OrderBy(t => t.Value).Select(t => t.Key)
//            .Where(t => surrounding[t] != 0 && t != prev);
//        output = new float[4];
//        if (nextBests.Any())
//            output[nextBests.First()] = 1;
//        
//        if (tileDistanceToIglooVisited.Count > 1)
//            output[tileDistanceToIglooVisited.OrderBy(t => t.Value).Select(t => t.Key).First(t => t != prev)] = 1;
//        else
//            output[tileDistanceToIglooVisited.First().Key] = 1;
//
//
//        return output;


        //1
//        _path = new AStar().FindPath(CurrentTile, LevelDrawer.Instance.FinishTile);
//        if (_path == null)
//        {
//            LevelDrawer.Instance.RecreateRandomMap();
//            return null;
//        }
//
//        var x = input[0];
//        var y = input[1];
//        var tiles = input.Skip(2).ToArray();
//        var availableTiles = new HashSet<int>();
//        var output = new float[4];
//
//        for (int i = 0; i < tiles.Count(); i++)
//        {
//            if (tiles[i] >= 0)
//                availableTiles.Add(i);
//        }
//
//        if (!availableTiles.Any())
//        {
//            LevelDrawer.Instance.RecreateRandomMap();
//            return null;
//        }
//
//        var tileDistanceToIgloo = new Dictionary<int, float>();
//
//        foreach (var walkableTile in availableTiles)
//        {
//            if (walkableTile == 0)
//                tileDistanceToIgloo[walkableTile] = Mathf.Abs(x) + Mathf.Abs(y + 1);
//            if (walkableTile == 2)
//                tileDistanceToIgloo[walkableTile] = Mathf.Abs(x) + Mathf.Abs(y - 1);
//            if (walkableTile == 1)
//                tileDistanceToIgloo[walkableTile] = Mathf.Abs(x + 1) + Mathf.Abs(y);
//            if (walkableTile == 3)
//                tileDistanceToIgloo[walkableTile] = Mathf.Abs(x - 1) + Mathf.Abs(y);
//        }
//
//        var tilesOrderedByDistanceToFinish = tileDistanceToIgloo.OrderBy(i => tiles[i.Key]).GroupBy(i => tiles[i.Key]).First()
//            .ToList().OrderBy(i => i.Value).First().Key;
//
//        for (int i = 0; i < output.Length; i++)
//        {
//            if (tilesOrderedByDistanceToFinish == i)
//                output[i] = 1;
//        }
//
//        return output;
        var output = new float[4];

        if (_path == null || !_path.Any())
        {
            Debug.Log("no path possible");
            LevelDrawer.Instance.RecreateRandomMap();
            CurrentTile = RandomWalkableTile();
            _path = new AStar().FindPath(CurrentTile, LevelDrawer.Instance.FinishTile);
            return null;
        }

        var best = _path.Last();
        _path.Remove(best);

        if (CurrentTile == best)
        {
            best = _path.Last();
            _path.Remove(best);
        }


        if (CurrentTile.Up == best)
            output[0] = 1;
        if (CurrentTile.Right == best)
            output[1] = 1;
        if (CurrentTile.Down == best)
            output[2] = 1;
        if (CurrentTile.Left == best)
            output[3] = 1;

        return output;


//        var x = input[0];
//        var y = input[1];
//        var output = new float[4];
//        //Try to get the best tile
//        if (x > 0 && input[5] != 0)
//            output = new float[] {0, 0, 0, 1};
//        if (x < 0 && input[3] != 0)
//            output = new float[] {0, 1, 0, 0};
//        if (y > 0 && input[4] != 0)
//            output = new float[] {0, 0, 1, 0};
//        if (y < 0 && input[2] != 0)
//            output = new float[] {1, 0, 0, 0};
//
//        var tile = GetNextTile(output);
//
//        //If we can't find a better Tile, get a random tile
//        if (tile != null && tile != _prevTile)
//            return output;
//        
//        return GetRandomTile(input);

//        if (_path == null || !_path.Any())
//        {
//            Debug.Log("No path to find");
//
//            return null;
//        }
//
//        var nextTile = _path.Last();
//        _path.Remove(nextTile);
//        if (nextTile == CurrentTile)
//        {
//            nextTile = _path.Last();
//            _path.Remove(nextTile);
//        }
//        


//        return GetOutputValue(nextTile);
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
        Debug.Log(bestIndex);


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
//                yield return new WaitForSeconds(0.5f);

                yield return new WaitForEndOfFrame();
            }

//            yield return null;
            if (CurrentTile.Type == TileType.Finish)
            {
                _path = null;
                while (_path == null)
                {
                    LevelDrawer.Instance.RecreateRandomMap();
                    CurrentTile = RandomWalkableTile();
                    _path = new AStar().FindPath(CurrentTile,
                        LevelDrawer.Instance.FinishTile);
                }

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

    private Tile RandomWalkableTile()
    {
        var x = Random.Range(0, 10);
        var y = Random.Range(0, 10);

        var tile = LevelDrawer.Instance.Tiles[x, y];

        if (tile.Type == TileType.Finish || tile.Type == TileType.Rock)
            return RandomWalkableTile();

        return tile;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("I died");
    }

    private float[] SerializeMap()
    {
        var map = new List<float>();
        foreach (var tile in LevelDrawer.Instance.Tiles)
        {
            if (tile.Active)
                map.Add(1);
            else if (tile.Type == TileType.Finish)
                map.Add(0.8f);
            else if (tile.Type == TileType.Rock)
                map.Add(0);
            else
                map.Add(0.5f);
        }

        return map.ToArray();
    }

    private float[] SerializeMapRange(int range)
    {
        var map = new List<float>();
        var x = CurrentTile.X;
        var y = CurrentTile.Y;

        for (int i = 1; i <= range; i++)
        {
            if (i < 0 || i > 9)
            {
                map.Add(0);
                continue;
            }

            if (CurrentTile.X + i <= 9)
                map.Add(TypeToFloat(LevelDrawer.Instance.Tiles[CurrentTile.X + i, CurrentTile.Y].Type));
            else
                map.Add(0.1f);
            
            if (CurrentTile.X - i >= 0)
                map.Add(TypeToFloat(LevelDrawer.Instance.Tiles[CurrentTile.X - i, CurrentTile.Y].Type));
            else
                map.Add(0.1f);
            
            if (CurrentTile.Y + i <= 9)
                map.Add(TypeToFloat(LevelDrawer.Instance.Tiles[CurrentTile.X, CurrentTile.Y + i].Type));
            else
                map.Add(0.1f);
            
            if (CurrentTile.Y - i >= 0)
                map.Add(TypeToFloat(LevelDrawer.Instance.Tiles[CurrentTile.X, CurrentTile.Y - i].Type));
            else
                map.Add(0.1f);
        }

//        for (int i = x - range; i < x + range + 1; i++)
//        {
//            for (int j = y - range; j < y + range + 1; j++)
//            {
//                if (i < 0 || i > 9 || j < 0 || j > 9)
//                {
//                    map.Add(0);
//                    continue;
//                }
//                    
//                
//                var tile = LevelDrawer.Instance.Tiles[i, j];
//
//                if (tile.Active)
//                    map.Add(1);
//                else if (tile.Type == TileType.Finish)
//                    map.Add(0.8f);
//                else if (tile.Type == TileType.Rock)
//                    map.Add(0.1f);
//                else
//                    map.Add(0.5f);
//            }
//        }

        return map.ToArray();
    }

    private float TypeToFloat(TileType type)
    {
        if (type == TileType.Finish)
            return 1f;
        if (type == TileType.Rock)
            return 0.01f;

        return 0.8f;
    }

    private float[] GetOutputValue(Tile newTile)
    {
        var output = new float[4];
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
//        _data.Input.Add(SerializeMap());
//        _data.Output.Add(GetOutputValue(newTile));
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