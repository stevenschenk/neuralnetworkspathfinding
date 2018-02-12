//using System;
//using System.Diagnostics;
//using System.Linq;
//using MachineLearning.NeuralNetwork;
//using UnityEngine;
//using Debug = UnityEngine.Debug;
//
//public class GameSimulation
//{
//    public float Time;
//    public bool Finished;
//    public bool Died;
//    public float DistanceToFinish;
//
//    private Tile[,] _map;
//    private Tile _currentPlayerTile;
//    private NeuralNetwork _network;
//    private Tile _finish;
//
//    public GameSimulation(Tile[,] map, NeuralNetwork network, float[][][] weights)
//    {
//        _map = map;
//        _network = network;
//        _currentPlayerTile = _map[1, 1];
//        _finish = LevelDrawer.Instance.FinishTile;
//        _network.Weigths = weights;
//    }
//
//    public GameSimulation Simulate()
//    {
//        while (!Finished && !Died && Time < 100)
//        {
//            Time++;
//            var finishPos = new float[] {_currentPlayerTile.X - _finish.X, _currentPlayerTile.Y - _finish.Y,};
//            var tileInfo = _currentPlayerTile.GetTileMapping();
//            var input = new float[6];
//            finishPos.CopyTo(input, 0);
//            tileInfo.CopyTo(input, finishPos.Length);
//
//            var movement = _network.Calculate(input);
//
//            var bestValue = movement.Max();
//            var bestIndex = Array.IndexOf(movement, bestValue);
//            var nextTile = GetTile(bestIndex, _currentPlayerTile);
//
//
//            if (nextTile == null || nextTile.Type == TileType.Rock)
//            {
//                DistanceToFinish = Vector2.Distance(new Vector2(_finish.X, _finish.Y),
//                    new Vector2(_currentPlayerTile.X, _currentPlayerTile.Y));
//                Died = true;
//                break;
//            }
//
//            if (nextTile.Type == TileType.Finish)
//            {
//                Finished = true;
//                break;
//            }
//
//            _currentPlayerTile = nextTile;
//        }
//
//        return this;
//    }
//
//    private Tile GetTile(int i, Tile t)
//    {
//        
//        if (i == 0)
//            return t.Up;
//        if (i == 1)
//            return t.Right;
//        if (i == 2)
//            return t.Down;
//        if (i == 3)
//            return t.Left;
//
//        return null;
//    }
//}