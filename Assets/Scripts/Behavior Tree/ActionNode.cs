using System;

public sealed class ActionNode : INode {
    Func<INode.ENodeState> myAction = null;

    public ActionNode(Func<INode.ENodeState> inAction) {
        myAction = inAction;
    }

    public INode.ENodeState Evaluate() => myAction?.Invoke() ?? INode.ENodeState.ENS_Failure;
}
