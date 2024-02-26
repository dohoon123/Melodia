using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    Vector2 inputVec;

    float moveSpeed = 10.0f;

    Rigidbody2D myRigidbody;
    SpriteRenderer mySpriter;
    Animator myAnimator;
    CapsuleCollider2D myCollider;

    void Awake() {
        myRigidbody = GetComponent<Rigidbody2D>();
        mySpriter= GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start() {
        
    }
    
    void Update() {
        Move();
    }
    
    void LateUpdate() {
        FlipSprite();
    }

    void FlipSprite() {
        if (inputVec.x != 0) {
            mySpriter.flipX = inputVec.x > 0;
        }
    }

    void OnMove(InputValue value) {
        inputVec = value.Get<Vector2>();
    }

    void Move() {
        Vector2 moveVector = inputVec * moveSpeed * Time.fixedDeltaTime;
        myRigidbody.MovePosition(myRigidbody.position + moveVector);
    }
}
