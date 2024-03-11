using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other) {
        Health health = other.GetComponent<Health>();

        if (health != null) {
            health.TakeDamage(GetDamage());
        }
        Hit();
    }
    public int GetDamage() {
        return damage;
    }

    public void Hit() {
        //do sth
        gameObject.SetActive(false);
    }
}
