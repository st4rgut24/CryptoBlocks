using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

/**
 * Listens for collisions and such
 */
public class RoomPrefab: MonoBehaviour
{
    public bool RoomLocked = false;

    public List<RoomPrefab> AdjacentRooms;
    public List<RoomPrefab> AdjacentConnectedRooms;

    public int roomId;

    private bool visibility;

    List<EdgeCollider2D> edgeCollider2Ds;

    public Box box;

    Grid grid;

    public List<Edge> Perimeter;

    private void Awake()
    {
        visibility = true;
        grid = new Grid();
        edgeCollider2Ds = new List<EdgeCollider2D>();

        AdjacentConnectedRooms = new List<RoomPrefab>();
        AdjacentRooms = new List<RoomPrefab>();
    }

    /// <summary>
    /// Find all shared edges with another room
    /// </summary>
    public List<Edge> GetSharedEdges(RoomPrefab otherRoom)
    {
        List<Edge> sharedEdges = new List<Edge>();
        List<Edge> otherEdges = otherRoom.Perimeter;

        Perimeter.ForEach((edge) =>
        {
            Edge otherEdge = otherEdges.Find((otherEdge) => otherEdge.Equals(edge));

            if (otherEdge != null)
            {
                sharedEdges.Add(edge);
            }
        });

        return sharedEdges;
    }

    public List<Cell> GetPerimeterCells()
    {
        HashSet<Vector2Int> uniqueCellLocs = new HashSet<Vector2Int>();
        List<Cell> cells = new List<Cell>();

        for (int c=box.min.x; c < box.max.x; c++)
        {
            Vector2Int bottomRowCell = new Vector2Int(c, box.min.y);
            AddUniqueCell(bottomRowCell, uniqueCellLocs, cells);

            Vector2Int topRowCell = new Vector2Int(c, box.max.y - 1);
            AddUniqueCell(topRowCell, uniqueCellLocs, cells);
        }

        for (int r=box.min.y; r < box.max.y; r++)
        {
            Vector2Int leftColumnCell = new Vector2Int(box.min.x, r);
            AddUniqueCell(leftColumnCell, uniqueCellLocs, cells);

            Vector2Int rightColumnCell = new Vector2Int(box.max.x - 1, r);
            AddUniqueCell(rightColumnCell, uniqueCellLocs, cells);
        }

        return cells;
    }

    private void AddUniqueCell(Vector2Int cellLoc, HashSet<Vector2Int> uniqueCells, List<Cell> cellList)
    {
        if (!uniqueCells.Contains(cellLoc)) {
            cellList.Add(grid.Get(cellLoc));

            uniqueCells.Add(cellLoc);
        }
    }

    public List<Cell> GetCells()
    {
        //List<Cell> cells = new List<Cell>();

        //cells.AddRange(box.cells);

        //return cells;

        return box.cells;
    }

    /// <summary>
    /// room is complete if none of its edges are hidden
    /// </summary>
    public bool IsRoomCompleted()
    {
        return Perimeter.Find((edge) => !edge.IsVisible()) == null;
    }

    public void ClearConnectedRoom()
    {
        AdjacentConnectedRooms.Clear();
    }

    public void RemoveConnectedRoom(RoomPrefab room)
    {
        AdjacentConnectedRooms.Remove(room);
    }

    public bool IsAdjacentConnectedRoom(RoomPrefab room)
    {
        return AdjacentRooms.Contains(room) && AdjacentConnectedRooms.Contains(room);
    }

    public bool IsAdjacentWalledRoom(RoomPrefab room)
    {
        return AdjacentRooms.Contains(room) && !AdjacentConnectedRooms.Contains(room);
    }

    public void AddAdjacentRoom(RoomPrefab room)
    {
        AdjacentRooms.Add(room);
    }

    public void AddConnectedRoom(RoomPrefab room)
    {
        if (!AdjacentRooms.Contains(room))
        {
            AddAdjacentRoom(room);
        }
        AdjacentConnectedRooms.Add(room);
    }

    public bool BoundsContains(Vector2 coord)
    {
        return this.box.bounds.Contains(coord);
    }

