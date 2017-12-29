using System.Collections.Generic;
using FormFinding_Engine.Base;

namespace FormFinding_Engine.Structural
{
    public interface IRelaxMass : IRelaxPosition
    {
        void ApplyMass(List<RelaxNode> nodeData);
    }
}
