using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using FormFinding_Engine.Base;

namespace FormFinding_Engine.Structural
{
    public interface IRelaxMass : IRelaxPosition
    {
        void ApplyMass(List<RelaxNode> nodeData);
    }
}
