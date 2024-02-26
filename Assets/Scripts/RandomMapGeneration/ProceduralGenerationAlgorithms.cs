using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProceduralGenerationAlgorithm
{
    public static HashSet<Vector2Int> CreateSimpleRoom(Vector2Int startPosition, int width, int height){
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        BoundsInt bound = new BoundsInt((Vector3Int)startPosition, new Vector3Int(width, height, 0));

        for (int col = 0; col < bound.size.x; col++)
        {
            for (int row = 0; row < bound.size.y; row++)
            {
                Vector2Int position = (Vector2Int)bound.min + new Vector2Int(col, row);
                floor.Add(position);
            }
        }
        return floor;
    }
}

public static class Direction2D{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1), //UP
        new Vector2Int(1, 0), //RIGHT
        new Vector2Int(0, -1), //DOWN
        new Vector2Int(-1, 0), //LEFT
    };

    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1, 1), //UP-RIGHT
        new Vector2Int(1, -1), //RIGHT-DOWN
        new Vector2Int(-1, -1), //DOWN-LEFT
        new Vector2Int(-1, 1), //LEFT-UP
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>(){
        new Vector2Int(0, 1), //UP
        new Vector2Int(1, 1), //UP-RIGHT
        new Vector2Int(1, 0), //RIGHT
        new Vector2Int(1, -1), //RIGHT-DOWN
        new Vector2Int(0, -1), //DOWN
        new Vector2Int(-1, -1), //DOWN-LEFT
        new Vector2Int(-1, 0), //LEFT
        new Vector2Int(-1, 1), //LEFT-UP
    };

    public static Vector2Int GetRandomCardinalDirection(){
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}