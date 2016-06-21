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
        public static void SetBarData(List<Bar> bars, List<double> prestresses)
        {
            for (int i = 0; i < bars.Count; i++)
            {
                if (bars[i].Name != null)
                    bars[i].CustomData.Add("SecType", bars[i].Name);
                else
                    bars[i].CustomData.Add("SecType", 0.050);

                try
                {
                    bars[i].CustomData.Add("prestress", prestresses[i]);
                    bars[i].CustomData.Add("T", prestresses[i]);
                }
                catch
                {
                    bars[i].CustomData.Add("prestress", 0);
                    bars[i].CustomData.Add("T", 0);
                }

                bars[i].CustomData.Add("StartLength", bars[i].Length);

                BHoM.Materials.Material material = new BHoM.Materials.Material("GalvLockedCoilCables", BHoM.Materials.MaterialType.Steel, 165000000000, 1, 1, 1, 8250);
                bars[i].Material = material;
            }
        }

        public static Structure SetStructure(List<Bar> bars, List<Node> lockedPts, List<double> prestresses, bool restrainXY, double treshold)
            {

            SetBarData(bars, prestresses);

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

