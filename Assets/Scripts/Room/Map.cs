using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The map defining the layout of the spaces
/// if you want to define a type of space (eg room),
/// you must reference the cells in the map
/// </summary>
public class Map : Singleton<Map>
{
    [SerializeField]
    private Transform RoomDaddy;

    [SerializeField]
    private Transform EdgeDaddy;

    [SerializeField]
    private GameObject RoomGo;

    [SerializeField]
    private GameObject ResearchRoomGo;

    [SerializeField]
	private int horCells = 100; // cell count horizontally

    List<RoomPrefab> Rooms;

    Grid grid;

    bool IsFirstRoomCreated = false;

	int vertCells; // cell count vertically

    int roomId = 0;

    float cellLength; // cell length in pixels (cell height == cell width)

    private void OnEnable()
    {
        Controller.TouchEvent += OnSingleTouch;
        Controller.DragEvent += OnDrag;
        PersonPrefab.DieEvent += OnDie;
    }

    private void Awake()
    {
        grid = new Grid();
        Rooms = new List<RoomPrefab>();
    }

    // Use this for initialization
    void Start()
	{
        Vector2 bottomLeft = Vector2.zero;
        Vector2 topRight = new Vector2(Screen.width, Screen.height);

		vertCells = (int)(horCells / Camera.main.aspect);
        cellLength = (float)Screen.width / horCells;

        CreateMap();

        RoomPrefab room = CreateRoom(Vector2Int.zero, new Vector2Int(horCells, vertCells), true);
    }

    public void OnDie(GameObject deadPerson, HealthState healthState)
    {
        if (healthState == HealthState.Sick)
        {
            Vector2 deathPos = deadPerson.transform.position;

            BurstThroughWalls(deathPos, Consts.BurstWallRadius);
        }
    }

    public void BurstThroughWalls(Vector2 blastPos, float burstRadius)
    {
        // destroy the wall(s) (if any) closest to the death location
        RoomPrefab room = FindRoomWorldCoords(blastPos);

        Rooms.ForEach((adjRoom) =>
        {
            if (adjRoom == null || room == adjRoom ||  room.IsAdjacentConnectedRoom(adjRoom))
                // dont need to break down walls if these rooms are already connected
                return;

            List<Edge> sharedEdges = room.GetSharedEdges(adjRoom);

            for (int s = 0; s < sharedEdges.Count; s++)
            {
                Edge edge = sharedEdges[s];

                if (edge.IsAnimating) // a wall under construction cannot be suspended,
                    // otherwise get weird artifacs resulting from edge visibility being indeterminate
                    break;

                if (edge.GetClosestDistance(blastPos) < burstRadius)
                {
                    JoinRoom(room, adjRoom);
                    break;
                }
            }
        });
    }

    /// <summary>
    /// when a single click is detected either divide a room into two OR
    /// restore a room's missing walls
    /// </summary>
    private void OnSingleTouch(Vector3 touchPos)
    {
        // dont create new rooms/walls until line animator is finished animating
        if (LineAnimator.Instance.IsAnimating)
            return;

        RoomPrefab room = FindRoom(Input.mousePosition);

        if (room.IsRoomCompleted())
        {
            DivideRoom(room);
        }
        else
        {
            FillRoom(room);

            room.AdjacentConnectedRooms.ForEach((adjRoom) =>
            {
                adjRoom.RemoveConnectedRoom(room);
            });

            room.ClearConnectedRoom();
        }
    }

    private void OnDrag(Vector3 dragStart, Vector3 dragEnd)
    {
        RoomPrefab room1 = FindRoom(dragStart);
        RoomPrefab room2 = FindRoom(dragEnd);

        if (room1.gameObject == room2.gameObject)
        {
            return;
        }

        JoinRoom(room1, room2);
    }

    public RoomPrefab FindRoomWorldCoords(Vector3 worldCoords)
    {
        Vector3 screenCoords = Camera.main.WorldToScreenPoint(worldCoords);
        return Rooms.Find((room) => room.Contains(screenCoords) != null);
    }

    private RoomPrefab FindRoom(Vector3 coord)
    {
        return Rooms.Find((room) => room.Contains(coord) != null);
    }

    public RoomPrefab CreateRoom(Vector2Int minCoord, Vector2Int maxCoord, bool ShowPerimeter)
    {
        List<Cell> cells = grid.GetCells(minCoord, maxCoord);
        Box box = new Box(cells, minCoord, maxCoord);

        GameObject RoomObject = Instantiate(RoomGo);

        RoomObject.transform.parent = RoomDaddy;

        roomId++;

        RoomPrefab room = RoomObject.GetComponent<RoomPrefab>();        

        room.roomId = roomId;
        RoomObject.name = "Room " + room.roomId.ToString();

        room.AnnexSpace(box);

        Rooms.Add(room);


        if (!PersonPlotter.Instance.IsPeopleInBounds(box.bounds))
        {
            GameObject ResearchPrefab = Instantiate(ResearchRoomGo);
            ResearchPrefab.transform.SetParent(RoomObject.transform);

            ResearchPrefab.GetComponent<ResearchCenterPrefab>().InitSpace(room.box);
        }

        if (!IsFirstRoomCreated)
        {
            room.InitRoomWithoutAnimating();
            IsFirstRoomCreated = true;
        }
        else {
            room.CreatePerimeter(ShowPerimeter);
        }

        return room;
    }

