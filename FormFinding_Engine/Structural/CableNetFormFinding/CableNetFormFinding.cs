using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FormFinding_Engine.Base;
using FormFinding_Engine.Structural;
using FormFinding_Engine.Structural.Goals;

using BHoM.Geometry;




namespace FormFinding_Engine.Structural.CableNetFormFinding
{
    public static class CableNetFormFinding
    {


        public static bool Run(List<Point> crPts, List<Point> trPts, List<Vector> trLoad, List<Vector> crLoad, double goalValue, double sFac, double initialStep, double dt, double treshold, double damping, int maxiterations, out List<double> crPrestressForces, out List<double> cablePrestresses, out List<Point> ffPts, out double finalScFac)
        {

            //Hardcoded nodal mass to be used in the relaxation
            double mass = 1000;

            //Set up constraints for relaxation steps
            NodeConstraint constr = new NodeConstraint(crPts);

            //Create nodeal masses of the compressionring nodes
            List<PointMass> crMass = crPts.Select(x => new PointMass(x, mass)).ToList();

            //Run bool for outer loop
            bool run = true;

            //Iteration counter for outer loop
            int counter = 0;


            //Empte lists for ouotput variables
            List<ConstantHorizontalPrestressGoal> goals = null;
            crPrestressForces = new List<double>();
            ffPts = new List<Point>();

            //values to be used in outer looping
            double prevMinHeight = 0;
            double prevSFac = 0;

            //Main outer loop of the whole formfinding procedure. Tries to find the correct scalefactor to be used when calculating the prestress forces
            while (run)
            {
                //Max iterations in the horisontal formfinding
                int maxIter = 1000;

                List<Point> newTrPts;

                //Horisontal formfinding and creation of prestress goals
                goals = CableNetPrecalculations.HorForceCalcGenericIterative(crPts, trPts, trLoad, crLoad, sFac, out newTrPts, out crPrestressForces, maxIter);

                //Set up for relaxation
                List<UnaryForce> forces = new List<UnaryForce>();

                for (int i = 0; i < newTrPts.Count; i++)
                {
                    forces.Add(new UnaryForce(trLoad[i], newTrPts[i]));
                }

                List<PointMass> trMass = newTrPts.Select(x => new PointMass(x, mass)).ToList();

                StructuralDynamicRelaxation relaxEngine = new StructuralDynamicRelaxation(dt, treshold, damping, maxiterations);

                relaxEngine.AddGoals(forces);
                relaxEngine.AddGoals(goals);
                relaxEngine.AddMasses(crMass);
                relaxEngine.AddMasses(trMass);
                relaxEngine.AddBCs(new IRelaxPositionBC[] { constr });
                
                //Relax the TR nodes into position
                relaxEngine.Run();


                ffPts = relaxEngine.GetPoints();

                //Find the smallest Z-value of the tension ring nodes
                double minHeight = ffPts.Select(x => x.Z).OrderBy(x => x).First();

                //Check convergence and update scale factor for horisontal formfinding
                if (run = (Math.Abs(minHeight - goalValue) > 0.00000001) && (counter < 40))
                {

                    //First iteration => update scalefactor based on guessed value
                    if (counter < 1)
                    {
                        prevSFac = sFac;
                        sFac += initialStep;
                    }
                    //All other iterations => update scalefactor based on linear interpolation of the two previous values
                    else
                    {
                        double k = (minHeight - prevMinHeight) / (sFac - prevSFac);
                        double m = minHeight - k * sFac;

                        prevSFac = sFac;

                        sFac = (goalValue - m) / k;
                    }
                    
                }

                //Store previous minimum height to be used in later iterations
                prevMinHeight = minHeight;
                counter++;
            }

            //Grap prestress forces from goal objects
            cablePrestresses = goals.Select(x => x.Result()[0]).ToList();

            //Final scalefactor set for output
            finalScFac = sFac;

            return true;

        }


    }
}
