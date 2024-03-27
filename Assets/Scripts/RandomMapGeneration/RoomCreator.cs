using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomCreator : MonoBehaviour
{
    enum State { CreateRoom, SpreadRooms, SelectRooms, ConnectRooms };
    
    [SerializeField] int circleRadius = 100;

    [SerializeField] int roomCount = 100;
    [SerializeField] int pixelPerUnit = 16;

    List<Room> roomList; 
    List<List<Room>> roomLinkList;
    Dictionary<string, Room> vectorRoomMap;
    HashSet<Vector2Int> floorTileCoordinates;
    HashSet<Vector2Int> corridorTileCoordinates;

    DelaunayTriangulation DT;
    MST mst;

    float totalWidth = 0; float totalHeight = 0;

    public bool isProcessFinished = false;

    void Awake() {   
        roomList = new();
        roomLinkList = new();
        vectorRoomMap = new();
        floorTileCoordinates = new();
        corridorTileCoordinates = new();

        DT = new();
        mst = new();
    }

    public void CreateTileCoordinates() {
        StartCoroutine(sequentialCoroutine());
    }

    public HashSet<Vector2Int> GetFloorTileCoordinates() {
        return floorTileCoordinates;
    }

    public HashSet<Vector2Int> GetCorridorTileCoordinates() {
        return corridorTileCoordinates;
    }

    IEnumerator sequentialCoroutine() {
        CreateRooms();
        yield return StartCoroutine(SpreadRooms());
        RoomListToDictionary();
        SelectRooms();
        ConnectRooms();
    }

    private void ConnectRooms() {

        List<Vector2> battleRoomPositionList = new List<Vector2>();

        foreach (Room room in roomList) {
            if (room.myRoomType == Room.RoomType.BattleRoom) {
                battleRoomPositionList.Add(new Vector2(room.roomPosition.x, room.roomPosition.y));
            }
        }

        List<Triangle> rightTriangles = DT.ConnectDelaunayTriangulation(battleRoomPositionList);
        List<List<Vector2>> mstPath = mst.GetMST(rightTriangles);

        ConnectCorridorsBtwRooms(mstPath);
    }

    private void ConnectCorridorsBtwRooms(List<List<Vector2>> path) {
        FromMSTPathToRoomLinkList(path);
        ConnectTwoRoom();
        AddRoomsCoordinates();
        isProcessFinished = true;
    }

    private void AddRoomsCoordinates() {
        foreach (Room room in roomList) {
            if (room.myRoomType == Room.RoomType.BattleRoom) {

                int startX = room.roomPosition.x - room.width;
                int startY = room.roomPosition.y - room.height;

                for (int i = 0; i <= room.width; i++) {
                    for (int j = 0; j <= room.height; j++) {
                        floorTileCoordinates.Add(new Vector2Int(startX + i, startY + j));
                    }
                }
            }
        }
    }

    private void ConnectTwoRoom() {
        foreach (List<Room> link in roomLinkList) {
            Room first = link[0];
            Room second = link[1];

            int firstPosX = link[0].roomPosition.x;
            int firstPosY = link[0].roomPosition.y;

            int secondPosX = link[1].roomPosition.x;
            int secondPosY = link[1].roomPosition.y;

            int midpointX = (firstPosX + secondPosX) / 2;
            int midpointY = (firstPosY + secondPosY) / 2;

            int biggerX = Mathf.Max(firstPosX, secondPosX);
            int smallerX = Mathf.Min(firstPosX, secondPosX);

            int biggerY = Mathf.Max(firstPosY, secondPosY);
            int smallerY = Mathf.Min(firstPosY, secondPosY);

            //vertical corridor
            if (firstPosX - first.width / 2 < midpointX && midpointX < firstPosX + first.width / 2 &&
                secondPosX - second.width / 2 < midpointX && midpointX < secondPosX + second.width / 2) 
            {
                for (int i = smallerY; i <= biggerY; i++) {
                    corridorTileCoordinates.Add(new Vector2Int(midpointX, i));
                }
                continue;
            }

            //horizontal corridor
            if (firstPosY - first.height / 2 < midpointY && midpointY < firstPosY + first.height / 2 &&
                secondPosY - second.height / 2 < midpointY && midpointY < secondPosY + second.height / 2) 
            {
                for (int i = smallerX; i <= biggerX; i++) {
                    corridorTileCoordinates.Add(new Vector2Int(i, midpointY));
                }
                continue;
            }

            // L-shape corridor
            int intervalX = firstPosX < secondPosX ? 1 : -1;
            int intervalY = firstPosY < secondPosY ? 1 : -1;

            int startX = firstPosX;
            for (; startX != secondPosX; startX += intervalX) {
                corridorTileCoordinates.Add(new Vector2Int(startX, firstPosY));
            }
            
            for (int i = firstPosY; i != secondPosY; i += intervalY) {
                corridorTileCoordinates.Add(new Vector2Int(startX, i));
            }
        }
    }

    private void FromMSTPathToRoomLinkList(List<List<Vector2>> path) {
        foreach (List<Vector2> segment in path) {
            vectorRoomMap.TryGetValue(Vector2ToString(segment[0]), out Room from);
            vectorRoomMap.TryGetValue(Vector2ToString(segment[1]), out Room to);

            List<Room> tempList = new List<Room>() { from, to };
            roomLinkList.Add(tempList);
        }
    }

    private void DrawTriangles() {
        DT.DrawRightTriangles();
    }

    private void SelectRooms()
    {
        foreach (Room room in roomList) {
            totalWidth += room.width; 
            totalHeight += room.height; 
        }

        int averageWidth = (int)totalWidth / roomList.Count;
        int averageHeight = (int)totalHeight / roomList.Count;

        foreach (Room room in roomList) {
            if (room.height > averageHeight && room.width > averageWidth) {
                room.myRoomType = Room.RoomType.BattleRoom;
            }
        }
    }

    private string Vector2ToString(Vector2 inVector) {
        string posX = ((int)(inVector.x)).ToString();
        string posY = ((int)(inVector.y)).ToString();
        string str = posX + "/" + posY;

        return str;
    }

    IEnumerator SpreadRooms() {
        bool isSpreadFinished = false;

        do {
            isSpreadFinished = true;
            for (int i = 0; i < roomList.Count; i++) {
                if (isOverlapped(i)) {
                    isSpreadFinished = false;
                }
            }           
            yield return null;
        } while (!isSpreadFinished);
    }

    private void RoomListToDictionary() {
        //Add rooms to vectorMap
        foreach (Room room in roomList) {
            vectorRoomMap.TryAdd(Vector2ToString(room.roomPosition), room);
        }
    }

    void CreateRooms(){
        for (int i = 0 ; i < roomCount; i++) {
            Room newRoom = new();
            newRoom.SetPosition(circleRadius);
            roomList.Add(newRoom);
        }
    }   

    bool isOverlapped(int index){
        int overlappedCount = 0;
        Room currentRoom = roomList[index];

        for (int i = 0; i < roomList.Count; i++) {
            if (index == i) continue;

            Room targetRoom = roomList[i];
            
            if (Vector2Int.Distance(currentRoom.roomPosition, new Vector2Int(0, 0)) > Vector2Int.Distance(targetRoom.roomPosition, new Vector2Int(0, 0))) { continue; }

            if (RoomAABB(currentRoom, targetRoom)) {
                overlappedCount++;

                float moveDirection = UnityEngine.Random.Range(0.0f, 1.0f);

                if (moveDirection > 0.5f) {
                    int moveForceX = targetRoom.roomPosition.x - currentRoom.roomPosition.x;
                    roomList[i].roomPosition += new Vector2Int(moveForceX, 0);
                }else {
                    int moveForceY = targetRoom.roomPosition.y - currentRoom.roomPosition.y;
                    roomList[i].roomPosition += new Vector2Int(0, moveForceY);
                }
            }
        }

        if (overlappedCount > 0) return true;

        return false;
    }

    bool RoomAABB(Room currentRoom, Room targetRoom){
        if (currentRoom.roomPosition.x + currentRoom.width > targetRoom.roomPosition.x - targetRoom.width && 
            currentRoom.roomPosition.x - currentRoom.width < targetRoom.roomPosition.x + targetRoom.width && 
            currentRoom.roomPosition.y + currentRoom.height > targetRoom.roomPosition.y - targetRoom.height && 
            currentRoom.roomPosition.y - currentRoom.height < targetRoom.roomPosition.y + targetRoom.height)
        {
            return true;
        }

        return false;
    }
}