    /// <summary>
    /// Join adjacent rooms by hiding the walls between them
    /// </summary>
    public void JoinRoom(RoomPrefab room1, RoomPrefab room2)
    {
        if (room1.AdjacentConnectedRooms.Contains(room2))
        {
            // exit if these rooms are already joined
            return;
        }

        List<Edge> roomBorders = room1.GetSharedEdges(room2);

        if (roomBorders.Count == 0) // not adjacent rooms
        {
            return;
        }

        roomBorders.ForEach((borderLine) =>
        {
            borderLine.ToggleLine(false);
        });

        room1.ReplaceColliders();
        room2.ReplaceColliders();

        room1.AddConnectedRoom(room2);
        room2.AddConnectedRoom(room1);
    }

    /// <summary>
    /// Completes a room that has a missing wall(s)
    /// </summary>
    private void FillRoom(RoomPrefab room)
    {
        room.CreatePerimeter(true);
    }

    /// <summary>
    /// creates new rooms on either side of a divider and
    /// removes the room that is divided
    /// </summary>
    /// <param name="plotCoord">coord where user wants to split into new rooms</param>
    private void DivideRoom(RoomPrefab room)
    {
        //Box box = room.boxes[0];x
        Box box = room.box;
        RemoveRoom(room);
        CreateDividedRooms(box);
    }

    private float GetAspect()
    {
        return (float) Screen.width / (float) Screen.height;
    }

    /// <summary>
    /// Should divide the room vertically or horizontally depending on orientation and previous cut
    /// </summary>
    /// <param name="roomAspectRatio">aspect ratio fo room being dividied</param>
    private void CreateDividedRooms(Box box)
    {
        //Debug.Log("Aspect camera " + Camera.main.aspect);
        float aspect = GetAspect();
        float deltaAspectRatio = Mathf.Abs(aspect - box.getAspectRatio());

        bool landscape = aspect > 1;

        float dividedAspectRatio = landscape ? aspect / 2 : aspect * 2;
        float deltaDividedAspectRatio = Mathf.Abs(dividedAspectRatio - box.getAspectRatio());

        bool mismatch = deltaAspectRatio > deltaDividedAspectRatio;

        bool DivideHor = landscape ? mismatch : !mismatch;

        Vector2Int box1Min = box.min;
        Vector2Int box1Max = DivideHor ? new Vector2Int(box.max.x, box.getMidY()) : new Vector2Int(box.getMidX(), box.max.y);

        Vector2Int box2Min = DivideHor ? new Vector2Int(box1Min.x, box1Max.y) : new Vector2Int(box1Max.x, box1Min.y); 
        Vector2Int box2Max = box.max;

        // bounds will be created in room 1, bounds will be shared with room 2
        RoomPrefab room2 = CreateRoom(box2Min, box2Max, false);
        RoomPrefab room1 = CreateRoom(box1Min, box1Max, true);

        room1.AddAdjacentRoom(room2);
        room2.AddAdjacentRoom(room1);
    }

    private void RemoveRoom(RoomPrefab room)
    {
        Rooms.Remove(room);
        room.RemoveEdgeCollider();
        GameObject.Destroy(room.gameObject);
    }

    private void CreateMap()
    {
        for (int c=0; c < horCells; c++)
        {
            for (int r=0; r < vertCells; r++)
            {
                Vector2Int pos = new Vector2Int(c, r);
                Cell cell = CreateCell(pos);

                grid.Add(cell);
            }
        }
    }

    private Cell CreateCell(Vector2Int pos)
    {
        int col = pos.x;
        int row = pos.y;

        Vector2 bl = new Vector2(col * cellLength, row * cellLength).RoundOff();
        Vector2 br = new Vector2(bl.x + cellLength, bl.y).RoundOff();
        Vector2 tl = new Vector2(bl.x, bl.y + cellLength).RoundOff();
        Vector2 tr = new Vector2(bl.x + cellLength, bl.y + cellLength).RoundOff();

        return new Cell(bl, br, tl, tr, row, col, EdgeDaddy);
    }

    private void OnDisable()
    {
        Controller.TouchEvent -= OnSingleTouch;
        Controller.DragEvent -= OnDrag;
    }
}

