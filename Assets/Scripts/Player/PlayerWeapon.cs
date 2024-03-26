using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public enum EWeaponType { Laser, Bezier, Melee };
    
    protected Rigidbody2D myRigidbody;
    protected Collider2D myCollider;

    protected void Awake() {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();

        myWeaponType = EWeaponType.Laser;
        damage = 10;
    }


    public EWeaponType myWeaponType;
    protected int damage;

}
