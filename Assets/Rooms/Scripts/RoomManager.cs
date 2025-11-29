using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static RoomData;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private TileBase blackTile;
    public RoomData[] roomPrefabs;
    public RoomData[] doorBlockPrefabs;
    public int maxDepth;

    public static event Action OnGenerationComplete;

    private HashSet<RectInt> occupiedTiles = new HashSet<RectInt>();
    public List<RectInt> dungeonBounds;

    void Start()
    {
        GenerateDungeon(maxDepth);
    }

    // Instantiate the root room at (0, 0) and start recursing for the rest of the rooms
    public RoomNode GenerateDungeon(int maxDepth)
    {
        RoomNode root = new RoomNode(RoomType.Start, 0);

        RoomData[] startingRooms = roomPrefabs.Where(r => r.roomType == RoomType.Start).ToArray();
        RoomData startRoomData = startingRooms[Random.Range(0, startingRooms.Length)];
        RoomData startRoomInstance = Instantiate(startRoomData, Vector3.zero, Quaternion.identity);
        root.roomData = startRoomInstance;
        RectInt newBounds = root.roomData.bounds = startRoomInstance.bounds;
        PlaceRoom(newBounds);
        dungeonBounds.Add(newBounds);

        GenerateChildren(root, maxDepth);

        GenerateDarkness(dungeonBounds);
        OnGenerationComplete?.Invoke();
        return root;
    }

    private void GenerateChildren(RoomNode parent, int maxDepth)
    {
        if (parent.depth >= maxDepth) return;

        // Pick a random door from the parent that is not already used
        var availableDoors = parent.roomData.doors
          .Select((r, i) => new { Door = r, Index = i })
          .Where(d => !d.Door.isConnected).ToList();

        // If the current room is the root/starting room, it should only have 1 children, otherwise choose 1 - 3 children
        //int desiredBranches = (parent.depth == 0) ? 1 : Random.Range(1, 4);
        // If the desired number of children is more than possible, set it to the most possible
        //int branches = Mathf.Min(desiredBranches, availableDoors.Count);
        int branches = availableDoors.Count;
        RoomNode current;
        for (int i = 0; i < branches; i++)
        {
            // Make random even rooms special, based on depth as long as its parent isn't already a special room, and odd rooms corridors
            RoomType type = parent.depth % 2 == 0 ? RoomType.Corridor : (parent.roomData.roomType == RoomType.Special
                || Random.Range(0f, 1f) >= GetSpecialRoomChance(parent.depth + 1)) ? RoomType.Normal : RoomType.Special;

            if (parent.depth + 1 == maxDepth) type = RoomType.Exit;

            current = new RoomNode(type, parent.depth + 1, parent);
            parent.AddChild(current);

            var chosenDoor = availableDoors[Random.Range(0, availableDoors.Count)];
            availableDoors.Remove(chosenDoor);
            RoomDoor parentDoor = parent.roomData.doors[chosenDoor.Index];

            // Get a random room prefab with a number of doors greater than or equal
            // to the number of children it will have, plus one for the parent room
            // and has a door on the correct side
            RoomData[] rooms = roomPrefabs
                .Where(r => r.doors.Any(r => r.direction == parentDoor.GetOpposite())
                && r.roomType == type).ToArray();

            // If there are no rooms with that door skip this room
            if(rooms.Length <= 0)
            {
                continue;
            }

            // If a room cannot be placed, try a different prefab 5 times
            bool isPlaced = false;
            int attempts = 0;
            int maxAttempts = 5;
            while (!isPlaced && attempts < maxAttempts)
            {
                RoomData currentRoomData = rooms[Random.Range(0, rooms.Length)];

                // Get a random door on the prefab that can connect to its parent
                // and store its index in the original array
                var childDoors = currentRoomData.doors
                    .Select((r, i) => new { Door = r, Index = i })
                    .Where(d => !d.Door.isConnected && d.Door.direction == parentDoor.GetOpposite()).ToArray();
                int childDoorIndex = childDoors[Random.Range(0, childDoors.Length)].Index;
                RoomDoor childDoor = currentRoomData.doors[childDoorIndex];

                Vector3 parentDoorPos = parentDoor.doorPos.position;
                Vector3 childDoorPos = childDoor.doorPos.position;

                Vector3 offset = parentDoorPos - childDoorPos;

                RoomData data = Instantiate(currentRoomData, offset, Quaternion.identity);
                current.roomData = data;

                Vector3 currentPos = data.transform.position;

                // Create a new bounds from the size of the prefab offset by the position of the room
                int roomX = Mathf.RoundToInt(currentPos.x);
                int roomY = Mathf.RoundToInt(currentPos.y);
                RectInt newBounds = new RectInt(current.roomData.bounds.x + roomX, current.roomData.bounds.y + roomY,
                    current.roomData.bounds.width, current.roomData.bounds.height);
                // Check if the bounds of the current room would overlap with another's
                if (!CanPlaceRoom(newBounds))
                {
                    Object.Destroy(data.gameObject);
                    attempts++;
                    continue;
                }
                else
                {
                    // Else set the bounds to the offset bounds and add the bounds to the stored bounds
                    current.roomData.bounds = newBounds;
                    PlaceRoom(newBounds);
                    dungeonBounds.Add(newBounds);

                    // Set the door to be used
                    current.roomData.doors[childDoorIndex].isConnected = true;
                    parent.roomData.doors[chosenDoor.Index].isConnected = true;

                    isPlaced = true;

                    GenerateChildren(current, maxDepth);

                    foreach (RoomDoor door in current.roomData.doors.Where(d => !d.isConnected).ToArray())
                    {
                        Vector3 doorOffset = new Vector3(0, 0, -0.2f);
                        Vector3 pos = door.doorPos.position + doorOffset;
                        RoomData doorBlock = doorBlockPrefabs.First(r => r.doors.Any(r => r.direction == door.direction));
                        RoomData blockInstance = Instantiate(doorBlock, pos, Quaternion.identity);
                    }
                }
            }
        }
    }
    float GetSpecialRoomChance(int x)
    {
        float H = 0.5f;         // Maximum probability possible
        float c = maxDepth / 2; // Center of the probability curve
        float s = 2;            // Steepness of the curve
        return H * Mathf.Exp(-((x - c)*(x - c)) / (2 * (s * s))) * ((x * (2 * c - x)) / (c*c));
    }

    bool CanPlaceRoom(RectInt bounds)
    {
        foreach (var rect in occupiedTiles)
        {
            if (bounds.Overlaps(rect)) return false;
        }
        return true;
    }

    void PlaceRoom(RectInt bounds)
    {
        occupiedTiles.Add(bounds);
    }

    void GenerateDarkness(List<RectInt> roomBounds)
    {
        var minX = roomBounds.Min(r => r.xMin) - 20;
        var minY = roomBounds.Min(r => r.yMin) - 20;
        var maxX = roomBounds.Max(r => r.xMax) + 20;
        var maxY = roomBounds.Max(r => r.yMax) + 20;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), blackTile);
            }
        }

        foreach (RectInt room in roomBounds)
        {
            for (int x = room.xMin; x < room.xMax; x++)
            {
                for (int y = room.yMin; y < room.yMax; y++)
                {
                    tileMap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }
}