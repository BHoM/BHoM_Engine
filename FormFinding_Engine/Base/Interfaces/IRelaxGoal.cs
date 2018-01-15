using System.Collections.Generic;

namespace FormFinding_Engine.Base
{
    public interface IRelaxGoal
    {
        //List<int> NodeIndices { get; set; }
        void CalcForces(List<RelaxNode> nodeData);
        /// <summary>
        /// Result for external use. Should not be used in the internal iterative process
        /// </summary>
        double[] Result();
    }
}
