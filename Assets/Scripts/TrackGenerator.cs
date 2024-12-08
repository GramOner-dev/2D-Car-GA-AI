using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    public int numberOfPoints = 100; 
    public float trackLength = 50f; 
    public float trackWidth = 2f; 
    public float noiseScale = 0.1f; 
    public LineRenderer trackRenderer; 

    public LineRenderer outerBorderRenderer; 
    public LineRenderer innerBorderRenderer; 

    public EdgeCollider2D outerEdgeCollider; 
    public EdgeCollider2D innerEdgeCollider; 

    private float noiseOffset; 

    void Start()
    {     
        noiseOffset = Random.Range(0f, 1000f);
        GenerateTrack();
    }

    void GenerateTrack()
    {
        List<Vector2> centralPath = GenerateCentralPath();
        RenderTrack(centralPath);
        CreateEdgeColliders(centralPath);
        RenderBorders(centralPath);
    }

    List<Vector2> GenerateCentralPath()
    {
        List<Vector2> path = new List<Vector2>();
        float angle = 0f;
        Vector2 currentPosition = Vector2.zero;

        for (int i = 0; i < numberOfPoints; i++)
        {
            
            float noiseValue = Mathf.PerlinNoise(noiseOffset + i * noiseScale, 0f);
            angle += Mathf.Lerp(-45f, 45f, noiseValue); 
            
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            currentPosition += direction * (trackLength / numberOfPoints);

            path.Add(currentPosition);
        }

        return path;
    }

    void RenderTrack(List<Vector2> path)
    {
        trackRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            trackRenderer.SetPosition(i, new Vector3(path[i].x, path[i].y, 0f));
        }
    }

    void CreateEdgeColliders(List<Vector2> centralPath)
    {
        List<Vector2> outerPath = new List<Vector2>();
        List<Vector2> innerPath = new List<Vector2>();

        for (int i = 0; i < centralPath.Count; i++)
        {
            Vector2 current = centralPath[i];
            Vector2 next = i < centralPath.Count - 1 ? centralPath[i + 1] : centralPath[0]; 
            Vector2 direction = (next - current).normalized;
            
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
     
            outerPath.Add(current + perpendicular * (trackWidth / 2f));
            innerPath.Add(current - perpendicular * (trackWidth / 2f));
        }

        
        outerEdgeCollider.points = outerPath.ToArray();
        innerEdgeCollider.points = innerPath.ToArray();
    }

    void RenderBorders(List<Vector2> centralPath)
    {
        List<Vector3> outerPath = new List<Vector3>();
        List<Vector3> innerPath = new List<Vector3>();

        for (int i = 0; i < centralPath.Count; i++)
        {
            Vector2 current = centralPath[i];
            Vector2 next = i < centralPath.Count - 1 ? centralPath[i + 1] : centralPath[0]; 
            Vector2 direction = (next - current).normalized;

            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            
            outerPath.Add(current + perpendicular * (trackWidth / 2f));
            innerPath.Add(current - perpendicular * (trackWidth / 2f));
        }

        outerBorderRenderer.positionCount = outerPath.Count;
        innerBorderRenderer.positionCount = innerPath.Count;

        for (int i = 0; i < outerPath.Count; i++)
        {
            outerBorderRenderer.SetPosition(i, outerPath[i]);
            innerBorderRenderer.SetPosition(i, innerPath[i]);
        }
    }
}
