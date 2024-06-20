using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LineGenerator3D : MonoBehaviour
{
    Transform[] points;
    float radius = 0.1f;
    int Segments = 10;
    int curveSegments = 50;
    
    public LineGenerator3D(Transform[] points)
    {
        this.points = points; 
    }

    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = CreateMesh();
    }

    Mesh CreateMesh()
    {
        if (points.Length < 2) return null;

        int vertexes = (Segments + 1) * (curveSegments + 1);
        int tris = Segments * curveSegments * 6;

        Vector3[] vertices = new Vector3[vertexes];
        Vector3[] normals = new Vector3[vertexes];
        Vector2[] uv = new Vector2[vertexes];
        int[] Tris = new int[tris];

        for (int i = 0; i <= curveSegments; i++)
        {
            float t = (float)i / curveSegments;
            
        }
        
        return null;
    }

    Vector3 GetPoint(float t)
    {
        int sections = points.Length - 1;
        int POI = Mathf.Min(Mathf.FloorToInt(t * sections), sections - 1);
        float u = t * sections - POI;

        Vector3 a = points[POI].position;
        Vector3 b = points[POI + 1].position;
        return Vector3.Lerp(a, b, u);
    }
}
