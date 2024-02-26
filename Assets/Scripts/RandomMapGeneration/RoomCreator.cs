using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

public class RoomCreator : MonoBehaviour
{
    enum State { CreateRoom, SpreadRooms, SelectRooms, ConnectRooms };
    
    [SerializeField] GameObject room;
    [SerializeField] int roomCount = 100;
    [SerializeField] GameObject dot;
    [SerializeField] int pixelPerUnit = 16;

    List<GameObject> roomList; 
    List<List<GameObject>> roomLinkList;
    Dictionary<string, GameObject> vectorRoomMap;
    HashSet<Vector2Int> tileCoordinates;

    DelaunayTriangulation DT = new DelaunayTriangulation();
    MST mst = new MST();

    float totalWidth = 0; float totalHeight = 0;

    public bool isProcessFinished = false;

    void Awake() {   
        roomList = new List<GameObject>();
        roomLinkList = new List<List<GameObject>>();
        vectorRoomMap = new Dictionary<string, GameObject>();
        tileCoordinates = new HashSet<Vector2Int>();

        DT = new DelaunayTriangulation();
    }

    public void CreateTileCoordinates() {
        StartCoroutine(sequentialCoroutine());
    }

    public HashSet<Vector2Int> GetTileCoordinates() {
        return tileCoordinates;
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

        foreach (GameObject room in roomList) {
            BasicRoom curRoom = room.GetComponent<BasicRoom>();
            if (curRoom.myRoomType == BasicRoom.RoomType.BattleRoom) {
                battleRoomPositionList.Add(new Vector2(room.transform.position.x, room.transform.position.y));
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
        //ShowTileMapCoordinates();
    }

    private void ShowTileMapCoordinates() {
        foreach (Vector2Int coordinate in tileCoordinates) {
            GameObject go = Instantiate(dot, transform);
            go.transform.position = new Vector3(coordinate.x, coordinate.y, 0);
        }
    }

    private void AddRoomsCoordinates() {
        foreach (GameObject room in roomList) {
            BasicRoom curRoom = room.GetComponent<BasicRoom>();
            if (curRoom.myRoomType == BasicRoom.RoomType.BattleRoom) {

                int width = (int)curRoom.myWidth / pixelPerUnit;
                int height = (int)curRoom.myHeight / pixelPerUnit;

                int startX = (int)curRoom.transform.position.x - width / 2;
                int startY = (int)curRoom.transform.position.y - height /2;

                for (int i = 0; i <= width; i++) {
                    for (int j = 0; j <= height; j++) {
                        tileCoordinates.Add(new Vector2Int(startX + i, startY + j));
                    }
                }
            }
        }
    }

    private void ConnectTwoRoom() {
        foreach (List<GameObject> rooms in roomLinkList) {
            BasicRoom first = rooms[0].GetComponent<BasicRoom>();
            BasicRoom second = rooms[1].GetComponent<BasicRoom>();

            int firstPosX = (int)(rooms[0].transform.position.x);
            int firstPosY = (int)(rooms[0].transform.position.y);

            int secondPosX = (int)(rooms[1].transform.position.x);
            int secondPosY = (int)(rooms[1].transform.position.y);

            int midpointX = (firstPosX + secondPosX) / 2;
            int midpointY = (firstPosY + secondPosY) / 2;

            int divideSize = pixelPerUnit * 2;

            int biggerX = Mathf.Max((int)firstPosX, (int)secondPosX);
            int smallerX = Mathf.Min((int)firstPosX, (int)secondPosX);

            int biggerY = Mathf.Max((int)firstPosY, (int)secondPosY);
            int smallerY = Mathf.Min((int)firstPosY, (int)secondPosY);

            //vertical corridor
            if (midpointX > firstPosX - first.myWidth / divideSize && midpointX < firstPosX + first.myWidth / divideSize &&
                midpointX > secondPosX - second.myWidth / divideSize && midpointX < secondPosX + second.myWidth / divideSize) 
            {
                for (int i = smallerY; i <= biggerY; i++) {
                    tileCoordinates.Add(new Vector2Int(midpointX, i));
                }
                continue;
            }

            //horizontal corridor
            if (midpointY > firstPosY - first.myHeight / divideSize && midpointY < firstPosY + first.myHeight / divideSize &&
                midpointY > secondPosY - second.myHeight / divideSize && midpointY < secondPosY + second.myHeight / divideSize) 
            {
                for (int i = smallerX; i <= biggerX; i++) {
                    tileCoordinates.Add(new Vector2Int(i, midpointY));
                }
                continue;
            }

            int intervalX = firstPosX < secondPosX ? 1 : -1;
            int intervalY = firstPosY < secondPosY ? 1 : -1;

            int startX = (int)firstPosX;
            for (; startX != (int)secondPosX; startX += intervalX) {
                tileCoordinates.Add(new Vector2Int(startX, firstPosY));
            }
            
            for (int i = (int)firstPosY; i != (int)secondPosY; i += intervalY) {
                tileCoordinates.Add(new Vector2Int(startX, i));
            }
        }
    }

    private void FromMSTPathToRoomLinkList(List<List<Vector2>> path) {
        foreach (List<Vector2> segment in path) {
            GameObject from;
            vectorRoomMap.TryGetValue(Vector2ToString(segment[0]), out from);

            GameObject to;
            vectorRoomMap.TryGetValue(Vector2ToString(segment[1]), out to);

            List<GameObject> tempList = new List<GameObject>() { from, to };
            roomLinkList.Add(tempList);
        }
    }

    private void DrawTriangles() {
        DT.DrawRightTriangles();
    }

    private void SelectRooms()
    {
        foreach (GameObject room in roomList) {
            BasicRoom tr = room.GetComponent<BasicRoom>();
            totalWidth += tr.myWidth; 
            totalHeight += tr.myHeight; 
        }

        int averageWidth = (int)totalWidth / roomList.Count;
        int averageHeight = (int)totalHeight / roomList.Count;

        foreach (GameObject room in roomList)
        {
            BasicRoom tr = room.GetComponent<BasicRoom>();

            // Condition
            // 1. bigger than average size;
            if (tr.myHeight > averageHeight && tr.myWidth > averageWidth) {
                tr.myRoomType = BasicRoom.RoomType.BattleRoom;
                tr.sr.color = Color.magenta;
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
        foreach (GameObject room in roomList) {
            vectorRoomMap.TryAdd(Vector2ToString(room.transform.position), room);
        }
    }

    void CreateRooms(){
        for (int i = 0 ; i < roomCount; i++) {
            GameObject go = Instantiate(room, transform);
            roomList.Add(go);
        }
    }   

    bool isOverlapped(int index){
        int overlappedCount = 0;

        BasicRoom currentRoom = roomList[index].GetComponent<BasicRoom>();

        float moveForceX = 0, moveForceY = 0;

        for (int i = 0; i < roomList.Count; i++)
        {
            if (index == i) continue;

            BasicRoom targetRoom = roomList[i].GetComponent<BasicRoom>();
            
            if (Vector3.Distance(currentRoom.transform.position, new Vector3(0, 0, 0)) <= Vector3.Distance(targetRoom.transform.position, new Vector3(0, 0, 0)))
            {
                //overlapped (AABB)
                if (BasicRoomAABB(currentRoom, targetRoom))
                {
                    overlappedCount++;
                    moveForceX = targetRoom.transform.position.x - currentRoom.transform.position.x;
                    moveForceY = targetRoom.transform.position.y - currentRoom.transform.position.y;

                    if (moveForceX != 0) moveForceX = Mathf.Round(moveForceX / Mathf.Abs(moveForceX));
                    if (moveForceY != 0) moveForceY = Mathf.Round(moveForceY / Mathf.Abs(moveForceY));

                    if (moveForceX != 0 && moveForceY != 0)
                    {
                        float moveXY = UnityEngine.Random.Range(0.0f, 1.0f);

                        if (moveXY > 0.5f)
                        {
                            moveForceX = 0;
                        }else{
                            moveForceY = 0;
                        }
                    }
                    roomList[i].transform.position += new Vector3(moveForceX, moveForceY, 0);
                }
            }
        }

        if (overlappedCount > 0) return true;

        return false;
    }

    // divide by 32 cause 16 pixel per unit
    bool BasicRoomAABB(BasicRoom currentRoom, BasicRoom targetRoom){
        if (currentRoom.transform.position.x + (currentRoom.myWidth / 32) > targetRoom.transform.position.x - (targetRoom.myWidth / 32) && 
            currentRoom.transform.position.x - (currentRoom.myWidth / 32) < targetRoom.transform.position.x + (targetRoom.myWidth / 32) && 
            currentRoom.transform.position.y + (currentRoom.myHeight / 32) > targetRoom.transform.position.y - (targetRoom.myHeight / 32) && 
            currentRoom.transform.position.y - (currentRoom.myHeight / 32) < targetRoom.transform.position.y + (targetRoom.myHeight / 32)            
            )
        {
            return true;
        }

        return false;
    }
}
