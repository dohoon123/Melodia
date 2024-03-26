using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBullet : PlayerWeapon
{
    Vector2 direction;
    float speed = 10.0f;
    private new void Awake() {
        base.Awake();

        myWeaponType = EWeaponType.Laser;
        damage = 10;
    }

    private void FixedUpdate() {
        Shoot();
    }

    public void Set(Vector3 position, Vector2 mousePosition) {
        myRigidbody.position = position;
        direction = mousePosition - myRigidbody.position;
    }

    void Shoot() {
        myRigidbody.velocity = direction.normalized * speed;
    }
}
