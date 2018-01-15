﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BHoM.Geometry;
//using BHoM.Structural.Elements;
//using BHoM.Structural.Results;
//using BHoM;

//namespace FormFinding_Engine
//{
//    public class DynamicRelaxation
//    {
//        public static Structure SetStructure(List<Bar> bars, List<Node> restrainedNodes, List<double> areas, List<double> prestresses, double timeStep, double damping, bool useMassDamping, bool calcSafeTimeStep, double treshold)
//        {
//            Structure structure = new Structure(bars, restrainedNodes);

//            structure.SetBarData(areas, prestresses);

//            structure.m_c = damping;
//            structure.m_dt = timeStep;  
//            structure.m_t = 0; 
//            structure.m_nodeTol = 0.1;  
//            structure.m_treshold = treshold;

//            structure.SetConnectedBars();
//            structure.SetStiffness();

//            if(useMassDamping)
//                structure.SetFictionalNodeMass();
//            else
//                structure.SetLumpedNodeMass();

//            if (calcSafeTimeStep)
//                structure.CalcSafeTimeStep();

//            structure.SetStartVelocity();

//            return structure;
//        }

//        public static void RelaxStructure(Structure structure, double gravity, bool useMassDamping)
//        {

//            structure.m_t += structure.m_dt;

//            structure.CalcBarForce();

//            structure.CalcNodeForce(gravity);

//            if(useMassDamping)
//                structure.SetFictionalNodeMass();

//            structure.CalcAcceleration();

//            structure.CalcVelocity();

//            structure.CheckConstraints();

//            structure.CalcTranslation();

                
//            structure.CalcKineticEnergy();

//            //Sets velocities to 0 if energy peak. Can use less damping and hence quicker, but needs more work.
//            //structure.CheckKineticEnergyPeak();

//            structure.UpdateGeometry();

//        }
//    }

//}

