using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class AStar
    {
        private HashSet<Tile> _closedSet;
        private HashSet<Tile> _openSet;
        private Dictionary<Tile, Tile> _cameFrom;
        private Dictionary<Tile, float> _gScores;
        private Dictionary<Tile, float> _fScores;
        private Tile _end;

        public AStar()
        {
            Clear();
        }

        public List<Tile> FindPath(Tile start, Tile end)
        {
            Init(start, end);

            while (_openSet.Any())
            {
                var current = _openSet.OrderBy(x => _fScores[x]).First();

                if (current == _end)
                {
                    return ReconstructPath(_cameFrom, current);
                }

                _openSet.Remove(current);
                _closedSet.Add(current);

                var neighbours = current.GetNeighbours();

                foreach (var neighbour in neighbours)
                {
                    if(_closedSet.Contains(neighbour))
                        continue;

                    _openSet.Add(neighbour);

                    float gScore = 0;

                    if (_gScores.ContainsKey(current))
                        gScore = _gScores[current] + ManhattanDistance(current, neighbour);
                    else
                        gScore = float.PositiveInfinity + ManhattanDistance(current, neighbour);

                    if (_gScores.ContainsKey(neighbour) && gScore >= _gScores[neighbour])
                    {
                        continue;
                    }
                    else
                    {
                        _cameFrom[neighbour] = current;
                        _gScores[neighbour] = gScore;
                        _fScores[neighbour] = gScore + ManhattanDistance(neighbour, _end);
                    }

                }

            }

            return null;
        }

        private void Init(Tile start, Tile end)
        {
            _gScores[start] = 0;
            _fScores[start] = HeuristicCostEstimate(start, end);
            _openSet.Add(start);
            _end = end;
        }

        public void Clear()
        {
            _closedSet = new HashSet<Tile>();
            _openSet = new HashSet<Tile>();
            _cameFrom = new Dictionary<Tile, Tile>();
            _gScores = new Dictionary<Tile, float>();
            _fScores = new Dictionary<Tile, float>();
        }

        private float HeuristicCostEstimate(Tile begin, Tile end)
        {
            return ManhattanDistance(begin, end);
        }

        private float ManhattanDistance(Tile begin, Tile end)
        {
            return Mathf.Abs(begin.X - end.X) + Mathf.Abs(begin.Y - end.Y);
        }

        private List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
        {
            var totalPath = new List<Tile> {current};
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }

            return totalPath;
        }
        
    }
}