using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

[System.Serializable]
public class DirectionInfo
{
    public Direction dir; // An integer representing a direction.
    public bool unExposed;   // A boolean indicating if the direction is open.

    public DirectionInfo(Direction direction, bool open)
    {
        dir = direction;
        unExposed = open;
    }
}


public class Room : MonoBehaviour
{

    //Dictionary<Direction, Direction> oppositeDirections = new Dictionary<Direction, Direction>
    //{
    //    {Direction.UP, Direction.DOWN},
    //    {Direction.DOWN, Direction.UP},
    //    {Direction.LEFT, Direction.RIGHT},
    //    {Direction.RIGHT, Direction.LEFT}
    //};

    public List<DirectionInfo> availableDirections = new List<DirectionInfo>();

}
