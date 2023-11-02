using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator2 : MonoBehaviour
{
    public List<GameObject> roomTemplates; // List of your room template prefabs.
    public GameObject entranceTemplate;   // The template for the entrance room.
    public GameObject exitTemplate;       // The template for the exit room.
    public Vector2 entrancePosition;
    public Vector2 exitPosition;
    public float roomInterval = 4.0f;

    private List<Vector2> CalculatePath()
    {
        // Use your pathfinding algorithm to determine the path between entrance and exit.
        // For this example, we'll use a straight-line path.
        List<Vector2> path = new List<Vector2>();
        Vector2 currentPosition = entrancePosition;
        Vector2 direction = (exitPosition - entrancePosition).normalized;
        float distance = Vector2.Distance(entrancePosition, exitPosition);

        for (float d = 0; d <= distance; d += roomInterval)
        {
            path.Add(currentPosition);
            currentPosition += direction * roomInterval;
        }

        // Add the exit position to the path.
        path.Add(exitPosition);

        return path;
    }

    private void Start()
    {
        List<Vector2> path = CalculatePath();
        GameObject previousRoom = null;
        Direction previousExitDirection = Direction.UP; // Initialize with any direction.

        for (int i = 0; i < path.Count; i++)
        {
            GameObject room;
            if (i == 0)
            {
                // First room is the entrance.
                room = Instantiate(entranceTemplate, path[i], Quaternion.identity);
            }
            else if (i == path.Count - 1)
            {
                // Last room is the exit.
                room = Instantiate(exitTemplate, path[i], Quaternion.identity);
            }
            else
            {
                // Instantiate a room template based on your roomTemplates list.
                int randomRoomIndex = Random.Range(0, roomTemplates.Count);
                room = Instantiate(roomTemplates[randomRoomIndex], path[i], Quaternion.identity);
            }

            Room roomScript = room.GetComponent<Room>();
            Direction exitDirection = GetExitDirection(roomScript, previousExitDirection);

            // Check and update availableDirections based on room connections.
            UpdateAvailableDirections(roomScript, exitDirection);

            // Ensure there is a connection between the previous room and the current room.
            if (previousRoom != null)
            {
                // You can add logic here to make sure the rooms are connected properly.
                // Update availableDirections of the previous room and the current room.
                UpdateRoomConnections(previousRoom.GetComponent<Room>(), exitDirection, roomScript);
            }

            previousRoom = room;
            previousExitDirection = exitDirection;
        }
    }

    private Direction GetExitDirection(Room room, Direction previousExitDirection)
    {
        // Check room's available directions to determine a valid exit direction.
        List<Direction> availableDirections = new List<Direction>();

        foreach (DirectionInfo info in room.availableDirections)
        {
            if (!info.unExposed)
            {
                availableDirections.Add(info.dir);
            }
        }

        // Ensure that the chosen exit direction is not the opposite of the previous exit direction.
        if (availableDirections.Count > 1 && availableDirections.Contains(GetOppositeDirection(previousExitDirection)))
        {
            availableDirections.Remove(GetOppositeDirection(previousExitDirection));
        }

        // Choose a random exit direction from the available ones.
        return availableDirections.Count > 0
            ? availableDirections[Random.Range(0, availableDirections.Count)]
            : Direction.UP; // Fallback to any direction.
    }

    private Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.UP:
                return Direction.DOWN;
            case Direction.DOWN:
                return Direction.UP;
            case Direction.LEFT:
                return Direction.RIGHT;
            case Direction.RIGHT:
                return Direction.LEFT;
            default:
                return Direction.UP;
        }
    }

    private void UpdateAvailableDirections(Room room, Direction exitDirection)
    {
        // Ensure that only the exit direction is considered open.
        foreach (DirectionInfo info in room.availableDirections)
        {
            info.unExposed = (info.dir == exitDirection);
        }
    }

    private void UpdateRoomConnections(Room previousRoom, Direction exitDirection, Room currentRoom)
    {
        // Update the previous room's availableDirections based on the exit direction.
        foreach (DirectionInfo info in previousRoom.availableDirections)
        {
            if (info.dir == exitDirection)
            {
                info.unExposed = false;
            }
        }

        // Update the current room's availableDirections based on the entrance direction.
        Direction entranceDirection = GetOppositeDirection(exitDirection);
        foreach (DirectionInfo info in currentRoom.availableDirections)
        {
            if (info.dir == entranceDirection)
            {
                info.unExposed = false;
            }
        }
    }
}
