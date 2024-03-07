using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBullet : MonoBehaviour
{
    Rigidbody2D targetRigidbody;
    Rigidbody2D myRigidbody;
    Vector2 direction;

    //int damage = 10;
    float speed = 10.0f;

    Collider2D myCollider;

    private void Awake() {
        targetRigidbody = GameManager.instance.player.GetComponent<Rigidbody2D>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
    }

    private void FixedUpdate() {
        Shoot();
    }

    public void Set(Vector3 position) {
        myRigidbody.position = position;
        direction = targetRigidbody.position - myRigidbody.position;
    }

    void Shoot() {
        myRigidbody.velocity = direction.normalized * speed;
    }

    private void OnTriggerEnter(Collider other) {

    }
}