    public Cell Contains(Vector2 coord)
    {
        List<Cell> cells = GetCells();

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].Contains(coord))
            {
                return cells[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Create a perimter involves adding new edges to complete a room or divide a room
    /// </summary>
    public void CreatePerimeter(bool ShouldShowDivider)
    {
        //HidePerimeter(); // hide the old perimeter

        // create the new perimeter
        Perimeter = GetPerimeter();

        // add colliders for new walls (edges that were previously hidden)

        // TODO:
        // 1. Get newly visible edges


        // 2. order these edges
        // 3. animate these edges (both the collider and renderer)


         ShowPerimeter(ShouldShowDivider); // show the new perimeter
        //TestDisplayPerimeter();

        // TODO: UpdateColliders to use NewVisibleEdges list
        //ReplaceColliders();
    }

    public void TestDisplayPerimeter()
    {
        if (Perimeter == null)
        {
            return;
        }

        Perimeter.ForEach((edge) =>
        {
            edge.ToggleLine(true);
        });

        visibility = true;
    }

    // animate the perimeter also
    public void ShowPerimeter(bool ShowDivider)
    {
        List<Edge> NewVisibleEdges = new List<Edge>();
        List<Edge> OldVisibleEdges = new List<Edge>();

        Perimeter.ForEach((edge) =>
        {
            //Debug.Log("is edge with id " + edge.id + " turned on? " + edge.IsVisible());
            if (!edge.IsVisible())
                NewVisibleEdges.Add(edge);
            else if (!edge.HasCollider)
            {
                OldVisibleEdges.Add(edge);
            }
        });
        
        // TODO:
        // 1. Create an edge collider
        // 2. pass in the edge collider

        if (NewVisibleEdges.Count > 0 && ShowDivider)
        {
            List<List<Edge>> LineList = GetLinesFromEdges(NewVisibleEdges);

            LineList.ForEach((line) =>
            {
                // create edge collider but don't add points until animation phase
                EdgeCollider2D edgeC = gameObject.AddComponent<EdgeCollider2D>(); 
                LineAnimator.Instance.AnimateEdges(line, edgeC);

                edgeCollider2Ds.Add(edgeC);
            });
        }
        if (OldVisibleEdges.Count > 0)
        {
            // create edge collider immediately (because no animation is required)
            List<EdgeCollider2D> colliders = CreateEdgeCollidersFromEdges(OldVisibleEdges);

            colliders.ForEach((edgeC) =>
            {
                edgeC.isTrigger = true;
                edgeC.enabled = visibility;
                edgeCollider2Ds.Add(edgeC);
            });
        }
        visibility = true;
    }

    /// <summary>
    /// only used for first created room, useful for when we want to create a room without
    /// animations
    /// </summary>
    public void InitRoomWithoutAnimating()
    {
        Perimeter = GetPerimeter();
        List<Edge> NewVisibleEdges = new List<Edge>();

        Perimeter.ForEach((edge) =>
        {
            //Debug.Log("is edge with id " + edge.id + " turned on? " + edge.IsVisible());
            if (!edge.IsVisible())
            {
                NewVisibleEdges.Add(edge);
                edge.ToggleLine(true);
            }
        });

        List<EdgeCollider2D> colliders = CreateEdgeCollidersFromEdges(NewVisibleEdges);

        colliders.ForEach((edgeC) =>
        {
            edgeC.isTrigger = true;
            edgeC.enabled = visibility;
            edgeCollider2Ds.Add(edgeC);
        });

        visibility = true;
    }

    public void HidePerimeter()
    {
        if (Perimeter == null)
        {
            return;
        }

        Perimeter.ForEach((edge) =>
        {
            edge.ToggleLine(false);
        });

        visibility = false;
    }

    /// <summary>
    /// updates the collider along active edges
    /// </summary>
    public void ReplaceColliders()
    {
        RemoveEdgeCollider();

        edgeCollider2Ds = ResetEdgeColliders();

        edgeCollider2Ds.ForEach((edgeC) =>
        {
            edgeC.isTrigger = true;
            edgeC.enabled = visibility;
        });
    }

    public List<List<Edge>> GetLinesFromEdges(List<Edge> edges)
    {
        List<List<Edge>> lines = new List<List<Edge>>();
        while (edges.Count > 0)
        {
            List<Edge> OrderedEdges = GetOrderedEdges(edges);
            lines.Add(OrderedEdges);
        }

        return lines;
    }

    public void RemoveEdgeCollider()
    {
        edgeCollider2Ds.ForEach((edgeC) =>
        {
            GameObject.Destroy(edgeC);
            EdgeMaker.RemoveEdgeCollider(edgeC);
        });
    }

    private EdgeCollider2D CreateEdgeCollider(List<Edge> copiedEdges)
    {
        List<Edge> orderedEdges = GetOrderedEdges(copiedEdges);

        EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        EdgeMaker.AddEdgeCollider(edgeCollider, orderedEdges); 

        Vector2[] edgeColliderPoints = new Vector2[orderedEdges.Count + 1];

        edgeColliderPoints[0] = Camera.main.ScreenToWorldPoint(orderedEdges[0].start);
        edgeColliderPoints[1] = Camera.main.ScreenToWorldPoint(orderedEdges[0].end);

        for (int i = 1; i < orderedEdges.Count; i++)
        {
            Edge edge = orderedEdges[i];

            if (edge.start.x == edgeColliderPoints[i].x &&
                edge.start.y == edgeColliderPoints[i].y)
            {
                edgeColliderPoints[i + 1] = Camera.main.ScreenToWorldPoint(edge.end);
            }
            else
            {
                edgeColliderPoints[i + 1] = Camera.main.ScreenToWorldPoint(edge.start);
            }
        }

        edgeCollider.points = edgeColliderPoints;

        return edgeCollider;
    }

    private List<EdgeCollider2D> ResetEdgeColliders()
    {
        List<Edge> copiedEdges = GetEdgesWithoutColliders();

        return CreateEdgeCollidersFromEdges(copiedEdges);
    }

    private List<EdgeCollider2D> CreateEdgeCollidersFromEdges(List<Edge> copiedEdges)
    {
        List<EdgeCollider2D> edgeCollider2Ds = new List<EdgeCollider2D>();

        while (copiedEdges.Count > 0)
        {
            EdgeCollider2D edgeC = CreateEdgeCollider(copiedEdges);
            edgeCollider2Ds.Add(edgeC);
        }

        return edgeCollider2Ds;
    }

    private List<Edge> GetActiveEdges()
    {
        List<Edge> ActiveEdges = new List<Edge>();

        Perimeter.ForEach((edge) =>
        {
            if (edge.IsVisible())
            {
                ActiveEdges.Add(edge);
            }
        });

        return ActiveEdges;
    }

    private List<Edge> GetEdgesWithoutColliders()
    {
        List<Edge> CollidableEdges = new List<Edge>();

        Perimeter.ForEach((edge) =>
        {
            if (edge.IsVisible() && !edge.HasCollider)
                CollidableEdges.Add(edge);
        });

        return CollidableEdges;
    }

    private List<Edge> GetOrderedEdges(List<Edge> copiedEdges)
    {
        List<Edge> orderedEdges = new List<Edge>();
        if (copiedEdges.Count < 2)
        {
            return copiedEdges;
        }

        Edge edge = copiedEdges[0];

        bool prepend = false;

        Vector2 vertex = edge.start;
        Vector2 otherVertex = edge.end;

        while (edge != null)
        {
            copiedEdges.Remove(edge);

            if (prepend)
                orderedEdges.Insert(0, edge);
            else
                orderedEdges.Add(edge);

            if (copiedEdges.Count == 0)
                break;

            edge = copiedEdges.Find((edge) => edge.start == vertex || edge.end == vertex);

            if (edge == null)
            {
                // add edges connected to the other end of the initial edge
                vertex = otherVertex;
                edge = copiedEdges.Find((edge) => edge.start == vertex || edge.end == vertex);
                prepend = true;
            }

            if (edge != null)
                vertex = edge.start == vertex ? edge.end : edge.start;
        }

        return orderedEdges;
    }

    public List<Edge> GetPerimeter()
    {
        // draw the perimter of the room
        List<Edge> PerimeterEdgeList = new List<Edge>();
        //List<Cell> cells = GetCells(); // filter the cells to only include those on the perimeter
        List<Cell> cells = GetPerimeterCells();


        cells.ForEach((cell) =>
        {
            // get neighboring cells that belong to the same room
            List<Cell> neighborCells = grid.GetNeighbors(cell);
            // Populate the list of non overlapping edges to get the room's perimeter
            PerimeterEdgeList.AddRange(cell.GetNonOverlappingEdges(neighborCells));
        });

        return PerimeterEdgeList;
    }

    /// <summary>
    /// Draw the bounds of the room
    /// </summary>
    /// <param name="anchoredPosition">bottom-left corner of rectangle</param>
    /// <param name="size">size of room</param>
    public virtual void AnnexSpace(Box box)
    {
        this.box = box;

        box.cells.ForEach((cell) =>
        {
            grid.Add(cell);
        });
    }

    public void RemoveSpace(Box box)
    {
        box.cells.ForEach((cell) =>
        {
            grid.Remove(cell);
        });
    }
}

