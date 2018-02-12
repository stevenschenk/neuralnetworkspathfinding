using System.Collections.Generic;
using UnityEngine;

namespace MachineLearning.NeuralNetwork
{
    public class Neuron
    {
        private Dictionary<Neuron, float> _weights = new Dictionary<Neuron, float>();
        private float _value;

        public bool IsBias;

        public float Value
        {
            get
            {
                if (IsBias)
                    return 1;
                
                return _value;
            }

            set { _value = value; }
        }

        public float WeightedCurrenValue(Neuron toNeuron)
        {
            return _weights[toNeuron] * Value;
        }

        public float GetWeigth(Neuron toNeuron)
        {
            return _weights[toNeuron];
        }

        public void SetWeigth(Neuron toNeuron, float weigth)
        {
            _weights[toNeuron] = weigth;
        }
    }
}