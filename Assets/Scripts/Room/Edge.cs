using System;
using UnityEngine;

public class Edge
{
	public Vector2 start;
	public Vector2 end;

    public Vector2 worldStart;
    public Vector2 worldEnd;

    public bool IsAnimating = false;

    public int id;
    public bool HasCollider = false;

    public LineRenderer lineRenderer;

	public Edge(Vector2 start, Vector2 end, Transform EdgeDaddy, int id)
	{
        this.id = id;
        //Debug.Log("Create edge with id " + id.ToString());

		this.start = new Vector3(start.x, start.y, 0);
        this.end = new Vector3(end.x, end.y, 0);

        worldStart = Camera.main.ScreenToWorldPoint(start);
        worldEnd = Camera.main.ScreenToWorldPoint(end);

        lineRenderer = CreateLineUI(this.start, this.end, EdgeDaddy);
        ToggleLine(false); // default to hidden edge
	}

    public LineRenderer CreateLineUI(Vector3 start, Vector3 end, Transform EdgeParent)
    {
        // Create a new GameObject with a LineRenderer component
        GameObject lineGameObject = new GameObject("ScreenSpaceLine " + id.ToString());
        lineGameObject.transform.parent = EdgeParent;

        LineRenderer lineRenderer = lineGameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Convert screen space coordinates to world space
        Vector3 worldPoint1 = Camera.main.ScreenToWorldPoint(start);
        Vector3 worldPoint2 = Camera.main.ScreenToWorldPoint(end);

        //float lineWidth = Vector3.Distance(worldPoint1, worldPoint2) / 4;
        float lineWidth = Vector3.Distance(worldPoint1, worldPoint2) / 2;

        lineRenderer.startWidth = lineRenderer.endWidth = lineWidth;

        // adjust to allow lines to overlap at the corners
        Vector3 adjWorldPoint1 = worldPoint1 + (worldPoint1 - worldPoint2).normalized * lineWidth / 2;
        Vector3 adjWorldPoint2 = worldPoint2 + (worldPoint2 - worldPoint1).normalized * lineWidth / 2;

        // Set positions for the LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, new Vector3(adjWorldPoint1.x, adjWorldPoint1.y, 0));
        lineRenderer.SetPosition(1, new Vector3(adjWorldPoint2.x, adjWorldPoint2.y, 0));


        return lineRenderer;
    }

    public void SetEdgeCollider(bool isActive)
    {
        HasCollider = isActive;
    }

    public bool IsVisible()
    {
        return lineRenderer.enabled;
    }

    public void ToggleLine(bool visible)
    {
        lineRenderer.enabled = visible;
    }

    public float GetClosestDistance(Vector2 point)
    {
        // line segments are short enough we can approximate closest distance by measuring distance to ends
        Vector2 startWorld = Camera.main.ScreenToWorldPoint(start);
        Vector2 endWorld = Camera.main.ScreenToWorldPoint(end);

        return Math.Min(Vector2.Distance(point, startWorld), Vector2.Distance(point, endWorld));
    }
}