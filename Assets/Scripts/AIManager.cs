using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public int[] layerSizes = new int[] { 5, 5, 3, 2 };

    public int currentIteration = 0;
    public float maxTimePerEpisode = 50f;
    private float timeSpentInEpisode;

    public float weightAdjustmentMultiplier = 0.05f;
    public int numberOfAgents = 20;
    public int numberOfAgentsToKeepPerGeneration = 5;
    private Network[] agents;
    private GameObject[] cars;
    public GameObject carPrefab;
    private CarAIManager[] carAIs;
    public TrackGenerator trackGen;
    public int incrementBetweenTrainingUpdates = 5;
    private int currentFrame;
    public float crossOverProbability = 0.6f;
    public float mutationProbability = 0.6f;

    public int[] actionIndexes;


    void Start()
    {
        InitNetworks();
        InitCars();
        actionIndexes = new int[numberOfAgents];
    }

    public void InitNetworks() {
        agents = new Network[numberOfAgents];
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i] = new Network(layerSizes);
        }
    }

    public void InitCars()
    {
        cars = new GameObject[numberOfAgents];
        carAIs = new CarAIManager[numberOfAgents];

        for (int i = 0; i < numberOfAgents; i++)
        {
            cars[i] = Instantiate(carPrefab);
            cars[i].GetComponent<CarFitnessCalculator>().setTrackGenerator(trackGen);
            carAIs[i] = cars[i].GetComponent<CarAIManager>();

        }
    }
    private void Update() {
        timeSpentInEpisode += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (shouldUpdateOnThisFrame())
        {
            SetEnvironmentsValues();
            FindActionsForAllAgents();
            if (didEpisodeEnd())
            {
                NewEpisode();
            }
        }
    }

    private bool shouldUpdateOnThisFrame()
    {
        currentFrame++;
        if (currentFrame > incrementBetweenTrainingUpdates) currentFrame = 0;
        return currentFrame == incrementBetweenTrainingUpdates;
    }

    public void NewEpisode() {
        ModifyAllAgents();
        for (int i = 0; i < numberOfAgents; i++)
        {
            Destroy(cars[i]);
        }
        InitCars();
    }

    public void FindActionsForAllAgents()
    {
        for (int i = 0; i < numberOfAgents; i++)
        {
            if (!carAIs[i].WasWallHit())
            {
                carAIs[i].setCarInputs(agents[i].GetQValues());
            }
        }
    }

    public void SetEnvironmentsValues()
    {
        for (int i = 0; i < numberOfAgents; i++)
        {
            agents[i].setState(carAIs[i].getState());
        }
    }
    public void ModifyAllAgents()
    {
        int[] bestAgentIndexes = getBestAgentIndexes();
        MultiplyAgents(bestAgentIndexes);
        DoGeneticModifications(bestAgentIndexes);
    }

    public void DoGeneticModifications(int[] bestAgentIndexes)
    {
        Network[] agentsToKeepUnchanged = new Network[bestAgentIndexes.Length];
        for (int i = bestAgentIndexes.Length; i < agents.Length; i++)
        {
            if (shouldCrossOver())
            {
                int randomAgent = RandomIntInRange(numberOfAgentsToKeepPerGeneration, numberOfAgents);
                float[][][] crossedOverWeights = getCrossedOverWeights(agents[i].getWeights(), agents[randomAgent].getWeights());
                agents[i].setWeights(crossedOverWeights);
            }

            if (shouldMutate())
            {
                agents[i].RandomlyAdjustAllWeightsAndBiases(weightAdjustmentMultiplier);
            }
        }
    }

    public float[][][] getCrossedOverWeights(float[][][] weightsToCrossOver, float[][][] weightsToCrossOverWith)
    {
        for(int layerIndex = 0; layerIndex < weightsToCrossOver.GetLength(0); layerIndex++)
        {
            for (int neuronIndex = 0; neuronIndex < weightsToCrossOver.GetLength(0); neuronIndex++)
            {
                for (int weightIndex = 0; weightIndex < weightsToCrossOver.GetLength(0); weightIndex++)
                {
                    if (randomTrueOrFalse())
                    {
                        weightsToCrossOver[layerIndex][neuronIndex][weightIndex] = weightsToCrossOverWith[layerIndex][neuronIndex][weightIndex];
                    }
                }
            }
        }
        return weightsToCrossOver;
    }

    public int RandomIntInRange(int min, int max)
    {
        System.Random rand = new System.Random();
        return (int)(min + rand.NextDouble() * (max + 1 - min));
    }

    public bool randomTrueOrFalse()
    {
        System.Random rand = new System.Random();
        return rand.NextDouble() > 0.5;
    }

    public bool shouldCrossOver() {
        System.Random rand = new System.Random();
        return rand.NextDouble() < crossOverProbability;
    }

    public bool shouldMutate()
    {
        System.Random rand = new System.Random();
        return rand.NextDouble() < mutationProbability;
    }

    public void MultiplyAgents(int[] bestAgentIndexes)
    {
        Network[] bestAgents = new Network[bestAgentIndexes.Length];
        for (int i = 0; i < bestAgents.Length; i++)
        {
            bestAgents[i] = agents[bestAgentIndexes[i]];
        }
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i] = bestAgents[i % bestAgents.Length];
        }
    }

    public int[] getBestAgentIndexes()
    {
        int[] bestAgentIndexes = new int[numberOfAgentsToKeepPerGeneration];
        float[] agentScores = new float[numberOfAgents];
        for (int i = 0; i < agents.Length; i++)
        {
            agentScores[i] = carAIs[i].getFitness();
        }
        float[] sortedAgentScores = Sort.MergeSort(agentScores, 0, agentScores.Length - 1);
        float[] bestScores = new float[numberOfAgentsToKeepPerGeneration];
        for (int i = 0; i < numberOfAgentsToKeepPerGeneration; i++)
        {
            bestScores[i] = sortedAgentScores[sortedAgentScores.Length - 1 - i];
        }

        for (int i = 0; i < bestAgentIndexes.Length; i++)
        {
            for (int j = 0; j < agentScores.Length; j++)
            {
                if (bestScores[i] == agentScores[j])
                {
                    bestAgentIndexes[i] = j;
                    break;
                }
            }
        }
        return bestAgentIndexes;
    }


    private bool didEpisodeEnd()
    {
        if(timeSpentInEpisode > maxTimePerEpisode){
            timeSpentInEpisode = 0;
            return true;
        }
        int numberOfCarsThatHitWall = 0;
        foreach (CarAIManager carAI in carAIs)
        {
            if (carAI.WasWallHit()) numberOfCarsThatHitWall++;
        }
        return numberOfCarsThatHitWall == numberOfAgents;
    }

}
public static class Sort
{
    public static float[] MergeSort(float[] array, int start, int end)
    {
        if (start >= end) return new float[] { array[start] };

        int mid = (start + end) / 2;

        var leftSorted = MergeSort(array, start, mid);
        var rightSorted = MergeSort(array, mid + 1, end);

        return Merge(leftSorted, rightSorted);
    }

    public static float[] Merge(float[] left, float[] right)
    {
        float[] merged = new float[left.Length + right.Length];
        int leftIndex = 0, rightIndex = 0, mergeIndex = 0;

        while (leftIndex < left.Length && rightIndex < right.Length)
        {
            if (left[leftIndex] <= right[rightIndex]) merged[mergeIndex++] = left[leftIndex++];
            else merged[mergeIndex++] = right[rightIndex++];
        }

        while (leftIndex < left.Length) merged[mergeIndex++] = left[leftIndex++];

        while (rightIndex < right.Length) merged[mergeIndex++] = right[rightIndex++];

        return merged;
    }
}