using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomTypeData", menuName = "RoomType")]
public class RoomTypeSO : ScriptableObject
{
    public int width = 10, height = 10;
    public string typeName = "";
}
