using System;
using System.Collections.Generic;

namespace MachineLearning.NeuralNetwork
{
    [Serializable]
    public class Layer
    {
        public List<Neuron> Neurons;

        public int Size
        {
            get { return Neurons.Count; }
        }

        public Layer(int amountOfNeurons)
        {
            Neurons = CreateNeurons(amountOfNeurons);
        }

        public void SetNeuronValues(float[] values, bool useBias)
        {
            int offset = 0;
            if (useBias)
                offset = 1;

            for (var i = 0; i < values.Length; i++)
            {
                Neurons[i + offset].Value = values[i];
            }
        }

        private List<Neuron> CreateNeurons(int amountOfNeurons)
        {
            var neurons = new List<Neuron>();
            for (var i = 0; i < amountOfNeurons; i++)
            {
                neurons.Add(new Neuron());
            }

            return neurons;
        }
    }
}