using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct State
{
    private float normalizedCarSpeed;
    private float normalizedDistanceToNextCheckpoint;
    private float normalizedDistanceToWallForwards;
    private float normalizedDistanceToWallRight;
    private float normalizedDistanceToWallLeft;
    private float normalizedRotationRelativeToCheckpoint;
    public State(float carSpeed, float maxCarSpeed, float distanceFromLastCheckPointToNext, float distanceToNextCheckpoint, float rotationRelativeToNextCheckpoint, float carViewDistanceToWall, float distanceToWallForwards, float distanceToWallRight, float distanceToWallLeft)
    {
        normalizedCarSpeed = carSpeed / maxCarSpeed;
        normalizedDistanceToNextCheckpoint = 1-(distanceToNextCheckpoint / distanceFromLastCheckPointToNext);
        normalizedDistanceToWallForwards = distanceToWallForwards / carViewDistanceToWall;

        normalizedDistanceToWallRight = distanceToWallRight / carViewDistanceToWall;
        normalizedDistanceToWallLeft = distanceToWallLeft / carViewDistanceToWall;
        normalizedRotationRelativeToCheckpoint = rotationRelativeToNextCheckpoint / 360f;

    }

    public float[] createInputVector()
    {
        float[] inputVector = new float[] { normalizedCarSpeed, normalizedDistanceToNextCheckpoint, normalizedDistanceToWallForwards, normalizedDistanceToWallRight, normalizedDistanceToWallLeft, normalizedRotationRelativeToCheckpoint };
        // Debug.Log($"Input Vector: {string.Join(", ", inputVector)}");
        return inputVector;
    }
    public float NormalizedCarSpeed { get { return normalizedCarSpeed; } }
    public float NormalizedDistanceToNextCheckpoint { get { return normalizedDistanceToNextCheckpoint; } }
    public float NormalizedDistanceToWallForwards { get { return normalizedDistanceToWallForwards; } }
    public float NormalizedDistanceToWallRight { get { return normalizedDistanceToWallRight; } }
    public float NormalizedDistanceToWallLeft { get { return normalizedDistanceToWallLeft; } }
    public float NormalizedRotationRelativeToCheckpoint {get {return normalizedRotationRelativeToCheckpoint; }}

}
