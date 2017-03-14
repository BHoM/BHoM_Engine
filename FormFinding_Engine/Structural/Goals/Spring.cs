using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormFinding_Engine.Base;
using BHoM.Geometry;

namespace FormFinding_Engine.Structural.Goals
{
    public class Spring : IRelaxPositionGoal
    {
        private double m_initialLength;
        private double m_stiffness;
        private double m_result;

        public List<int> NodeIndices
        {
            get; set;
        }

        public List<Point> Positions
        {
            get;
            private set;
        }

        public Spring(Point startPoint, Point endPoint, double stiffness)
        {
            Positions = new List<Point> { startPoint, endPoint };
            m_stiffness = stiffness;
            m_initialLength = startPoint.DistanceTo(endPoint);
        }

        public Spring(Line line, double stiffness) : this(line.StartPoint, line.EndPoint, stiffness)
        { }
        
        
        public void CalcForces(List<RelaxNode> nodeData)
            
        {
            double newLength;

            double[] newStPos = nodeData[NodeIndices[0]].NewPosition();
            double[] newEndPos = nodeData[NodeIndices[1]].NewPosition();


            double[] springVector = ArrayUtils.Sub(newEndPos, newStPos);

            newLength = ArrayUtils.Length(springVector);

            double stretchFactor = (m_initialLength - newLength) / newLength * m_stiffness;

            double[] springForce = ArrayUtils.Multiply(springVector, stretchFactor);


            //---------------------------------------------------------------------------------------------------------
            // force on startnode
           
            double[] force_0 = nodeData[NodeIndices[0]].Force();

            for (int j = 0; j < force_0.Length; j++)
            {
                force_0[j] -= springForce[j];
            }

            nodeData[NodeIndices[0]].SetForce(force_0);

            //---------------------------------------------------------------------------------------------------------
            // force on endnode
            double[] force_1 = nodeData[NodeIndices[1]].Force();

            for (int j = 0; j < force_1.Length; j++)
            {
                force_1[j] += springForce[j];
            }

            nodeData[NodeIndices[1]].SetForce(force_1);

            // Store result

            m_result = ArrayUtils.Length(springForce);

        }

        public double[] Result()
        {
            return new double[] { m_result };
        }
    }
}
