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
        public static void SetBarData(List<Bar> bars, List<double> barStiffnesses, List<double> lengthMultiplier, List<double> prestresses)
        {
            for (int i = 0; i < bars.Count; i++)
            {
                if (bars[i].Name != null)
                    bars[i].CustomData.Add("SecType", bars[i].Name);
                else
                    bars[i].CustomData.Add("SecType", 0.050);

                //try
                //{
                //    bars[i].CustomData.Add("lengthMultiplier", lengthMultiplier[i]);
                //}
                //catch
                //{
                //    bars[i].CustomData.Add("lengthMultiplier", 1);
                //}

                try
                {
                    bars[i].CustomData.Add("prestress", prestresses[i]);
                }
                catch
                {
                    bars[i].CustomData.Add("prestress", 0);
                }

                //bars[i].CustomData.Add("Stiffness", barStiffnesses[i]);
                bars[i].CustomData.Add("StartLength", bars[i].Length);

                BHoM.Materials.Material material = new BHoM.Materials.Material("GalvLockedCoilCables", BHoM.Materials.MaterialType.Steel, 165000000000, 1, 1, 1, 8250);
                bars[i].Material = material;
            }
        }

        public static Structure SetStructure(List<Bar> bars, List<Node> lockedPts, List<double> barStiffnesses, List<double> prestresses, List<double> lengthMultiplier, bool restrainXY, double treshold)
            {

            SetBarData(bars, barStiffnesses, lengthMultiplier, prestresses);

            Structure structure = new Structure(bars);

            structure.c = 0.95;  //Which is the damping constant for viscous damping.
            structure.dt = 0.001;  //Is the time step.
            structure.t = 0; //startTime
            structure.nodeTol = 0.1;  //Tolerance, used for position comparision
            structure.treshold = treshold;
            List<double> nodeMass = new List<double>() { 1.0 };

            //structure.SegmentStructure(6);
            //structure.CalcSlackLength();
            //structure.SetBarStiffness();
            structure.SetMassPerMetre();

            structure.SetNodeMass();
            structure.SetStartVelocity();
            structure.FindLockedNodes(lockedPts);
            if (restrainXY)
                structure.RestrainXY();

            structure.SetCustomStriffnessScaleFactor();
            structure.SetGlobalStriffnessScaleFactor();

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

           // structure.CalcSafeDynamicTimeStep();
        }
    }

}

