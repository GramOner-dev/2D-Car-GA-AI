using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFitnessCalculator : MonoBehaviour
{
    public float distanceAlongTrack;
    public List<Vector2> pathPoints;
    public TrackGenerator trackGen;

    private void Update() {
        pathPoints = trackGen.getCentralPathPoints();
        distanceAlongTrack = CalculateCarProgress();

    }

    public float CalculateCarProgress()
    {
        Vector2 carPosition = new Vector2(transform.position.x, transform.position.y);
        int closestPointIndex = 0;
        for(int i = 0; i < pathPoints.Count; i++){
            if((carPosition - pathPoints[closestPointIndex]).magnitude > (carPosition - pathPoints[i]).magnitude){
                closestPointIndex = i;
            }
        }
        return closestPointIndex; 
    }
    public void setTrackGenerator(TrackGenerator trackGen){
        this.trackGen = trackGen;
    }
    public float getFitness() => distanceAlongTrack;
}
