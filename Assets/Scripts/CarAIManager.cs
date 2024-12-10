using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAIManager : MonoBehaviour
{
    public CarController controller;
    public CarViewManager carView;
    public WallManager wallManager;
    public CarFitnessCalculator carFitness;

    public State currentState;

    private void Update() {
        currentState.setState(carView.getDistanceToWalls(), carView.getCarViewDistanceToWalls(), wallManager.WasWallHit());
    }
    public State getState() => currentState;
    public bool WasWallHit() => wallManager.WasWallHit();
    public float getFitness() => carFitness.getFitness();


}

