using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    public int numberOfPoints = 100;
    public float trackLength = 50f;
    public float trackWidth = 2f;
    public float noiseScale = 0.1f;
    public float baseThicknessOfCentre, baseThicknessOfWalls;
    public LineRenderer trackRenderer;

    public LineRenderer outerBorderRenderer;
    public LineRenderer innerBorderRenderer;

    public EdgeCollider2D outerEdgeCollider;
    public EdgeCollider2D innerEdgeCollider;

    public List<Vector2> centralPath;
    private float totalTrackLength;

    private float noiseOffset;

    public float scaleFactor = 1f; 

    public GameObject pathPoint;
    public List<Vector2> getCentralPathPoints(){ 
        List<Vector2> scaledPoints = new List<Vector2>();
        for(int i = 0; i < centralPath.Count; i++){
            scaledPoints.Add(centralPath[i] * scaleFactor);
        }
        return scaledPoints;
    }

    void Start()
    {
        noiseOffset = Random.Range(0f, 1000f);
        GenerateTrack();
    }

    void GenerateTrack()
    {
        centralPath = GenerateCentralPath();
        foreach(Vector2 point in centralPath){
            Vector3 pos = new Vector3(point.x, point.y, 0);
            pos *= scaleFactor;
            Instantiate(pathPoint, pos, Quaternion.identity);
        }
        totalTrackLength = CalculateTotalTrackLength(centralPath);
        UpdateLineRendererThickness(); 
        RenderTrack(centralPath);
        CreateEdgeColliders(centralPath);
        RenderBorders(centralPath);
    }

    private float CalculateTotalTrackLength(List<Vector2> path)
    {
        float length = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            length += Vector2.Distance(path[i], path[i + 1]);
        }
        return length;
    }

    private List<Vector2> GenerateCentralPath()
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

            path.Add(currentPosition * scaleFactor); 
        }

        return path;
    }

    private void UpdateLineRendererThickness()
    {
        trackRenderer.startWidth = baseThicknessOfCentre * scaleFactor * trackWidth;
        trackRenderer.endWidth = baseThicknessOfCentre * scaleFactor * trackWidth;

        
        outerBorderRenderer.startWidth = baseThicknessOfWalls * scaleFactor * trackWidth;
        outerBorderRenderer.endWidth = baseThicknessOfWalls * scaleFactor * trackWidth;

        
        innerBorderRenderer.startWidth = baseThicknessOfWalls * scaleFactor * trackWidth;
        innerBorderRenderer.endWidth = baseThicknessOfWalls * scaleFactor * trackWidth;
    }

    private void RenderTrack(List<Vector2> path)
    {
        trackRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 scaledPosition = new Vector3(path[i].x, path[i].y, 0f) * scaleFactor; 
            trackRenderer.SetPosition(i, scaledPosition);
        }
    }

    private void CreateEdgeColliders(List<Vector2> centralPath)
    {
        List<Vector2> outerPath = new List<Vector2>();
        List<Vector2> innerPath = new List<Vector2>();

        for (int i = 0; i < centralPath.Count; i++)
        {
            Vector2 current = centralPath[i];
            Vector2 next = i < centralPath.Count - 1 ? centralPath[i + 1] : centralPath[0];
            Vector2 direction = (next - current).normalized;

            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            
            outerPath.Add((current + perpendicular * (trackWidth / 2f)) * scaleFactor);
            innerPath.Add((current - perpendicular * (trackWidth / 2f)) * scaleFactor);
        }

        outerEdgeCollider.points = outerPath.ToArray();
        innerEdgeCollider.points = innerPath.ToArray();
    }

    private void RenderBorders(List<Vector2> centralPath)
    {
        List<Vector3> outerPath = new List<Vector3>();
        List<Vector3> innerPath = new List<Vector3>();

        for (int i = 0; i < centralPath.Count; i++)
        {
            Vector2 current = centralPath[i];
            Vector2 next = i < centralPath.Count - 1 ? centralPath[i + 1] : centralPath[0];
            Vector2 direction = (next - current).normalized;

            Vector2 perpendicular = new Vector2(-direction.y, direction.x);

            
            outerPath.Add((current + perpendicular * (trackWidth / 2f)) * scaleFactor);
            innerPath.Add((current - perpendicular * (trackWidth / 2f)) * scaleFactor);
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
