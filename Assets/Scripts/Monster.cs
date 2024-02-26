using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{

    public float moveSpeed = 2.0f;
    public Rigidbody2D targetRigidbody;
    float detectRange = 5.0f;
    float attackRange = 2.0f;

    float previousPositionX;

    Rigidbody2D myRigidbody;
    SpriteRenderer mySpriter;
    Animator myAnimator;

    //bool isAlive = true;
    bool isMoving = false;

    Vector2 originPosition;
    BehaviorTreeRunner BTRunner = null;

    void Awake() {
        myRigidbody = GetComponent<Rigidbody2D>();
        mySpriter = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();

        originPosition = transform.position;
        BTRunner = new BehaviorTreeRunner(SettingBT());

        previousPositionX = transform.position.x;
    }

    void FixedUpdate() {
        SetVelocityToZero();
        BTRunner.Operate();
        FlipSprite();
        SetMovingAnimation();
        SetPreviousPositionX();
    }

    void SetVelocityToZero() {
        myRigidbody.velocity = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other) {
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

    INode SettingBT() {
        return new SelectorNode
        (
            new List<INode>() { 
                new SequenceNode
                    (new List<INode>() { new ActionNode(CheckMeleeAttacking), new ActionNode(CheckEnemyWithinAttackRange), new ActionNode(DoMeleeAttack) } ),
                new SequenceNode 
                    (new List<INode>() { new ActionNode(CheckEnemyWithinDetectRange), new ActionNode(MoveToEnemy) } ),
                new ActionNode(MoveToOriginalPosition)
            }
        );
    }

    void FlipSprite() {
        float currentPositionX = transform.position.x;
        if (Mathf.Abs(currentPositionX - previousPositionX) > float.Epsilon) {
            mySpriter.flipX = previousPositionX < currentPositionX;
        }
    }
    bool IsAnimationRunning(string stateName)
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

    INode.ENodeState CheckMeleeAttacking()
    {
        if (IsAnimationRunning("attack")) {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    INode.ENodeState DoMeleeAttack() {
        myAnimator.SetTrigger("attack");
        return INode.ENodeState.ENS_Success;
    }

    INode.ENodeState CheckEnemyWithinAttackRange() {
        if (Vector2.SqrMagnitude(new Vector2(transform.position.x, transform.position.y) - targetRigidbody.position) < attackRange * attackRange) {
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState CheckEnemyWithinDetectRange() {
        if (Vector2.SqrMagnitude(new Vector2(transform.position.x, transform.position.y) - targetRigidbody.position) < detectRange * detectRange) {
            return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState MoveToEnemy() {
        Vector2 currentPosition = transform.position;

        if (Vector2.SqrMagnitude(currentPosition - targetRigidbody.position) < attackRange * attackRange) {
            return INode.ENodeState.ENS_Success;
        }

        Vector2 dir = targetRigidbody.position - myRigidbody.position;
        dir = moveSpeed * Time.fixedDeltaTime * dir.normalized;
        myRigidbody.MovePosition(myRigidbody.position + dir);

        return INode.ENodeState.ENS_Running;
    }

    INode.ENodeState MoveToOriginalPosition() {
        Vector2 currentPosition = transform.position;

        if (Vector2.SqrMagnitude(originPosition - currentPosition) < 0.01f) {
            myAnimator.ResetTrigger("attack");
            return INode.ENodeState.ENS_Success;
        }

        Vector2 dir = originPosition - myRigidbody.position;

        dir = moveSpeed * Time.fixedDeltaTime * dir.normalized;
        myRigidbody.MovePosition(myRigidbody.position + dir);

        return INode.ENodeState.ENS_Running;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, detectRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(this.transform.position, attackRange);
    }
}
