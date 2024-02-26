using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree {
    INode myRootNode;

    public BehaviourTree(INode rootNode) {
        myRootNode = rootNode;
    }

    public void Operate() {
        myRootNode.Evaluate();
    }
}
