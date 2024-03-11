using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Monster {
    private bool isEvasionTurn = false;
    
    private float evadeSpeed = 7.0f;
    private float attackDuration;
    private float evadeDuration;

    private void Start() {
        RuntimeAnimatorController ac = myAnimator.runtimeAnimatorController;

        foreach (AnimationClip clip in ac.animationClips) {
            if (clip.name == "Skull_Attack") {
                attackDuration = clip.length;
            }
            
            if (clip.name == "Skull_Evade") {
                evadeDuration = clip.length;
            }
        }
    }

    protected override INode SettingBT()  {
        return new SelectorNode
        (
            new List<INode>() { 
                new SelectorNode
                    (new List<INode>() {
                        new SequenceNode
                            (new List<INode>() { new ActionNode(CheckAttacking), new ActionNode(CheckEnemyWithinAttackRange), new ActionNode(DoRangeAttack) } ),
                        new SequenceNode
                            (new List<INode>() { new ActionNode(CheckEvading), new ActionNode(DoEvasion) } ),
                    } ),
                new SequenceNode 
                    (new List<INode>() { new ActionNode(CheckEnemyWithinDetectRange), new InverterNode(new ActionNode(CheckEnemyWithinAttackRange)), new ActionNode(MoveToEnemy) } ),
                new ActionNode(MoveToOriginalPosition)
            }
        );
    }

    INode.ENodeState CheckEvading() {
        if (IsAnimationRunning("Skull_Evade")) {
            return INode.ENodeState.ENS_Running;
        }
        if (isEvasionTurn)
            return INode.ENodeState.ENS_Success;

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState DoEvasion() {
        myAnimator.SetTrigger("evade");
        StartCoroutine(Evade());

        return INode.ENodeState.ENS_Success;
    }

    INode.ENodeState CheckAttacking()
    {
        if (IsAnimationRunning("Skull_Attack")) {
            return INode.ENodeState.ENS_Running;
        }

        if (!isEvasionTurn)
            return INode.ENodeState.ENS_Success;

        return INode.ENodeState.ENS_Failure;
    }

    INode.ENodeState DoRangeAttack() {
        myAnimator.SetTrigger("attack");

        StartCoroutine(Attack());
        return INode.ENodeState.ENS_Success;
    }

    IEnumerator Attack() {
        GameObject bullet = GameManager.instance.pool.Get(0);
        SkullBullet sb = bullet.GetComponent<SkullBullet>();
        sb.Set(transform.position);

        SetVelocityToZero();

        yield return new WaitForSeconds(attackDuration);
        myAnimator.SetTrigger("idle");
        isEvasionTurn = true;
    }

    IEnumerator Evade() {
        float flag = UnityEngine.Random.Range(0.0f, 1.0f);

        float theta = flag > 0.5f ? 1.0f : -1.0f;
        theta *= 60.0f;
        
        Vector2 dir = targetRigidbody.position - myRigidbody.position;
        Vector2 direction = Quaternion.AngleAxis(theta, Vector3.forward) * dir.normalized;

        myRigidbody.velocity = evadeSpeed * direction.normalized;

        yield return new WaitForSeconds(evadeDuration);

        myAnimator.SetTrigger("idle");
        SetVelocityToZero();
        isEvasionTurn = false;
    }
}
