using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormFinding_Engine.Base
{
    public interface IRelaxBC
    {
        //List<int> NodeIndices { get; set; }
        void ApplyConstraint(List<RelaxNode> nodeData);
    }
}
