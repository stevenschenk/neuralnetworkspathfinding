using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DefaultNamespace;
using MachineLearning.GeneticAlgorithm;
using NUnit.Framework.Constraints;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MachineLearning.NeuralNetwork
{
    public class NeuralNetwork
    {
        private readonly List<Layer> _layers;
        private bool _useBias;
        private float _error = 0;
        private float _iterations = 0;
        

        public NeuralNetwork(List<Layer> layers)
        {
            _layers = layers;
        }

        public NeuralNetwork(int[] topology, bool useBias = true)
        {
            _layers = CreateNetwork(topology);

            if (useBias)
            {
                _useBias = true;
                MakeBiasesFromEveryFirstNeuron();
            }
            
            InitRandomWeights(-4f, 4f);
        }


        public float[] Calculate(float[] input)
        {
            if (_useBias && _layers.First().Size - 1 != input.Length)
                throw new Exception("input size does not match input layer size");

            if (!_useBias && input.Length != _layers.First().Size)
                throw new Exception("input size does not match input layer size");
            
            _layers[0].SetNeuronValues(input, _useBias);

            //Calculate neuron values for eacht layer except the first
            for (var i = 1; i < _layers.Count; i++)
            {
                var layer = _layers[i];
                var prevLayer = _layers[i - 1];

                foreach (var neuron in layer.Neurons)
                {
                    float activationInput = 0;
                    foreach (var prevLayerNeuron in prevLayer.Neurons)
                    {
                        activationInput += prevLayerNeuron.WeightedCurrenValue(neuron);
                    }

                    neuron.Value = Sigmoid(activationInput);
                }
            }

            return _layers.Last().Neurons.Select(x => x.Value).ToArray();
        }

        public float Test(float[][] inputData, float[][] expectedOutputData)
        {
            float accuracy;
            int good = 0;
            int bad = 0;

            for (int i = 0; i < inputData.Length; i++)
            {
                var output = Calculate(inputData[i]);
                output = output.Select(x => Mathf.Round(x)).ToArray();

                bool isGood = true;
                
                for (int j = 0; j < output.Length; j++)
                {
                    if (output[j] != expectedOutputData[i][j])
                        isGood = false;
                }

                if (isGood)
                    good++;
                else
                    bad++;
            }

            return ((float)good / inputData.Length) * 100f;
        }

        public void Learn(float[][] inputData, float[][] expectedOutputData, float learningrate)
        {
            for (int i = 0; i < inputData.Length; i++)
            {
                var input = inputData[i];
                var expectedOutput = expectedOutputData[i];

                Calculate(input);
                _error += Error(expectedOutput);
                _iterations++;
                if (_iterations % 100 == 0)
                {
//                    PrintError();
                    _error = 0;
                    _iterations = 0;
                }

                UpdateWeigths(expectedOutput, learningrate);
            }
        }

        public void PrintWeigths()
        {
            for (int i = 0; i < _layers.Count - 1; i++)
            {
                for (int j = 0; j < _layers[i].Size; j++)
                {
                    var neuronWeigths = string.Empty;
                    for (int k = 0; k < _layers[i+1].Size; k++)
                    {
                        neuronWeigths += _layers[i].Neurons[j].GetWeigth(_layers[i + 1].Neurons[k]) + " ";
                    }
                    Debug.Log(neuronWeigths);
                }
                Debug.Log("new layer");
            }
        }

        private float Error(float[] expectedOutput)
        {
            float sum = 0f;
            for (int i = 0; i < _layers.Last().Size; i++)
            {
                sum += Mathf.Abs(_layers[_layers.Count - 1].Neurons[i].Value - expectedOutput[i]);
            }

            return sum;
        }

        private void PrintError()
        {
            Debug.Log(_error / _iterations);
        }

        private void MakeBiasesFromEveryFirstNeuron()
        {
            for (int i = 0; i < _layers.Count - 1; i++)
            {
                var neuron = _layers[i].Neurons[0];
                neuron.IsBias = true;
            }
        }

        private void InitRandomWeights(float min, float max)
        {
            for (var i = 0; i < _layers.Count - 1; i++)
            {
                var layer = _layers[i];
                var nextLayer = _layers[i + 1];

                foreach (var fromNeuron in layer.Neurons)
                {
                    foreach (var toNeuron in nextLayer.Neurons)
                    {
                        fromNeuron.SetWeigth(toNeuron, Random.Range(min, max));
                    }
                }
            }
        }

        private void UpdateWeigths(float[] expectedValues, float learnRate)
        {
            float[][][] gradients = new float[_layers.Count - 1][][];
            float[][] Signals = new float[_layers.Count - 1][];
            
            //Compute signal values for output nodes
            Signals[Signals.Length - 1] = new float[_layers.Last().Size];
            for (int i = 0; i < _layers.Last().Size; i++)
            {
                Signals[Signals.Length - 1][i] = OutputSignal(_layers.Last().Neurons[i].Value, expectedValues[i]);
            }
            
            //Calculate signals for each neuron in hidden layer(s)
            for (int i = Signals.Length - 2; i >= 0; i--)
            {
                Signals[i] = new float[_layers[i + 1].Size];
                for (int j = 0; j < Signals[i].Length; j++)
                {
                    var neuron = _layers[i + 1].Neurons[j];
                    Signals[i][j] = HiddenSignal(neuron.Value, Signals[i + 1], neuron, i + 1);
                }
            }
            
            //Calculate gradients
            for (int i = gradients.Length - 1; i >= 0 ; i--)
            {
                gradients[i] = new float[_layers[i].Size][];
                for (int j = 0; j < gradients[i].Length; j++)
                {
                    gradients[i][j] = new float[_layers[i+1].Size];
                    var neuron = _layers[i].Neurons[j];
                    for (int k = 0; k < gradients[i][j].Length; k++)
                    {
                        gradients[i][j][k] = neuron.Value * Signals[i][k];
                    }
                }
            }
            
            //Update weigths
            for (int i = 0; i < _layers.Count - 1; i++)
            {
                var layer = _layers[i];
                for (int j = 0; j < _layers[i].Size; j++)
                {
                    var fromNeuron = layer.Neurons[j];
                    for (int k = 0; k < _layers[i+1].Size; k++)
                    {
                        var toNeuron = _layers[i + 1].Neurons[k];
                        var delta = learnRate * gradients[i][j][k];
                        var newWeigth = fromNeuron.GetWeigth(toNeuron) + delta;
                        fromNeuron.SetWeigth(toNeuron, newWeigth);
                    }
                }
            }
        }

        private float OutputSignal(float outputValue, float expectedValue)
        {
            return ((1 - outputValue) * (outputValue)) * (expectedValue - outputValue);
        }

        private float HiddenSignal(float outputValue, float[] signalsNextLayer, Neuron neuron, int layer)
        {
            var partial = (1 - outputValue) * outputValue;
            float sum = 0;

            for (var i = 0; i < signalsNextLayer.Length; i++)
            {
                sum += neuron.GetWeigth(_layers[layer + 1].Neurons[i]) * signalsNextLayer[i];
            }

            return partial * sum;
        }

        private float Sigmoid(float x)
        {
            return 1 / (1 + Mathf.Exp(-x));
        }

        private List<Layer> CreateNetwork(int[] topology)
        {
            var layers = new List<Layer>();

            for (var i = 0; i < topology.Length; i++)
            {
                layers.Add(new Layer(topology[i]));
            }

            return layers;
        }
    }
}