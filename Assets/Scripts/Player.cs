using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    enum PlayerState { EPS_Alive, EPS_Dead };

    Vector2 inputVec;
    Vector2 pointerInput;

    float moveSpeed = 10.0f;
    
    Rigidbody2D myRigidbody;
    SpriteRenderer mySpriterRenderer;
    Animator myAnimator;
    CapsuleCollider2D myCollider;

    Health healthComponent;

    WeaponParent weaponParent;
    PlayerWeapon weapon;


    [SerializeField] InputActionReference pointerPosition;

    void Awake() {
        myRigidbody = GetComponent<Rigidbody2D>();
        mySpriterRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<CapsuleCollider2D>();

        healthComponent = GetComponent<Health>();

        weaponParent = GetComponentInChildren<WeaponParent>();
        weapon       = GetComponentInChildren<PlayerWeapon>();
    }
    
    void Update() {
        pointerInput = GetPointerInput();
        //weaponParent.pointerPosition = pointerInput;
        Move();
    }
    
    void LateUpdate() {
        FlipSprite();
    }

    void FlipSprite() {
        Vector2 direction = (pointerInput - (Vector2)transform.position).normalized;

        if (direction.x >= 0) {
            mySpriterRenderer.flipX = false;
        }else {
            mySpriterRenderer.flipX = true;
        }
    }

    void OnMove(InputValue value) {
        inputVec = value.Get<Vector2>();
    }

    void OnFire() {
        if (weaponParent != null) {
            weaponParent.Attack();
        }
    }

    void Move() {
        if (!IsPlayerMoving()) {
            myAnimator.SetBool("isMoving", false);
            return; 
        }

        Vector2 moveVector = inputVec * moveSpeed * Time.fixedDeltaTime;
        myRigidbody.MovePosition(myRigidbody.position + moveVector);
        
        myAnimator.SetBool("isMoving", true);
    }



    bool IsPlayerMoving() {
        bool isPlayerHasHorizontalSpeed = Mathf.Abs(inputVec.x) > Mathf.Epsilon;
        bool isPlayerHasVerticalSpeed = Mathf.Abs(inputVec.y) > Mathf.Epsilon;

        return isPlayerHasHorizontalSpeed || isPlayerHasVerticalSpeed;
    }

    private Vector2 GetPointerInput() {
        Vector3 mousePosition = pointerPosition.action.ReadValue<Vector2>();
        //mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
