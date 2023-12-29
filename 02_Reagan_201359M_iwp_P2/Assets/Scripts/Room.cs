using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
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



//[CustomEditor(typeof(Room))]
//public class RoomEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        Room room = (Room)target;

//        if (GUILayout.Button("Assign Prefab ID"))
//        {
//            // Get the asset path of the prefab
//            string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(room);

//            // Extract the name from the asset path
//            room.prefabId = System.IO.Path.GetFileNameWithoutExtension(assetPath);

//            EditorUtility.SetDirty(room);
//        }
//    }
//}



public class Room : MonoBehaviour
{
    [HideInInspector]
    public int prefabId;


    //public string prefabId;

    void Start()
    {
        //Debug.Log($"ID IS {prefabId}");
    }

    //Dictionary<Direction, Direction> oppositeDirections = new Dictionary<Direction, Direction>
    //{
    //    {Direction.UP, Direction.DOWN},
    //    {Direction.DOWN, Direction.UP},
    //    {Direction.LEFT, Direction.RIGHT},
    //    {Direction.RIGHT, Direction.LEFT}
    //};

    public List<DirectionInfo> availableDirections = new List<DirectionInfo>();

}
