using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormFinding_Engine.Base;
using BHoM.Geometry;

namespace FormFinding_Engine.Structural.Goals
{
    public class ConstantHorizontalPrestressGoal : IRelaxPositionGoal
    {
        private double m_prestressForce;
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

        public ConstantHorizontalPrestressGoal(Point startPoint, Point endPoint, double xy_prestressForce)
        {
            Positions = new List<Point> { startPoint, endPoint };
            m_prestressForce = xy_prestressForce;
        }

        public ConstantHorizontalPrestressGoal(Line line, double prestressForce) : this(line.StartPoint, line.EndPoint, prestressForce)
        { }



        public void CalcForces(List<RelaxNode> nodeData)

        {

            double[] stPos = nodeData[NodeIndices[0]].NewPosition();
            double[] endPos = nodeData[NodeIndices[1]].NewPosition();

            double[] vec = new double[stPos.Length];

            vec = ArrayUtils.Sub(endPos, stPos);

            double[] unitVec = ArrayUtils.Normalise(vec);

            double cableForceValue = m_prestressForce / Math.Sqrt(unitVec[0] * unitVec[0] + unitVec[1] * unitVec[1]);

            double[] cableForce = ArrayUtils.Multiply(unitVec, cableForceValue);


            // Calculate force

            //---------------------------------------------------------------------------------------------------------
            // force on startnode

            double[] force_0 = nodeData[NodeIndices[0]].Force();

            for (int j = 0; j < force_0.Length; j++)
            {
                force_0[j] += cableForce[j];
            }

            nodeData[NodeIndices[0]].SetForce(force_0);

            //---------------------------------------------------------------------------------------------------------
            // force on endnode
            double[] force_1 = nodeData[NodeIndices[1]].Force();

            for (int j = 0; j < force_1.Length; j++)
            {
                force_1[j] -= cableForce[j];
            }

            nodeData[NodeIndices[1]].SetForce(force_1);


            //Store result
            m_result = cableForceValue;
        }

        public double[] Result()
        {
            return new double[] { m_result };
        }
    }
}
