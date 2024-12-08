using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarViewManager : MonoBehaviour
{
    public string targetTag = "Wall";       
    public List<Transform> wallCheckers = new List<Transform>();      
    public float[] distancesToWalls;          
    public float raycastDistance = 20f;       
    public bool wasWallHit;                   
    private List<LineRenderer> lineRenderers = new List<LineRenderer>(); 

    private void Start() {
        distancesToWalls = new float[wallCheckers.Count];
        for(int i = 0; i < wallCheckers.Count; i++){
            lineRenderers.Add(wallCheckers[i].GetComponent<LineRenderer>());
        }
    }

    private void Update() {
        for (int i = 0; i < wallCheckers.Count; i++) {
            distancesToWalls[i] = CalculateDistance(wallCheckers[i]);
            UpdateLineRenderer(i);  
        }
    }

    public float CalculateDistance(Transform wallChecker) {
        Vector3 rayOrigin = wallChecker.position;
        Vector3 rayDirection = wallChecker.forward;
        float distance = raycastDistance;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastDistance);

        if (hit.collider != null && hit.collider.CompareTag(targetTag)) {
            distance = Vector3.Distance(rayOrigin, hit.point); 
        }

        return distance;
    }

    
    public void UpdateLineRenderer(int index) {
        Vector3 rayOrigin = wallCheckers[index].position;
        Vector3 rayDirection = wallCheckers[index].up;
   
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, raycastDistance);
        Vector3 targetPoint;

        if (hit.collider != null && hit.collider.CompareTag(targetTag)) {
            targetPoint = hit.point;  
        } else {
            targetPoint = rayOrigin + rayDirection * raycastDistance;  
        }

        LineRenderer lineRenderer = lineRenderers[index];
        lineRenderer.SetPosition(0, rayOrigin);  
        lineRenderer.SetPosition(1, targetPoint);  
        Debug.Log(rayOrigin);
        Debug.Log(targetPoint);

    }

    public float[] getDistanceToWalls() => distancesToWalls;

    public float getCarViewDistanceToWalls() => raycastDistance;
}