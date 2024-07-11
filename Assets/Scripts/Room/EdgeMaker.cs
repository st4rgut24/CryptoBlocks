using System;
using System.Collections.Generic;
using UnityEngine;

public class EdgeMaker
{
    public static int id = 0;

    public static Dictionary<Vector2, Dictionary<Vector2, Edge>> EdgeDict =
        new Dictionary<Vector2, Dictionary<Vector2, Edge>>();

    public static Dictionary<EdgeCollider2D, List<Edge>> EdgeCDict =
        new Dictionary<EdgeCollider2D, List<Edge>>();

    private static void AddEdge(Vector2 start, Vector2 end, Edge edge)
    {
        if (!EdgeDict.ContainsKey(start))
        { 
            EdgeDict.Add(start, new Dictionary<Vector2, Edge>());
        }
        EdgeDict[start].Add(end, edge);
    }

    public static void AddEdgeCollider(EdgeCollider2D collider, List<Edge> edges)
    {
        if (!EdgeCDict.ContainsKey(collider))
            EdgeCDict.Add(collider, new List<Edge>());

        EdgeCDict[collider].AddRange(edges);

        edges.ForEach((edge) =>
        {
            edge.SetEdgeCollider(true);
        });
    }

    public static void RemoveEdgeCollider(EdgeCollider2D collider)
    {
        if (EdgeCDict.ContainsKey(collider))
        {
            List<Edge> edges = EdgeCDict[collider];

            edges.ForEach((edge) =>
            {
                edge.SetEdgeCollider(false);
            });

            EdgeCDict.Remove(collider);
        }
    }

    public static Edge GetOrCreateEdge(Vector2 start, Vector2 end, Transform EdgeDaddy)
    {
        Edge edge = null;

        if (EdgeDict.ContainsKey(start) && EdgeDict[start].ContainsKey(end))
        {
            edge = EdgeDict[start][end];
        }
        else if (EdgeDict.ContainsKey(end) && EdgeDict[end].ContainsKey(start))
        {
            edge = EdgeDict[end][start];
        }

        if (edge == null)
        {
            edge = new Edge(start, end, EdgeDaddy, id++);
            AddEdge(start, end, edge);
        }

        return edge;
    }
}

