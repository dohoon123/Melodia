using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster {
    protected override INode SettingBT()  {
        return new SelectorNode
        (
            new List<INode>() { 
                new SequenceNode
                    (new List<INode>() { new ActionNode(CheckAttacking), new ActionNode(CheckEnemyWithinAttackRange), new ActionNode(DoMeleeAttack) } ),
                new SequenceNode 
                    (new List<INode>() { new ActionNode(CheckEnemyWithinDetectRange), new ActionNode(MoveToEnemy) } ),
                new ActionNode(MoveToOriginalPosition)
            }
        );
    }

    INode.ENodeState CheckAttacking()
    {
        if (IsAnimationRunning("Slime_Attack")) {
            return INode.ENodeState.ENS_Running;
        }

        return INode.ENodeState.ENS_Success;
    }

    INode.ENodeState DoMeleeAttack() {
        myAnimator.SetTrigger("attack");
        return INode.ENodeState.ENS_Success;
    }
}
