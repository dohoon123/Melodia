using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Rigidbody2D targetRigidbody;
    protected float detectRange = 5.0f;
    protected float attackRange = 2.0f;

    protected float previousPositionX;

    protected Rigidbody2D myRigidbody;
    protected SpriteRenderer mySpriter;
    protected Animator myAnimator;

    //bool isAlive = true;
    protected bool isMoving = false;

    protected Vector2 originPosition;
    protected BehaviorTreeRunner BTRunner = null;

    void Awake() {
        myRigidbody = GetComponent<Rigidbody2D>();
        mySpriter = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        
        originPosition = transform.position;
        BTRunner = new BehaviorTreeRunner(SettingBT());

        previousPositionX = transform.position.x;
    }

    void FixedUpdate() {
        BTRunner.Operate();
        FlipSprite();
        SetMovingAnimation();
        SetPreviousPositionX();
    }

    abstract protected INode SettingBT();

    protected void SetVelocityToZero() {
        myRigidbody.velocity = Vector2.zero;
    }

    void SetPreviousPositionX() {
        previousPositionX = transform.position.x;
    }

    private void SetMovingAnimation() {
        isMoving = Mathf.Abs(previousPositionX - transform.position.x) > float.Epsilon;
        if (isMoving) {
            myAnimator.SetBool("isMoving", true);
        }else {
            myAnimator.SetBool("isMoving", false);
        }
    }


    void FlipSprite() {
        float currentPositionX = transform.position.x;
        if (Mathf.Abs(currentPositionX - previousPositionX) > float.Epsilon) {
            mySpriter.flipX = previousPositionX > currentPositionX;
        }
    }
    protected bool IsAnimationRunning(string stateName)
    {
        if(myAnimator != null)
        {
            if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                var normalizedTime = myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                return normalizedTime != 0 && normalizedTime < 1f;
            }
        }

        return false;
    }

    protected INode.ENodeState CheckEnemyWithinAttackRange() {
        if (Vector2.SqrMagnitude(new Vector2(transform.position.x, transform.position.y) - targetRigidbody.position) < attackRange * attackRange) {
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }

    protected INode.ENodeState CheckEnemyWithinDetectRange() {
        if (Vector2.SqrMagnitude(new Vector2(transform.position.x, transform.position.y) - targetRigidbody.position) < detectRange * detectRange) {
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }

    protected INode.ENodeState MoveToEnemy() {
        Vector2 currentPosition = transform.position;

        if (Vector2.SqrMagnitude(currentPosition - targetRigidbody.position) < attackRange * attackRange) {
            return INode.ENodeState.ENS_Success;
        }

        Vector2 dir = targetRigidbody.position - myRigidbody.position;
        myRigidbody.velocity = moveSpeed * dir.normalized;

        return INode.ENodeState.ENS_Running;
    }

    protected INode.ENodeState MoveToOriginalPosition() {
        Vector2 currentPosition = transform.position;

        if (Vector2.SqrMagnitude(originPosition - currentPosition) < 0.01f) {
            myAnimator.ResetTrigger("attack");
            SetVelocityToZero();
            return INode.ENodeState.ENS_Success;
        }

        Vector2 dir = originPosition - myRigidbody.position;
        myRigidbody.velocity = moveSpeed * dir.normalized;

        return INode.ENodeState.ENS_Running;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, detectRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }

}
