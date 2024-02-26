using System.Collections.Generic;

public class SequenceNode : INode {
    List<INode> myChilds;

    public SequenceNode(List<INode> childs) {
        myChilds = childs;
    }

    public INode.ENodeState Evaluate() {
        if (myChilds == null || myChilds.Count == 0) 
            return INode.ENodeState.ENS_Failure;

        foreach (INode child in myChilds) {
            switch (child.Evaluate())
            {
                case INode.ENodeState.ENS_Running:
                    return INode.ENodeState.ENS_Running;
                case INode.ENodeState.ENS_Success:
                    continue;
                case INode.ENodeState.ENS_Failure:
                    return INode.ENodeState.ENS_Failure;
            }
        }

        return INode.ENodeState.ENS_Success; 
    }
}
