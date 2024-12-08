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
    public GameObject carPrefab;
    public BoxCollider[] checkPoints;

    private GameObject[] cars;
    private WallManager[] wallManagers;
    // private CarController[] carControllers;
    private CheckPointManager[] checkpointManagers;
    public int incrementBetweenTrainingUpdates = 5;
    private int currentFrame;

    public int[] actionIndexes;

    public float normalizedCarSpeed;
    public float normalizedDistanceToNextCheckpoint;
    public float normalizedDistanceToWallForwards;
    public float normalizedDistanceToWallRight;
    public float normalizedDistanceToWallLeft;
    public float normalizedRotationRelativeToCheckpoint;
    public float score;


    void Start()
    {
        InitNetworks();
        InitCars();
        actionIndexes = new int[numberOfAgents];
    }
    private void Update() {
        timeSpentInEpisode += Time.deltaTime;
    }

    void FixedUpdate()
    {
        normalizedCarSpeed = agents[0].getState().NormalizedCarSpeed;
        normalizedDistanceToNextCheckpoint = agents[0].getState().NormalizedDistanceToNextCheckpoint;
        normalizedDistanceToWallForwards = agents[0].getState().NormalizedDistanceToWallForwards;
        normalizedDistanceToWallRight = agents[0].getState().NormalizedDistanceToWallRight;
        normalizedDistanceToWallLeft = agents[0].getState().NormalizedDistanceToWallLeft;
        normalizedRotationRelativeToCheckpoint = agents[0].getState().NormalizedRotationRelativeToCheckpoint;
        score = agents[0].getScore(checkpointManagers[0].getTotalCheckpoints());

        
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

    public void InitCars()
    {
        cars = new GameObject[numberOfAgents];
        wallManagers = new WallManager[numberOfAgents];
        carControllers = new CarController[numberOfAgents];
        checkpointManagers = new CheckPointManager[numberOfAgents];
        for(int i = 0; i < numberOfAgents; i++)
        {
            cars[i] = Instantiate(carPrefab);
            cars[i].GetComponent<CheckPointManager>().setCheckPoints(checkPoints);
            wallManagers[i] = cars[i].GetComponent<WallManager>();
            carControllers[i] = cars[i].GetComponent<CarController>();
            checkpointManagers[i] = cars[i].GetComponent<CheckPointManager>();

        }
    }

    public void InitNetworks() {
        agents = new Network[numberOfAgents];
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i] = new Network(layerSizes);
        }
    }

    public void NewEpisode(){
        ModifyAllAgents();
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
            carControllers[i].resetActionSpace();

            if (!wallManagers[i].WasWallHit())
            {
                actionIndexes[i] = agents[i].GetChosenActionIndex();
                carControllers[i].setActionSpace(actionIndexes[i]);
            }
            
        }
    }

    public void SetEnvironmentsValues()
    {
        for(int i = 0; i < numberOfAgents; i++)
        {
            agents[i].setValues(
                carControllers[i].getMaxCarSpeed(),
                carControllers[i].getCurrentCarSpeed(),
                checkpointManagers[i].getDistanceBetweenCheckPoints(),
                checkpointManagers[i].getCarsDistanceToNextCheckPoint(),
                checkpointManagers[i].getRotationRelativeToNextCheckpoint(),
                wallManagers[i].getCarViewDistanceToWall(),
                wallManagers[i].getDistanceToLeftWall(),
                wallManagers[i].getDistanceToRightWall(),
                wallManagers[i].getDistanceToForwardWall());
        }
    }
    public void ModifyAllAgents()
    {
        MultiplyAgents(getBestAgentIndexes());
        DoGeneticModifications();
    }

    public void DoGeneticModifications()
    { 
        for(int i = numberOfAgentsToKeepPerGeneration; i < agents.Length; i++)
        {
            agents[i].RandomlyAdjustAllWeightsAndBiases(weightAdjustmentMultiplier);
        }

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
            agents[i] = bestAgents[i % 5];
        }
    }

    public int[] getBestAgentIndexes()
    {
        int[] bestAgentIndexes = new int[numberOfAgentsToKeepPerGeneration];
        float[] agentScores = new float[numberOfAgents];
        for (int i = 0; i < agents.Length; i++)
        {
            agentScores[i] = agents[i].getScore(checkpointManagers[i].getTotalCheckpoints());
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
        foreach (WallManager wallManager in wallManagers)
        {
            if (wallManager.WasWallHit()) numberOfCarsThatHitWall++;
        }
        return numberOfCarsThatHitWall == numberOfAgents;
    }

}
