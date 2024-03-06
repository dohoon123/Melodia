using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : Monster {
    private bool isEvasionTurn = false;
    
    private float evadeSpeed = 7.0f;
    private float attackDuration;
    private float evadeDuration;

    private void Start() {
        detectRange = 20.0f;
        attackRange = 10.0f;

        RuntimeAnimatorController ac = myAnimator.runtimeAnimatorController;

        foreach (AnimationClip clip in ac.animationClips) {
            if (clip.name == "Skull_Attack") {
                Debug.Log("Skull_Attack exist!");
                attackDuration = clip.length;
            }
            
            if (clip.name == "Skull_Evade") {
                Debug.Log("Skull_Evade exist!");
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
                    (new List<INode>() { new ActionNode(CheckEnemyWithinDetectRange), new ActionNode(MoveToEnemy) } ),
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
        // do sth
        yield return new WaitForSeconds(attackDuration);
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
        isEvasionTurn = false;
    }
}
