using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace AI
{
    public class Genetic
    {
        int inputLayer;
        int hiddenLayer;
        int outputLayer;
        int population;
        float elitism;
        float mutationRate;
        float mutationRange;
        public List<GeneticOpt> geneticOpts = new List<GeneticOpt>();
        GeneticOpt betterOfGeneticOpt;
        public int currentGeneticOpt = 0;
        public float totalScore = 0;
        public float maxScore = 0;

        public Genetic(int input, int hidden, int output, int populationSize, float elitismSize, float mutationRateSize, float mutationRangeSize)
        {
            inputLayer = input;
            hiddenLayer = hidden;
            outputLayer = output;
            population = populationSize;
            elitism = elitismSize;
            mutationRate = mutationRateSize;
            mutationRange = mutationRangeSize;
        }

        public void PopulateGeneticOpts()
        {
            if (geneticOpts.Count == 0)
            {
                FirstGeneticOpt();
            }
            else
            {
                NextGeneticOpt();
            }

            currentGeneticOpt++;
        }

        void FirstGeneticOpt()
        {
            for (int i = 0; i < population; i++)
            {
                geneticOpts.Add(new GeneticOpt(inputLayer, hiddenLayer, outputLayer, currentGeneticOpt));
            }

            betterOfGeneticOpt = geneticOpts[0];
        }

        void OrderGeneticOpt()
        {
            geneticOpts = geneticOpts.OrderByDescending(g => g.score).ToList();
            betterOfGeneticOpt = geneticOpts[0];
        }

        void SetTotalScore()
        {
            totalScore = 0;

            for (int i = 0; i < geneticOpts.Count; i++)
            {
                if (geneticOpts[i].score > maxScore)
                {
                    maxScore = geneticOpts[i].score;
                }

                totalScore += geneticOpts[i].score;
            }
        }

        void NextGeneticOpt()
        {
            OrderGeneticOpt();
            SetTotalScore();

            List<GeneticOpt> nextGeneticOpt = new List<GeneticOpt>();

            for (int i = 0; i < Mathf.RoundToInt(elitism * population); i++)
            {
                if (nextGeneticOpt.Count < population)
                {
                    nextGeneticOpt.Add(geneticOpts[i]);
                }
            }

            int diff = (geneticOpts.Count - nextGeneticOpt.Count);

            for (int i = 0; i < diff; i += 2)
            {
                int parent1 = getParentIndex();
                int parent2 = getParentIndex();

                List<GeneticOpt> children = CrossOver(geneticOpts[parent1], geneticOpts[parent2]);

                nextGeneticOpt.Add(children[0]);
                nextGeneticOpt.Add(children[1]);
            }

            geneticOpts = nextGeneticOpt;
            betterOfGeneticOpt = geneticOpts[0];
        }

        List<GeneticOpt> CrossOver(GeneticOpt parent1, GeneticOpt parent2)
        {
            List<GeneticOpt> children = new List<GeneticOpt>();

            children.Add(new GeneticOpt(inputLayer, hiddenLayer, outputLayer, currentGeneticOpt));
            children.Add(new GeneticOpt(inputLayer, hiddenLayer, outputLayer, currentGeneticOpt));

            for (int i = 0; i < inputLayer; i++)
            {
                for (int j = 0; j < hiddenLayer; j++)
                {
                    if (Random.Range(0.0f, 1.0f) <= 0.5)
                    {
                        double weights = children[0].neuralNetwork.weights1[i, j];
                        children[0].neuralNetwork.weights1[i, j] = children[1].neuralNetwork.weights1[i, j];
                        children[1].neuralNetwork.weights1[i, j] = weights;
                    }
                }
            }

            for (int i = 0; i < hiddenLayer; i++)
            {
                for (int j = 0; j < outputLayer; j++)
                {
                    if (Random.Range(0.0f, 1.0f) <= 0.5)
                    {
                        double weights = children[0].neuralNetwork.weights2[i, j];
                        children[0].neuralNetwork.weights2[i, j] = children[1].neuralNetwork.weights2[i, j];
                        children[1].neuralNetwork.weights2[i, j] = weights;
                    }
                }
            }

            for (int i = 0; i < inputLayer; i++)
            {
                for (int j = 0; j < hiddenLayer; j++)
                {        
                    if (Random.Range(0.0f, 1.0f) <= mutationRate)
                    {
                        children[0].neuralNetwork.weights1[i, j] = Random.Range(0.0f, 1.0f) * mutationRange * 2 - mutationRange;
                    }

                    if (Random.Range(0.0f, 1.0f) <= mutationRate)
                    {
                        children[1].neuralNetwork.weights1[i, j] = Random.Range(0.0f, 1.0f) * mutationRange * 2 - mutationRange;
                    }
                }
            }

            for (int i = 0; i < hiddenLayer; i++)
            {
                for (int j = 0; j < outputLayer; j++)
                {
                    if (Random.Range(0.0f, 1.0f) <= mutationRate)
                    {
                        children[0].neuralNetwork.weights2[i, j] = Random.Range(0.0f, 1.0f) * mutationRange * 2 - mutationRange;
                    }

                    if (Random.Range(0.0f, 1.0f) <= mutationRate)
                    {
                        children[1].neuralNetwork.weights2[i, j] = Random.Range(0.0f, 1.0f) * mutationRange * 2 - mutationRange;
                    }
                }
            }

            return children;
        }

        int getParentIndex()
        {
            int index = -1;

            float randomScoreSorted = Random.Range(0.0f, 1.0f) * totalScore;
            float scoreSum = 0;

            int i = 0;
            while (i < geneticOpts.Count && scoreSum < randomScoreSorted)
            {
                scoreSum += geneticOpts[i].score;
                index++;
                i++;
            }

            return index;
        }
    }
}