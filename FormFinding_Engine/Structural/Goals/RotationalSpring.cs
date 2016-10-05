//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using FormFinding_Engine.Base;
//using BHoM.Geometry;

//namespace FormFinding_Engine.Structural.Goals
//{
//    public class RotationalSpring : IRelaxPositionGoal
//    {
//        private double m_initialAngle;
//        private double m_rotationalStiffness;
//        private double[] m_v1;
//        private double[] m_v2; 

//        public List<int> NodeIndices
//        {
//            get; set;
//        }

//        public List<Point> Positions
//        {
//            get;
//            private set;
//        }

//        public RotationalSpring(Point startPoint, Point midPoint, Point endPoint, double rotationalStiffness)
//        {
//            Positions = new List<Point> { startPoint, midPoint, endPoint };
//            m_rotationalStiffness = rotationalStiffness;
//            m_v1 = VectorUtils.Sub(startPoint, midPoint);
//            m_v2 = VectorUtils.Sub(endPoint, midPoint);
//            m_initialAngle = VectorUtils.Angle(m_v1, m_v2);
            
//        }

//        //public RotationalSpring(Line line, double rotationalStiffness) : this(line.StartPoint, line.EndPoint, rotationalStiffness)
//        //{ }

//        public void CalcForces(List<RelaxNode> nodeData)
//        {
//            double newAngle;

//            double[] newStPos = nodeData[NodeIndices[0]].NewPosition();
//            double[] newMidPos = nodeData[NodeIndices[1]].NewPosition();
//            double[] newEndPos = nodeData[NodeIndices[2]].NewPosition();

//            double[] newV1 = VectorUtils.Sub(newStPos, newMidPos);
//            double[] newV2 = VectorUtils.Sub(newEndPos, newMidPos);
//            newAngle = VectorUtils.Angle(newV1, newV2);

//            double torque = (m_initialAngle - newAngle) / newAngle * m_rotationalStiffness;

//            // The size of the different forces is calculated

//            double startPtForceSize = torque / VectorUtils.Length(newV1);
//            double endPtForceSize = torque / VectorUtils.Length(newV2);
//            double midPtForceSize = startPtForceSize + endPtForceSize;

//            // The direction of the different forces is calculated


//            double[] normalVector = VectorUtils.CrossProduct(newV1, newV2);
//            double[] startPtForceDir = VectorUtils.CrossProduct(newV1, normalVector);
//            double[] endPtForceDir = VectorUtils.CrossProduct(normalVector, newV2);
//            double[] midPtForceDir = VectorUtils.Add(startPtForceDir, endPtForceDir);

//            // Normalizing force directions

//            double[] startPtForceDirNorm = VectorUtils.Normalise(startPtForceDir);
//            double[] endPtForceDirNorm = VectorUtils.Normalise(endPtForceDir);
//            double[] midPtForceDirNorm = VectorUtils.Normalise(midPtForceDir);

//            // Final Force Vectors
//            double[] startPtForce = VectorUtils.Multiply(startPtForceDirNorm, startPtForceSize);
//            double[] endPtForce = VectorUtils.Multiply(endPtForceDirNorm, endPtForceSize);
//            double[] midPtForce = VectorUtils.Multiply(midPtForceDirNorm, midPtForceSize);


//            // Set forces to nodes

//            double[] force_0 = nodeData[NodeIndices[0]].Force();
//            double[] force_1 = nodeData[NodeIndices[1]].Force();
//            double[] force_2 = nodeData[NodeIndices[2]].Force();

//            for (int j = 0; j < force_0.Length; j++)
//            {
//                force_0[j] += startPtForce[j];
//                force_1[j] += midPtForce[j];
//                force_2[j] += endPtForce[j];

//            }

//            nodeData[NodeIndices[0]].SetForce(force_0);

//            nodeData[NodeIndices[1]].SetForce(force_1);

//            nodeData[NodeIndices[2]].SetForce(force_2);

//        }

//        public double[] Result()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

