using UnityEngine;
public class LineRendererScaler : MonoBehaviour
{
    private LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        //Debug.Log(lineRenderer);
    }
    void Update()
    {
        UpdateLineRendererPositions();
    }

    void UpdateLineRendererPositions()
    {
        // Assuming the LineRenderer points are in local space of the helices object
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            positions[i] = transform.TransformPoint(lineRenderer.GetPosition(i)); // Transform to world space
        }
        lineRenderer.SetPositions(positions);
    }
}