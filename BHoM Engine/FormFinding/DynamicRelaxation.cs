using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Structural;
using BHoM.Structural.Results.Bars;
using BHoM.Structural.Results.Nodes;
using BHoM;

namespace BHoM_Engine.FormFinding
{
        public class DynamicRelaxation
        {
        public static void SetBarData(List<Bar> bars, List<double> areas, List<double> densities, List<double> eModulus, List<double> prestresses)
        {
            for (int i = 0; i < bars.Count; i++)
            {
                bars[i].CustomData.Add("StartLength", bars[i].Length);

                bars[i].CustomData.Add("Area", areas[i]);

                bars[i].CustomData.Add("Density", densities[i]);

                bars[i].CustomData.Add("E", eModulus[i]);

                bars[i].CustomData.Add("Prestress", prestresses[i]);

            }
        }

        public static Structure SetStructure(List<Bar> bars, List<Node> lockedPts, List<double> areas, List<double> densities, List<double> eModules, List<double> prestresses, bool restrainXY, double treshold)
            {

            SetBarData(bars, areas, densities, eModules, prestresses);

            Structure structure = new Structure(bars);

            structure.c = 0.95;
            structure.dt = 0.1;  
            structure.t = 0; 
            structure.nodeTol = 0.1;  
            structure.treshold = treshold;
            List<double> nodeMass = new List<double>() { 1.0 };

            structure.SetMassPerMetre();
            structure.SetStiffness();

            //structure.SetLumpedNodeMass();
            structure.SetFictionalNodeMass();
            structure.SetStartVelocity();
            structure.FindLockedNodes(lockedPts);
            if (restrainXY)
                structure.RestrainXY();

            //structure.CalcSafeTimeStep();

            return structure;
            }

        public static void RelaxStructure(Structure structure, double gravity)
        {
            structure.t += structure.dt;

            structure.CalcBarForce();

            structure.CalcNodeForce(gravity);          

            structure.UpdateNodeMass();

            structure.CalcAcceleration();

            structure.CalcVelocity();

            structure.SetLockedToZero();

            structure.CalcTranslation();

            structure.SetRestrainedTranslationsToZero();

            structure.UpdateGeometry();

            structure.CalcKineticEnergy();
        }
    }

}

