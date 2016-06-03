using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Structural;
using BHoM.Structural.Results.Bars;
using BHoM.Structural.Results.Nodes;

namespace BHoM_Engine.FormFinding
{
        public class DynamicRelaxation
        {
        public static void Relax(List<Line> lines, List<Point> lockedPts, List<double> gravityLoads, List<double> barStiffnesses, List<double> lengthMultiplier, int maxNoIt, double treshold)
        {


            BarForceCollection barForceCollection = new BarForceCollection();
            NodalResultCollection nodalResultCollection = new NodalResultCollection();

            Structure structure = new Structure(lines);

            structure.c = 0.95;  //Which is the damping constant for viscous damping.
            structure.dt = 0.05;  //Is the time step.
            structure.t = 0; //startTime
            structure.nodeTol = 0.1;  //Tolerance, used for position comparision
            List<double> nodeMass = new List<double>() { 1.0 };
            int counter = 1;
            int maxNoOfIterations = maxNoIt;

            structure.CalcSlackLength(lengthMultiplier);
            structure.FindLockedNodes(lockedPts);
            structure.SetNodeMass(nodeMass);
            structure.SetStartVelocity();
            structure.SetBarStiffness(barStiffnesses);

            foreach (Node node in structure.Nodes)
                node.CustomData.Add("KineticEnergy", 0);


            for (int i = 0; i < maxNoOfIterations; i++)
            {
                lines.Clear();

                structure.t += structure.dt;

                structure.CalcBarForce();

                structure.CalcNodeForce(gravityLoads);

                structure.SetLockedToZero();

                structure.CalcAcceleration();

                structure.CalcVelocity();

                structure.CalcTranslation();

                structure.UpdateGeometry();

                structure.CalcKineticEnergy();

                foreach (Bar bar in structure.Bars)
                    lines.Add(bar.Line);



                if (structure.HasConverged())
                    break;
                else
                    counter++;
            }

        }

        public static Structure SetStructure(List<Line> lines, List<Point> lockedPts, List<double> barStiffnesses, List<double> lengthMultiplier, double treshold)
            {
            Structure structure = new Structure(lines);

            structure.c = 0.95;  //Which is the damping constant for viscous damping.
            structure.dt = 0.05;  //Is the time step.
            structure.t = 0; //startTime
            structure.nodeTol = 0.1;  //Tolerance, used for position comparision
            structure.treshold = treshold;
            List<double> nodeMass = new List<double>() { 1.0 };

            structure.CalcSlackLength(lengthMultiplier);
            structure.FindLockedNodes(lockedPts);
            structure.SetNodeMass(nodeMass);
            structure.SetStartVelocity();
            structure.SetBarStiffness(barStiffnesses);

            return structure;
            }

        public static void RelaxStructure(Structure structure, List<double> gravityLoads)
        {
            structure.t += structure.dt;

            structure.CalcBarForce();

            structure.CalcNodeForce(gravityLoads);

            structure.SetLockedToZero();

            structure.CalcAcceleration();

            structure.CalcVelocity();

            structure.CalcTranslation();
        
            structure.UpdateGeometry();

            structure.CalcKineticEnergy();
        }
    }






        ///** Calculates the bounding box for the geometry
        //*
        //*/
        //public void CalcSafeDynamicTimeStep()
        //{
        //    if (_structure == null || _structure.GetEdges().Count == 0) { return; }

        //    double ts1;
        //    double ts2;
        //    double newTimeStep;
        //    double tempStiff = 0;
        //    double stiffest = 0;
        //    double lightest;

        //    BHoM.Structural.Bar stiffestBar = _structure.FindStiffestBar();
        //    BHoM.Structural.Node lightestNode = _structure.FindLightestNode();

        //    //Find the stiffest bar connected to the node.
        //    foreach (BHoM.Structural.Bar bar in lightestNode.GetRingEdges())
        //    {
        //        tempStiff = bar.Stiffness * bar.StiffnessGlobalScaleFactor * bar.StiffnessCustomScaleFactor;
        //        if (tempStiff > stiffest)
        //            stiffest = tempStiff;
        //    }
        //    ts1 = Math.Sqrt(2 * lightestNode.Mass / tempStiff);

        //    //Find the lightest node connected to the stiffest bar.
        //    if (stiffestBar.GetStart().Mass > stiffestBar.GetEnd().Mass)
        //        lightest = stiffestBar.GetStart().Mass;
        //    else
        //        lightest = stiffestBar.GetEnd().Mass;

        //    ts2 = Math.Sqrt(2 * lightest / (stiffestBar.Stiffness * stiffestBar.StiffnessGlobalScaleFactor * stiffestBar.StiffnessCustomScaleFactor));

        //    if (ts1 < ts2)
        //        newTimeStep = ts1;
        //    else
        //        newTimeStep = ts2;

        //    _safeTimeStep = newTimeStep;
        //}

        ///// <summary>
        ///// Calculate the timestep intervall dynamically based on the node mass and the bar stiffness. 
        ///// </summary>
        //public void CalcDynamicMaxTimeStep()
        //{
        //    List<double> stiffnessValues = new List<double>(_structure.GetBars().Count);
        //    List<double> massValues = new List<double>(_structure.GetNodes().Count);

        //    double tempStiffness;
        //    double medianStiffness;
        //    double medianMass;

        //    foreach (BHoM.Structural.Bar bar in _structure.GetBars())
        //    {
        //        tempStiffness = bar.Stiffness * bar.StiffnessGlobalScaleFactor * bar.StiffnessCustomScaleFactor;
        //        stiffnessValues.Add(tempStiffness);
        //    }
        //    foreach (BHoM.Structural.Node node in _structure.GetNodes())
        //    {
        //        massValues.Add(node.Mass);
        //    }

        //    //Get max, min and median stiffness
        //    //Sort(ref stiffnessValues);
        //    SortNew(ref stiffnessValues);
        //    medianStiffness = stiffnessValues[(int)Math.Round((double)_structure.GetBars().Count / 2, 0)];

        //    //Get max, min and median mass
        //    //Sort(ref massValues);
        //    SortNew(ref massValues);
        //    medianMass = massValues[(int)Math.Round((double)_structure.GetNodes().Count / 2, 0)];

        //    //Current timestep value is based on the smallest mass and the maximum stiffness.
        //    _maxTimeStep = 0.4 * Math.Sqrt((2 * medianMass) / medianStiffness);
        //}

        //public void UpdateTimeStep()
        //{
        //    if (_autoTimeStep)
        //        _timeStep = 0.35 * _safeTimeStep;
        //    else
        //        _timeStep = _maxTimeStep * _timeStepScaleFactor;
        //}
    }

