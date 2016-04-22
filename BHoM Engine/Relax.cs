using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM;

namespace BHoM_Engine
{
    public class Relax
    {
        // <summary>
        /// Runs the relaxation
        /// </summary>


        public void Run()
        {
            //1. Reset Nodel forces (can be made when moving them)
            _structure.ResetNodalForces();

            //hack
            if (_inflationForceValue != 0)
            {
                //SmartTool.mainMutex.WaitOne();
                _structure.CalculateNormalsAndArea();
                _structure.AreasCalculated = true;
                //SmartTool.mainMutex.ReleaseMutex();
            }


            //2. Calculate Panel force (can be added to nodes at the same time, yes it should!)

            _structure.CalculatePanelForces();

            //3. Calculate Bar forces (can be added to nodes at the same time)
            _structure.CalculateBarForces(_timeStep);

            //4. Add nodal forces
            _structure.CalcNodalForces(_timeStep);

            //5. Calculate movement. Protect with mutex when move.
            _structure.CalculateNodalAccelerations(_pointConst, _timeStep);
            _structure.CalculateNodalVelocities(_carryOver, _timeStep);

            // SmartTool.mainMutex.WaitOne();
            _structure.CalculateNodalCoordinates(_timeStep, _stabilityControl, _pointConst, _maxMoveDist);
            ApplyBoundaryConstraints();

            //mbConverged = IsConverged();
        }






        /** Calculates the bounding box for the geometry
        *
        */
        public void CalcSafeDynamicTimeStep()
        {
            if (_structure == null || _structure.GetEdges().Count == 0) { return; }

            double ts1;
            double ts2;
            double newTimeStep;
            double tempStiff = 0;
            double stiffest = 0;
            double lightest;

            BHoM.Structural.Bar stiffestBar = _structure.FindStiffestBar();
            BHoM.Structural.Node lightestNode = _structure.FindLightestNode();

            //Find the stiffest bar connected to the node.
            foreach (BHoM.Structural.Bar bar in lightestNode.GetRingEdges())
            {
                tempStiff = bar.Stiffness * bar.StiffnessGlobalScaleFactor * bar.StiffnessCustomScaleFactor;
                if (tempStiff > stiffest)
                    stiffest = tempStiff;
            }
            ts1 = Math.Sqrt(2 * lightestNode.Mass / tempStiff);

            //Find the lightest node connected to the stiffest bar.
            if (stiffestBar.GetStart().Mass > stiffestBar.GetEnd().Mass)
                lightest = stiffestBar.GetStart().Mass;
            else
                lightest = stiffestBar.GetEnd().Mass;

            ts2 = Math.Sqrt(2 * lightest / (stiffestBar.Stiffness * stiffestBar.StiffnessGlobalScaleFactor * stiffestBar.StiffnessCustomScaleFactor));

            if (ts1 < ts2)
                newTimeStep = ts1;
            else
                newTimeStep = ts2;

            _safeTimeStep = newTimeStep;
        }

        /// <summary>
        /// Calculate the timestep intervall dynamically based on the node mass and the bar stiffness. 
        /// </summary>
        public void CalcDynamicMaxTimeStep()
        {
            List<double> stiffnessValues = new List<double>(_structure.GetBars().Count);
            List<double> massValues = new List<double>(_structure.GetNodes().Count);

            double tempStiffness;
            double medianStiffness;
            double medianMass;

            foreach (BHoM.Structural.Bar bar in _structure.GetBars())
            {
                tempStiffness = bar.Stiffness * bar.StiffnessGlobalScaleFactor * bar.StiffnessCustomScaleFactor;
                stiffnessValues.Add(tempStiffness);
            }
            foreach (BHoM.Structural.Node node in _structure.GetNodes())
            {
                massValues.Add(node.Mass);
            }

            //Get max, min and median stiffness
            //Sort(ref stiffnessValues);
            SortNew(ref stiffnessValues);
            medianStiffness = stiffnessValues[(int)Math.Round((double)_structure.GetBars().Count / 2, 0)];

            //Get max, min and median mass
            //Sort(ref massValues);
            SortNew(ref massValues);
            medianMass = massValues[(int)Math.Round((double)_structure.GetNodes().Count / 2, 0)];

            //Current timestep value is based on the smallest mass and the maximum stiffness.
            _maxTimeStep = 0.4 * Math.Sqrt((2 * medianMass) / medianStiffness);
        }

        public void UpdateTimeStep()
        {
            if (_autoTimeStep)
                _timeStep = 0.35 * _safeTimeStep;
            else
                _timeStep = _maxTimeStep * _timeStepScaleFactor;
        }
    }
}
