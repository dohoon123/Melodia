using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeRunner {
    INode myRootNode;
    public BehaviorTreeRunner(INode rootNode) {
        myRootNode = rootNode;
    }

    public void Operate() {
        myRootNode.Evaluate();
    }
}
