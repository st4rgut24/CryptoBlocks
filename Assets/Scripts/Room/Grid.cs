using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Grid
{
    Dictionary<Vector2Int, Cell> grid;

    public Grid()
	{
        grid = new Dictionary<Vector2Int, Cell>();

    }

    public void Add(Cell cell)
    {
        grid[new Vector2Int(cell.col, cell.row)] = cell;
    }

    public void Remove(Cell cell)
    {
        grid.Remove(new Vector2Int(cell.col, cell.row));
    }

    public Cell Get(Vector2Int loc)
    {
        return grid[loc];
    }

    private bool CellExists(int col, int row)
    {
        Vector2Int loc = new Vector2Int(col, row);

        return grid.ContainsKey(loc);
    }

    /// <summary>
    /// Get all cells from the grid in the rectangular area
    /// provided the min corner and the max corner
    /// </summary>
    /// <returns></returns>
    public List<Cell> GetCells(Vector2Int min, Vector2Int max)
    {
        List<Cell> cells = new List<Cell>();

        for (int c = min.x; c < max.x; c++)
        {
            for (int r = min.y; r < max.y; r++)
            {
                //if (CellExists(c, r))
                //{
                    cells.Add(Get(new Vector2Int(c, r)));
                //}
            }
        }

        return cells;
    }

    /// <summary>
    /// Get bordering cells
    /// </summary>
    public List<Cell> GetNeighbors(Cell cell)
    {
        List<Vector2Int> neighborPos = new List<Vector2Int>()
        {
            new Vector2Int(cell.col + 1, cell.row),
            new Vector2Int(cell.col - 1, cell.row),
            new Vector2Int(cell.col, cell.row + 1),
            new Vector2Int(cell.col, cell.row - 1)
        };

        List<Vector2Int> cellNeighborPosList = neighborPos.FindAll((Vector2Int loc) => CellExists(loc[0], loc[1]));

        return cellNeighborPosList.Select(pos => Get(pos)).ToList();
    }
}

