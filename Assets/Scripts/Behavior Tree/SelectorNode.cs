using System.Collections.Generic;

public class SelectorNode : INode {
    List<INode> myChilds;

    public SelectorNode(List<INode> childs) {
        myChilds = childs;
    }

    public INode.ENodeState Evaluate() {
        if (myChilds == null) 
            return INode.ENodeState.ENS_Failure;

        foreach (var child in myChilds) {
            switch (child.Evaluate())
            {
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                case INode.ENodeState.ENS_Success:
                    return INode.ENodeState.ENS_Success;
            }
        }
        return INode.ENodeState.ENS_Failure; 
    }
}
