using UnityEngine;

public class RoomData : MonoBehaviour
{
    public RoomDoor[] doors;
    public int doorCount;
    public RoomType roomType;
    public RectInt bounds;
    public enum RoomType
    {
        Start,
        Normal,
        Corridor,
        Special,
        Exit,
        Boss
    }

    private void Update()
    {
        Debug.DrawLine(new Vector3(bounds.xMin, bounds.yMin), new Vector3(bounds.xMax, bounds.yMin), Color.red);
        Debug.DrawLine(new Vector3(bounds.xMin, bounds.yMin), new Vector3(bounds.xMin, bounds.yMax), Color.red);
        Debug.DrawLine(new Vector3(bounds.xMax, bounds.yMin), new Vector3(bounds.xMax, bounds.yMax), Color.red);
        Debug.DrawLine(new Vector3(bounds.xMin, bounds.yMax), new Vector3(bounds.xMax, bounds.yMax), Color.red);
    }
}
