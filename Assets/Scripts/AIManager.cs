using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public int[] layerSizes = new int[] {5, 5, 3, 2};

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
    public int incrementBetweenTrainingUpdates = 5;
    private int currentFrame;

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
        
        for(int i = 0; i < numberOfAgents; i++)
        {
            cars[i] = Instantiate(carPrefab);
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

    public void NewEpisode(){
        // ModifyAllAgents();
        for(int i = 0; i < numberOfAgents; i++)
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
                
            } 
        }
    }

    public void SetEnvironmentsValues()
    {
        for(int i = 0; i < numberOfAgents; i++)
        {
            agents[i].setState(carAIs[i].getState());
        }
    }
    // public void ModifyAllAgents()
    // {
    //     MultiplyAgents(getBestAgentIndexes());
    //     DoGeneticModifications();
    // }

    // public void DoGeneticModifications()
    // { 
    //     for(int i = numberOfAgentsToKeepPerGeneration; i < agents.Length; i++)
    //     {
    //         agents[i].RandomlyAdjustAllWeightsAndBiases(weightAdjustmentMultiplier);
    //     }

    // }

    // public void MultiplyAgents(int[] bestAgentIndexes)
    // {
    //     Network[] bestAgents = new Network[bestAgentIndexes.Length];
    //     for (int i = 0; i < bestAgents.Length; i++)
    //     {
    //         bestAgents[i] = agents[bestAgentIndexes[i]];
    //     }
    //     for (int i = 0; i < agents.Length; i++)
    //     {
    //         agents[i] = bestAgents[i % 5];
    //     }
    // }

    // public int[] getBestAgentIndexes()
    // {
    //     int[] bestAgentIndexes = new int[numberOfAgentsToKeepPerGeneration];
    //     float[] agentScores = new float[numberOfAgents];
    //     for (int i = 0; i < agents.Length; i++)
    //     {
    //         agentScores[i] = agents[i].getScore(checkpointManagers[i].getTotalCheckpoints());
    //     }
    //     float[] sortedAgentScores = Sort.MergeSort(agentScores, 0, agentScores.Length - 1);
    //     float[] bestScores = new float[numberOfAgentsToKeepPerGeneration];
    //     for (int i = 0; i < numberOfAgentsToKeepPerGeneration; i++)
    //     {
    //         bestScores[i] = sortedAgentScores[sortedAgentScores.Length - 1 - i];
    //     }

    //     for (int i = 0; i < bestAgentIndexes.Length; i++)
    //     {
    //         for (int j = 0; j < agentScores.Length; j++)
    //         {
    //             if (bestScores[i] == agentScores[j])
    //             {
    //                 bestAgentIndexes[i] = j;
    //                 break;
    //             }
    //         }
    //     }
    //     return bestAgentIndexes;
    // }


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
