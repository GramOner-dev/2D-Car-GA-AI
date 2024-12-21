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

    public void setCarInputs(float[] QValues)
    {
        controller.setInputs(doTurnLeft(QValues), doTurnRight(QValues), QValues[0]);
    }

    public bool doTurnLeft(float[] QValues) => QValues[1] > QValues[2] && QValues[1] > 0;
    public bool doTurnRight(float[] QValues) => QValues[2] > QValues[1] && QValues[2] > 0;

}
