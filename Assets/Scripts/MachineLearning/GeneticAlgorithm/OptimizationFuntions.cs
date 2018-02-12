//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//
//
//namespace MachineLearning.GeneticAlgorithm
//{
//    public class OptimizationFuntions : IGeneticFunctions<float[][][]>
//    {
//        private PlayerBot _bot;
//        private NeuralNetwork.NeuralNetwork _neuralNetwork;
//
//        public OptimizationFuntions(NeuralNetwork.NeuralNetwork neuralNetwork)
//        {
////            _bot = bot;
//            _neuralNetwork = neuralNetwork;
//        }
//
//        public double ComputeFitness(float[][][] individual)
//        {
//            var simulation = new GameSimulation(LevelDrawer.Instance.Tiles, _neuralNetwork, individual).Simulate();
//            var timeNormalized = simulation.Time / 100f;
//            if (simulation.Finished)
//            {
//                return 100 - timeNormalized;
//            }
//
//            if (simulation.Died)
//            {
//                Debug.Log(timeNormalized + (simulation.DistanceToFinish));
//                return timeNormalized + 100 - simulation.DistanceToFinish;
//            }
//
//            Debug.Log("Not finished or died");
//            return Random.Range(0f, 1f);
//            //TODO: Simulate learning behaviour
//        }
//
//        public float[][][] CreateIndividual()
//        {
//            for (int i = 0; i < _neuralNetwork.Weigths.Length; i++)
//            {
//                for (int j = 0; j < _neuralNetwork.Weigths[i].Length; j++)
//                {
//                    for (int k = 0; k < _neuralNetwork.Weigths[i][j].Length; k++)
//                    {
//                        _neuralNetwork.Weigths[i][j][k] = Random.Range(0f, 1f);
//                    }
//                }
//            }
//
//            return _neuralNetwork.Weigths;
//        }
//
//        public float[][][] Mutate(float[][][] individual, double mutationRate)
//        {
//            if (Random.Range(0f, 1f) < mutationRate)
//            {
//                Debug.Log("Mutate");
//                var randomLayer = Random.Range(0, individual.Length);
//                var randomNodeOne = Random.Range(0, individual[randomLayer].Length);
//                var randomNodeTwo = Random.Range(0, individual[randomLayer][randomNodeOne].Length);
//
//                individual[randomLayer][randomNodeOne][randomNodeTwo] += Random.Range(-1, 1);
//            }
//
//            return individual;
//        }
//
//        public Tuple<float[][][], float[][][]> SelectParents(float[][][][] individuals, double[] fitnesses)
//        {
//            var totalFitness = fitnesses.Sum();
//            var parents = new List<float[][][]>();
//
//            var index = 0;
//
//            while (parents.Count < 2)
//            {
//                var chance = Random.Range(0f, 1f);
//
//                if (chance < fitnesses[index] / totalFitness)
//                {
//                    parents.Add(individuals[index++]);
//                }
//
//                if (index + 1 == parents.Count)
//                {
//                    index = 0;
//                }
//            }
//
//            _neuralNetwork.Weigths = parents[0];
//
//            return new Tuple<float[][][], float[][][]>(parents[0], parents[1]);
//        }
//
//        public Tuple<float[][][], float[][][]> Crossover(Tuple<float[][][], float[][][]> parents)
//        {
//            float[][][] childOne = parents.Item1;
//            float[][][] childTwo = parents.Item2;
//
//            for (var i = 0; i < parents.Item1.Length; i++)
//            {
//                var layerPOne = parents.Item1[i];
//                var layerPTwo = parents.Item2[i];
//
//                var mid = (layerPOne.Length / 2) - 1;
//                var midmid = (layerPOne[mid].Length / 2) - 1;
//
//                for (var k = 0; k < mid; k++)
//                {
//                    for (int j = 0; j < layerPOne[k].Length; j++)
//                    {
//                        childOne[i][k][j] = layerPOne[k][j];
//                        childTwo[i][k][j] = layerPTwo[k][j];
//                    }
//                }
//
//                for (var k = mid + 1; k < layerPOne.Length; k++)
//                {
//                    for (int j = 0; j < layerPOne[k].Length; j++)
//                    {
//                        childOne[i][k][j] = layerPTwo[k][j];
//                        childTwo[i][k][j] = layerPOne[k][j];
//                    }
//                }
//
//                for (int j = 0; j < midmid; j++)
//                {
//                    childOne[i][mid][j] = layerPOne[mid][j];
//                    childTwo[i][mid][j] = layerPTwo[mid][j];
//                }
//                
//                for (int j = midmid + 1; j < layerPOne[0].Length; j++)
//                {
//                    childOne[i][mid][j] = layerPTwo[mid][j];
//                    childTwo[i][mid][j] = layerPOne[mid][j];
//                }
//            }
//
//            return new Tuple<float[][][], float[][][]>(childOne, childTwo);
//        }
//    }
//
//}