using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BasicRoom : MonoBehaviour
{
    [SerializeField] int circleRadius = 100;
    [SerializeField] GenerationDataSO generationDataSO;

    public enum RoomType { FreshRoom, BattleRoom, CorridorRoom };

    public Texture2D texture;
    public Sprite mySprite;
    public SpriteRenderer sr;

    public float myWidth, myHeight;
    public RoomType myRoomType;

    void Awake() {
        sr = gameObject.GetComponent<SpriteRenderer>();

        myWidth = UnityEngine.Random.Range(generationDataSO.minWidth, generationDataSO.maxWidth);
        myHeight = UnityEngine.Random.Range(generationDataSO.minHeight, generationDataSO.maxHeight);

        myWidth = (float)(myWidth - myWidth % 16) * 2;
        myHeight = (float)(myHeight - myHeight % 16) * 2;

        mySprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, myWidth, myHeight), new Vector2(0.5f, 0.5f), 16.0f);    
        sr.sprite = mySprite; 

        myRoomType = RoomType.FreshRoom;
    }

    void Start(){
        SetPosition();
    }

    void SetPosition()
    {
        float r = Mathf.Sqrt(UnityEngine.Random.Range(0.0f, 1.0f)) * circleRadius;
        float theta = UnityEngine.Random.Range(0.0f, 1.0f) * 2 * Mathf.PI;

        float x = Mathf.Round(r * Mathf.Cos(theta));
        float y = Mathf.Round(r * Mathf.Sin(theta));

        transform.position = new Vector3(x, y, 0);
    }

}
