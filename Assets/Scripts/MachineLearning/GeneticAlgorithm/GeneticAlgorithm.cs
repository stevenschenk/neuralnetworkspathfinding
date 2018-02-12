using System;
using UnityEngine;
using Random = System.Random;

namespace MachineLearning.GeneticAlgorithm
{
    public class GeneticAlgorithm<T>
    {
        private readonly int _maxGenerations;
        private readonly int _populationSize;
        private readonly double _mutationRate;
        private readonly double _crossoverRate;
        private readonly bool _elitism;
        private readonly IGeneticFunctions<T> _functions;
        private readonly Random _random;


        public GeneticAlgorithm(IGeneticFunctions<T> functions, int maxGenerations, int populationSize,
            double mutationRate, double crossoverRate, bool elitism)
        {
            _functions = functions;
            _maxGenerations = maxGenerations;
            _populationSize = populationSize;
            _mutationRate = mutationRate;
            _crossoverRate = crossoverRate;
            _elitism = elitism;
            _random = new Random();
        }

        public void Run()
        {
            var currentPopulation = CreateInitialPopulation();

            for (var generation = 0; generation < _maxGenerations; generation++)
            {
                var fitnesses = ComputeFitnesses(currentPopulation);
                var newPopulation = new T[_populationSize];
                PrintCurrentPopulation(currentPopulation, fitnesses, generation);
                PrintResult(currentPopulation, fitnesses);
                var startIndex = 0;

                if (_elitism)
                {
                    int bestIndividual = 0;
                    var bestFitness = double.MinValue;

                    for (var i = 0; i < currentPopulation.Length; i++)
                    {
                        if (fitnesses[i] > bestFitness)
                        {
                            bestFitness = fitnesses[i];
                            bestIndividual = i;
                        }
                    }

                    newPopulation[0] = currentPopulation[bestIndividual];
                    startIndex = 1;
                }


                for (var i = startIndex; i < _populationSize; i++)
                {
                    var parents = _functions.SelectParents(currentPopulation, fitnesses);
                    var children = parents;

                    if (_random.NextDouble() < _crossoverRate)
                        children = _functions.Crossover(parents);


                    newPopulation[i++] = _functions.Mutate(children.Item1, _mutationRate);

                    if (i < _populationSize)
                        newPopulation[i] = _functions.Mutate(children.Item2, _mutationRate);
                }

                currentPopulation = newPopulation;

                if (generation + 1 == _maxGenerations)
                {
                    fitnesses = ComputeFitnesses(currentPopulation);
                    PrintCurrentPopulation(currentPopulation, fitnesses, generation + 1);
                    PrintResult(currentPopulation, fitnesses);
                }
            }
        }

        private double[] ComputeFitnesses(T[] population)
        {
            var fitnesses = new double[population.Length];

            for (var i = 0; i < population.Length; i++)
            {
                fitnesses[i] = _functions.ComputeFitness(population[i]);
            }

            return fitnesses;
        }

        private T[] CreateInitialPopulation()
        {
            var population = new T[_populationSize];

            for (var i = 0; i < _populationSize; i++)
            {
                population[i] = _functions.CreateIndividual();
            }

            return population;
        }

        private static void PrintCurrentPopulation(T[] individuals, double[] fitnesses, int generation)
        {
            Console.WriteLine("### Generation " + generation + " ###");

            for (var i = 0; i < individuals.Length; i++)
            {
                Console.WriteLine("Individual " + individuals[i] + " with fitness {fitnesses[i]}");
            }

            Console.WriteLine('\n');
        }

        private static void PrintResult(T[] individuals, double[] fitnesses)
        {
            var bestIndex = 0;
            var bestFitness = double.MinValue;
            var avg = 0.0;

            for (int i = 0; i < fitnesses.Length; i++)
            {
                if (fitnesses[i] > bestFitness)
                {
                    bestFitness = fitnesses[i];
                    bestIndex = i;
                }

                avg += fitnesses[i] / fitnesses.Length;
            }

            Debug.Log("### Result ###");
            Debug.Log("Best fitness = " + bestFitness);
            Debug.Log("Avg fitness =" + avg);
            Debug.Log("Best inidividual = " + individuals[bestIndex]);
        }
    }
}