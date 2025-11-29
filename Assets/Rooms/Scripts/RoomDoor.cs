using System.Collections;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public Transform doorPos;
    public Direction direction;
    public bool isConnected = false;
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public Direction GetOpposite() => direction switch
    {
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        _ => direction,
    };
}
