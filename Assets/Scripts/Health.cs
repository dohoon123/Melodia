using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int currentHP = 50;
    [SerializeField] int maxHP = 50;

    public void TakeDamage(int damage) {
        currentHP -= damage;
    }

    public int GetCurrentHP() {
        return currentHP;
    }

    public int GetMaxHP() {
        return maxHP;
    }
}
