using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct State
{
    private float[] normalizedViewDistances;
    private bool wasWallHit;
    public void setState(float[] distancesToWalls, float carViewDistance, bool wasWallHit)
    {
        normalizedViewDistances = new float[distancesToWalls.Length];
        for(int i = 0; i < distancesToWalls.Length; i++){
            normalizedViewDistances[i] = distancesToWalls[i] / carViewDistance;
        }

    }

    public float[] createInputVector()
    {
        return normalizedViewDistances;
    }
}
