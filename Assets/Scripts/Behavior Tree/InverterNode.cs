using System.Collections.Generic;

public class InverterNode : INode {
    INode myChild;

    public InverterNode(INode child) {
        myChild = child;
    }

    public INode.ENodeState Evaluate() {
        if (myChild == null) 
            return INode.ENodeState.ENS_Failure;

        switch (myChild.Evaluate())
        {
            case INode.ENodeState.ENS_Running:
                return INode.ENodeState.ENS_Running;
            case INode.ENodeState.ENS_Success:
                return INode.ENodeState.ENS_Failure;;
            case INode.ENodeState.ENS_Failure:
                return INode.ENodeState.ENS_Success;
        }
        return INode.ENodeState.ENS_Failure; 
    }
}
