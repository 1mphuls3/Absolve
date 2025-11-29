using System.Collections.Generic;
using UnityEngine;
using static RoomData;

public class RoomNode
{
    public RoomType type;
    public List<RoomNode> children;
    public RoomNode parent;
    public int depth;
    public bool isPlaced;
    public RoomData roomData;
    public RoomNode(RoomType type, int depth = 0, RoomNode parent = null)
    {
        this.type = type;
        this.depth = depth;
        this.parent = parent;
        this.children = new List<RoomNode>();
        this.isPlaced = false;
        this.roomData = null;
    }

    public void AddChild(RoomNode node)
    {
        children.Add(node);
    }
}
