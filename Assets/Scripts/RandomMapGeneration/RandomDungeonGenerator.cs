using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    RoomCreator roomCreator;

    protected override IEnumerator RunProceduralGeneration(){

        roomCreator.CreateTileCoordinates();

        while (!roomCreator.isProcessFinished) {
            yield return null;
        }
        HashSet<Vector2Int> floorPositions = roomCreator.GetTileCoordinates();
        Debug.Log(floorPositions.Count);
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreatWalls(floorPositions, tilemapVisualizer);
    }
}
