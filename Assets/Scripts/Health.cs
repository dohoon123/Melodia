using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int currentHp = 50;
    [SerializeField] int maxHp = 50;

    public void TakeDamage(int damage) {
        currentHp -= damage;
    }

    public int GetCurrentHealth() {
        return currentHp;
    }
}
