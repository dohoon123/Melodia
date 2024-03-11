using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MonsterData", fileName = "New MonsterData")]
public class MonsterDataSO : ScriptableObject 
{
    public int currentHp = 100;
    public int maxHp = 100;
    public float moveSpeed = 5.0f;
    public float detectRange = 1.0f;
    public float attackRange = 1.0f;
}
