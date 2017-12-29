using System.Collections.Generic;
using FormFinding_Engine.Base;
using BH.oM.Geometry;

namespace FormFinding_Engine.Structural.Goals
{
    public class UnaryForce : IRelaxPositionGoal
    {

        public UnaryForce()
        { }

        public UnaryForce(Vector forceVector, Point position)
        {
            ForceVector = forceVector;
            Positions = new List<Point> { position };
        }

        public Vector ForceVector { get; set; }
        

        public UnaryForce(double[] force)
        {
            ForceVector = new Vector { X = force[0], Y = force[1], Z = force[2] };
        }


        public List<int> NodeIndices { get; set; }

        public List<Point> Positions
        {
            get;
            private set;
        }

        public void CalcForces(List<RelaxNode> nodeData)
        {
            
            double[] force = nodeData[NodeIndices[0]].Force();

            for (int j = 0; j < force.Length; j++)
            {
                force[j] += ((double[])ForceVector)[j];
            }

            nodeData[NodeIndices[0]].SetForce(force);

        }

        public double[] Result()
        {
            return (double[])ForceVector;
        }
    }
}
