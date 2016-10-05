using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormFinding_Engine.Base;
using BHoM.Structural.Elements;
using BHoM.Geometry;

namespace FormFinding_Engine.Structural
{
    public class NodeConstraint : IRelaxPositionBC
    {
        public NodeConstraint(List<Point> constrainedNodes)
        {
            Positions = constrainedNodes;
        }

        public List<int> NodeIndices { get; set; }

        public List<Point> Positions { get; set; }

        public void ApplyConstraint(List<RelaxNode> nodeData)
        {
            foreach (int i in NodeIndices)
            {
                double[] vel = nodeData[i].Velocity();
                    
                for (int j = 0; j < vel.Length; j++)
                {
                    vel[j] = 0;
                }

                nodeData[i].SetVelocity(vel);
            }
        }
    }
}
