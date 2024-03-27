using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField] RoomCreator roomCreator;

    protected override IEnumerator RunProceduralGeneration(){

        roomCreator.CreateTileCoordinates();

        while (!roomCreator.isProcessFinished) {
            yield return null;
        }
        HashSet<Vector2Int> floorTilePositions = roomCreator.GetFloorTileCoordinates();
        HashSet<Vector2Int> corridorTilePositions = roomCreator.GetCorridorTileCoordinates();
        tilemapVisualizer.PaintFloorTiles(floorTilePositions);
        WallGenerator.CreatWalls(floorTilePositions, tilemapVisualizer);
    }
}
