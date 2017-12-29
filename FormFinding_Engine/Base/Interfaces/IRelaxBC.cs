using System.Collections.Generic;

namespace FormFinding_Engine.Base
{
    public interface IRelaxBC
    {
        //List<int> NodeIndices { get; set; }
        void ApplyConstraint(List<RelaxNode> nodeData);
    }
}
