using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using FormFinding_Engine.Base;
using BHoM.Geometry;

namespace FormFinding_Engine.Structural
{

    public class StructuralRelaxCalculator : IRelaxCalculator
    {
        private double m_dt;
        private double m_ConvergenceThreshold;
        private double m_prevEnergy;
        private double m_damping;
        private double m_maxiterations;
        // private bool m_peakEnergyReached;   //Never used
        

        public StructuralRelaxCalculator():this(0.1,0.0001,0.1,5000)
        {  }

        public StructuralRelaxCalculator(double dt, double threshold, double damping, double maxiterations)
        {
            m_prevEnergy = 0;
            m_dt = dt;
            m_ConvergenceThreshold = threshold;
            m_damping = damping;
            m_maxiterations = maxiterations;
            // m_peakEnergyReached = false;    //Never used
        }

        public double Dt
        {
            get { return m_dt; }
            set { m_dt = value; }
        }

        public double ConvergenceThreshold
        {
            get { return m_ConvergenceThreshold; }
            set { m_ConvergenceThreshold = value; }
        }

        public void CalculateChangeRate(List<RelaxNode> nodeData)
        {

            //Parallel.For(0, nodeData.Count, i
            //     =>
            //{

            //    double[] force = nodeData[i].Force();
            //    double[] acc = new double[nodeData[i].Acceleration().Length];
            //    double mass = nodeData[i].Mass();

            //    for (int j = 0; j < force.Length; j++)
            //    {
            //        acc[j] = force[j] / mass * m_dt;   // Should it really be a += here? if we want to add forces together maybe that should be done somewhere else?
            //    }

            //    double dir = VectorUtils.DotProduct(acc, nodeData[i].Acceleration());

            //    double[] vel = nodeData[i].Velocity();
            //    for (int k = 0; k < acc.Length; k++)
            //    {
            //        if (true)//(m_peakEnergyReached)
            //            vel[k] = (vel[k] + acc[k] * m_dt) * (1 - m_damping);
            //        else
            //            vel[k] = (vel[k] + acc[k] * m_dt);

            //       // if (dir < 0)

            //    }

            //    nodeData[i].SetAcceleration(acc);
            //    nodeData[i].SetVelocity(vel);

            //});


            for (int i = 0; i < nodeData.Count; i++)
            {
                double[] force = nodeData[i].Force();
                double[] acc = nodeData[i].Acceleration();
                double mass = nodeData[i].Mass();

                for (int j = 0; j < force.Length; j++)
                {
                    acc[j] = force[j] / mass * m_dt;   // Should it really be a += here? if we want to add forces together maybe that should be done somewhere else?
                }

                double[] vel = nodeData[i].Velocity();
                for (int k = 0; k < acc.Length; k++)
                {
                    vel[k] = (vel[k] + acc[k] * m_dt) * (1 - m_damping);
                }

                nodeData[i].SetAcceleration(acc);
                nodeData[i].SetVelocity(vel);

            }


        }
        public void CalculateDisplacement(List<RelaxNode> nodeData)
        {

            //Parallel.For(0, nodeData.Count, i
            //    =>
            //{
            //    double[] vel = nodeData[i].Velocity();
            //    double[] disp = nodeData[i].Displacement();

            //    for (int j = 0; j < vel.Length; j++)
            //    {

            //        disp[j] += vel[j] * m_dt;
            //    }

            //    nodeData[i].Data[NodeProps.DISP] = disp;

            //});

            for (int i = 0; i < nodeData.Count; i++)
            {
                double[] vel = nodeData[i].Velocity();
                double[] disp = nodeData[i].Displacement();

                for (int j = 0; j < vel.Length; j++)
                {

                    disp[j] += vel[j] * m_dt;
                }

                nodeData[i].Data[NodeProps.DISP] = disp;
            }

        }

        public void CalculateEnergy(List<RelaxNode> nodeData)
        {
            
            double scalarvel;

            double enrg = 0;
            for (int i = 0; i < nodeData.Count; i++)
            {
                double[] vel = nodeData[i].Velocity();
                double mass = nodeData[i].Mass();
                scalarvel = ArrayUtils.LengthSq(vel);
                enrg += scalarvel * mass;
            }

            if (enrg < m_prevEnergy)
            {
                for (int i = 0; i < nodeData.Count; i++)
                {
                    nodeData[i].SetVelocity(new double[nodeData[i].Velocity().Length]);
                }
                // m_peakEnergyReached = true;   //Never used
                m_prevEnergy = 0;
            }
            else
                m_prevEnergy = enrg;
        }


        public bool CheckConvergence(List<RelaxNode> nodeData, int iterations)
        {

            if (iterations < 1)
                return false;

            if (iterations > m_maxiterations)
                return true;

            double scalarvel = 0;
            double maxVel = 0;
            
            double[] lngts = new double[nodeData.Count];

            for (int i = 0; i < nodeData.Count; i++)
            {
                double[] vel = nodeData[i].Velocity();
                scalarvel = ArrayUtils.Length(vel);
                maxVel = Math.Max(scalarvel, maxVel);
            }
            
            
            if (maxVel < m_ConvergenceThreshold)
                return true;

            return false;
        }

        public Dictionary<string, double[]> InitiateDictionary(int dimensions)
        {
            Dictionary<string, double[]> nodeDict = new Dictionary<string, double[]>();

            nodeDict.Add(NodeProps.POS, new double[dimensions]);
            nodeDict.Add(NodeProps.ACC, new double[dimensions]);
            nodeDict.Add(NodeProps.ENRG, new double[1]);
            nodeDict.Add(NodeProps.DISP, new double[dimensions]);
            nodeDict.Add(NodeProps.MASS, new double[1]);
            nodeDict.Add(NodeProps.VEL, new double[dimensions]);
            nodeDict.Add(NodeProps.FORCE, new double[dimensions]);

            return nodeDict;
        }

        public void SetUpIteration(List<RelaxNode> nodeData)
        {

            Parallel.For(0, nodeData.Count, i
    =>
            {
                nodeData[i].SetForce(new double[nodeData[i].Force().Length]);

            });

            //for (int i = 0; i < nodeData.Count; i++)
            //{
            //    nodeData[i].SetForce(new double[nodeData[i].Force().Length]);
            //}
        }
    }
}
