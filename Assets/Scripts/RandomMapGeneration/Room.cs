using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Room
{
    [SerializeField] GenerationDataSO generationDataSO;

    public enum RoomType { FreshRoom, BattleRoom, CorridorRoom };

    public int width, height;
    public Vector2Int roomPosition;

    public RoomType myRoomType;

    public HashSet<Vector2Int> nearWallTiles = new();
    public List<GameObject> enemiesInThisRoom = new();

    void Awake() {
        width = UnityEngine.Random.Range(generationDataSO.minWidth, generationDataSO.maxWidth) * 2;
        height = UnityEngine.Random.Range(generationDataSO.minHeight, generationDataSO.maxHeight) * 2;

        myRoomType = RoomType.FreshRoom;
    }

    public void SetPosition(int areaRadius) {
        float r = Mathf.Sqrt(UnityEngine.Random.Range(0.0f, 1.0f)) * areaRadius;
        float theta = UnityEngine.Random.Range(0.0f, 1.0f) * 2 * Mathf.PI;

        roomPosition.x = (int)Mathf.Round(r * Mathf.Cos(theta));
        roomPosition.y = (int)Mathf.Round(r * Mathf.Sin(theta));
    }

}
