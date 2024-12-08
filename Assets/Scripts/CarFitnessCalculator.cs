using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarFitnessCalculator : MonoBehaviour
{
    public float distanceAlongTrack;
    public TrackGenerator track;
    private void Update() {
        distanceAlongTrack = track.CalculateCarProgress(transform.position);
    }
}
