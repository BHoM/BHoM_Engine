using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHoM.Geometry;
using BHoM.Structural.Elements;
using BHoM.Structural.Results;
using BHoM;

namespace FormFinding_Engine
{
    public class DynamicRelaxation
    {
        public static Structure SetStructure(List<Bar> bars, List<Node> restrainedNodes, List<double> areas, List<double> prestresses, double timeStep, double damping, bool useMassDamping, bool calcSafeTimeStep, double treshold)
        {
            Structure structure = new Structure(bars, restrainedNodes);

            structure.SetBarData(areas, prestresses);

            structure.c = damping;
            structure.dt = timeStep;  
            structure.t = 0; 
            structure.nodeTol = 0.1;  
            structure.treshold = treshold;

            structure.SetConnectedBars();
            structure.SetStiffness();

            if(useMassDamping)
                structure.SetFictionalNodeMass();
            else
                structure.SetLumpedNodeMass();

            if (calcSafeTimeStep)
                structure.CalcSafeTimeStep();

            structure.SetStartVelocity();

            return structure;
        }

        public static void RelaxStructure(Structure structure, double gravity, bool useMassDamping)
        {

            structure.t += structure.dt;

            structure.CalcBarForce();

            structure.CalcNodeForce(gravity);

            if(useMassDamping)
                structure.SetFictionalNodeMass();

            structure.CalcAcceleration();

            structure.CalcVelocity();

            structure.CheckConstraints();

            structure.CalcTranslation();

                
            structure.CalcKineticEnergy();

            //Sets velocities to 0 if energy peak. Can use less damping and hence quicker, but needs more work.
            //structure.CheckKineticEnergyPeak();

            structure.UpdateGeometry();

        }
    }

}

