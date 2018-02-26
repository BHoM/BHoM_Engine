using System.Collections.Generic;

namespace FormFinding_Engine.Base
{
    public interface IRelaxCalculator
    {
        Dictionary<string, double[]> InitiateDictionary(int dimensions);
        void SetUpIteration(List<RelaxNode> nodeData);
        void CalculateChangeRate(List<RelaxNode> nodeData);
        void CalculateDisplacement(List<RelaxNode> nodeData);
        void CalculateEnergy(List<RelaxNode> nodeData);
        bool CheckConvergence(List<RelaxNode> nodeData, int iterations);
    }
}
