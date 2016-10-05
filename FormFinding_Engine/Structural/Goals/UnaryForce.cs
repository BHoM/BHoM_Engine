using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormFinding_Engine.Base;
using BHG = BHoM.Geometry;

namespace FormFinding_Engine.Structural.Goals
{
    public class UnaryForce : IRelaxPositionGoal
    {

        public UnaryForce()
        { }

        public UnaryForce(BHG.Vector forceVector, BHG.Point position)
        {
            ForceVector = forceVector;
            Positions = new List<BHG.Point> { position };
        }

        public BHG.Vector ForceVector { get; set; }
        

        public UnaryForce(double[] force)
        {
            ForceVector = new BHG.Vector(force[0], force[1], force[2]);
        }


        public List<int> NodeIndices { get; set; }

        public List<BHG.Point> Positions
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
