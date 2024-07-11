using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A group of cells organized in a rectangular area
/// </summary>
public class Box
{
    public enum Dir
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public Bounds bounds;

    public List<Cell> cells;

    public Vector2Int min;
    public Vector2Int max;

    public Box(List<Cell> cells, Vector2Int min, Vector2Int max)
	{
        this.cells = cells;
        this.min = min;
        this.max = max;

        SetBounds();
	}

    private void SetBounds()
    {
        Cell minCell = cells[0];
        Cell maxCell = cells[cells.Count - 1];

        Vector2 boxMidPoint = (minCell.worldMinCorner + maxCell.worldMaxCorner) / 2;
        Vector2 boxSize = new Vector2(maxCell.worldMaxCorner.x - minCell.worldMinCorner.x, maxCell.worldMaxCorner.y - minCell.worldMinCorner.y);

        bounds = new Bounds(boxMidPoint, boxSize);
    }

    public int getMidY()
    {
        return (min.y + max.y) / 2;
    }

    public int getMidX()
    {
        return (min.x + max.x) / 2;
    }

    public float getAspectRatio()
    {
        return (float)(this.max.x - this.min.x) / (float)(this.max.y - this.min.y);
    }
}

