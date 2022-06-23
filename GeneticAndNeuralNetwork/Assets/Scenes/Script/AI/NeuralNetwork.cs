using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class NeuralNetwork
    {
        int inputLayer;
        int hiddenLayer;
        int outputLayer;

        public double[,] weights1;
        public double[,] weights2;

        public NeuralNetwork(int input, int hidden, int output)
        {
            inputLayer = input;
            hiddenLayer = hidden;
            outputLayer = output;
            weights1 = this.generateRandomMatrix(input, hidden);
            weights2 = this.generateRandomMatrix(hidden, output);
        }

        double[,] generateRandomMatrix(int rows, int cols)
        {
            double[,] matrix = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = (Random.Range(0.0f, 1.0f) * 2.0f) - 1.0f;
                }
            }

            return matrix;
        }
    }
}