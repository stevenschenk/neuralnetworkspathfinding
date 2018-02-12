using MachineLearning.NeuralNetwork;
using UnityEngine;

namespace DefaultNamespace
{
    public class Or
    {
        private NeuralNetwork _neuralNetwork;

        public Or()
        {
            var input = new float[10000][];
            var output = new float[10000][];
            for (int i = 0; i < input.Length; i++)
            {
                input[i] = new float[2];
                output[i] = new float[1];

                var randomOne = Random.Range(0, 2);
                var randomTwo = Random.Range(0, 2);
                output[i][0] = randomOne | randomTwo;

                input[i][0] = randomOne;
                input[i][1] = randomTwo;

            }
            _neuralNetwork = new NeuralNetwork(new[] {3, 1});
            _neuralNetwork.Learn(input, output, 0.1f);
            _neuralNetwork.PrintWeigths();
            Debug.Log("1 | 1 = " + _neuralNetwork.Calculate(new float[]{1, 1})[0] + " expected = " + (1 | 1));
            Debug.Log("1 | 0 = " + _neuralNetwork.Calculate(new float[]{1, 0})[0] + " expected = " + (1 | 0));
            Debug.Log("0 | 1 = " + _neuralNetwork.Calculate(new float[]{0, 1})[0] + " expected = " + (0 | 1));
            Debug.Log("0 | 0 = " + _neuralNetwork.Calculate(new float[]{0, 0})[0] + " expected = " + (0 | 0));
        }
    }
}