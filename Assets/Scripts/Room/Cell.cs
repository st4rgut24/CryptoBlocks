using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A square area that is the smallest unit of a room
/// it should be no bigger than a single occupant (Person)
/// </summary>
public class Cell
{
    public Vector2 worldCenter;
    public Vector2 worldMinCorner;
    public Vector2 worldMaxCorner;

    public int row;
    public int col;

    Bounds bounds;

    List<Edge> EdgeList;

    public Cell(Vector2 bl, Vector2 br, Vector2 tl, Vector2 tr, int row, int col, Transform EdgeDaddy)
	{
        Vector2 center = (bl + tr) / 2;
        worldCenter = Camera.main.ScreenToWorldPoint(center);

        Vector2 worldBl = worldMinCorner = Camera.main.ScreenToWorldPoint(bl);
        Vector2 worldBr = Camera.main.ScreenToWorldPoint(br);
        worldMaxCorner = Camera.main.ScreenToWorldPoint(tr);

        float sideLength = Vector2.Distance(worldBl, worldBr);
        float pixelLength = Vector2.Distance(bl, br);

        Vector2 screenSize = new Vector2(pixelLength, pixelLength);

        bounds = new Bounds(center, screenSize);

        this.row = row;
        this.col = col;

        AddEdges(bl, br, tl, tr, EdgeDaddy);
	}

    private void AddEdges(Vector2 bl, Vector2 br, Vector2 tl, Vector2 tr, Transform EdgeDaddy)
    {
        EdgeList = new List<Edge>();

        EdgeList.Add(EdgeMaker.GetOrCreateEdge(bl, br, EdgeDaddy));
        EdgeList.Add(EdgeMaker.GetOrCreateEdge(bl, tl, EdgeDaddy));
        EdgeList.Add(EdgeMaker.GetOrCreateEdge(br, tr, EdgeDaddy));
        EdgeList.Add(EdgeMaker.GetOrCreateEdge(tl, tr, EdgeDaddy));
    }

    /// <summary>
    /// finds the edges belonging to cell that do not overlap with provided cells
    /// </summary>
    public List<Edge> GetNonOverlappingEdges(List<Cell> cells)
    {
        List<Edge> NoOverlapEdges = new List<Edge>(EdgeList);

        cells.ForEach((cell) =>
        {
            List<Edge> NeighborEdges = cell.EdgeList;

            NeighborEdges.ForEach((nEdge) =>
            {
                EdgeList.ForEach((myEdge) =>
                {
                    // if same edge remove from no overlap edge list
                    if (nEdge == myEdge && NoOverlapEdges.Contains(myEdge))
                    { 
                        NoOverlapEdges.Remove(myEdge);
                    }
                });
            });
        });

        return NoOverlapEdges;
    }

    public bool Contains(Vector2 point)
    {
        return bounds.Contains(point);
    }
}

