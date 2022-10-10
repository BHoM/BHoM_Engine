using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.MEP;
using BH.oM.MEP.System;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the static pressure loss for a duct given a duct object, surface roughness, fluid density, and fluid kinematic viscosity. For use only on ducts pulled from Revit.")]
        [Input("ductObject", "Duct Object pulled from Revit.", typeof(Duct))]
        [Input("surfaceRoughness", "Surface roughness.",typeof(Length))]
        [Input("fluidDensity", "Fluid density.", typeof(Density))]
        [Input("fluidKinematicViscosity", "Fluid kinematic viscosit.y [m2/s]")]
        [Output("pressureLoss", "The static pressure loss due to fluid flow through the flow area.",typeof(Pressure))]
        public static double DuctPressureLoss(this Duct duct, double surfaceRoughness, double fluidDensity, double fluidKinematicViscosity)
        {
            if (duct == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null duct object.");
                return -1;
            }

            if (surfaceRoughness == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null surface roughness.");
                return -1;
            }

            if (fluidDensity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null fluid density.");
                return -1;
            }

            if (fluidKinematicViscosity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null fluid kinematic viscosity.");
                return -1;
            }

            double circularDiameter = duct.SectionProperty.CircularEquivalentDiameter;
            double volumetricFlowRate = duct.FlowRate;
            double ductLength = BH.Engine.MEP.Query.Length(duct);

            double pressureLoss = PressureLossCalculations(surfaceRoughness, fluidDensity, fluidKinematicViscosity, circularDiameter, volumetricFlowRate, ductLength);

            return pressureLoss;
        }

        [Description("Calculates the  friction losses for a duct given duct length, circular diameter, volumetric flow rate, surface roughness, fluid density, and fluid kinematic viscosity.")]
        [Input("ductLength", "Length of duct.", typeof(Length))]
        [Input("circularDiameter", "Circular diameter of a fluid flow area, typically referred to as equivalent circular diameter given any non-circular flow area.", typeof(Length))]
        [Input("volumetricFlowRate", "Volumetric flow rate of fluid through fluid flow area.", typeof(VolumetricFlowRate))]
        [Input("surfaceRoughness", "Surface roughness.", typeof(Length))]
        [Input("fluidDensity", "Fluid density.", typeof(Density))]
        [Input("fluidKinematicViscosity", "Fluid kinematic viscosity [m2/s]")]
        [Output("pressureLoss", "The static pressure loss due to fluid flow through the flow area.", typeof(Pressure))]
        public static double DuctPressureLoss(double ductLength, double circularDiameter, double volumetricFlowRate, double surfaceRoughness, double fluidDensity, double fluidKinematicViscosity)
        {
            if (ductLength == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null duct length.");
                return -1;
            }

            if (circularDiameter == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null circular diameter.");
                return -1;
            }

            if (volumetricFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null volumetric flow rate.");
                return -1;
            }

            if (surfaceRoughness == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null surface roughness.");
                return -1;
            }

            if (fluidDensity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null fluid density.");
                return -1;
            }

            if (fluidKinematicViscosity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null fluid kinematic viscosity.");
                return -1;
            }

            double pressureLoss = PressureLossCalculations(surfaceRoughness, fluidDensity, fluidKinematicViscosity, circularDiameter, volumetricFlowRate, ductLength);

            return pressureLoss;
        }

        [Description("Calculates the friction losses for a duct given duct length, duct height, duct width, volumetric flow rate, surface roughness, fluid density, and fluid kinematic viscosity.")]
        [Input("ductLength", "Length of duct.", typeof(Length))]
        [Input("ductHeight", "Height of duct opening", typeof(Length))]
        [Input("ductWidth", "Width of duct opening", typeof(Length))]
        [Input("ductProfile", "Duct opening profile shape", typeof(DuctProfile))]
        [Input("volumetricFlowRate", "Volumetric flow rate of fluid through fluid flow area.", typeof(VolumetricFlowRate))]
        [Input("surfaceRoughness", "Surface roughness.", typeof(Length))]
        [Input("fluidDensity", "Fluid density.", typeof(Density))]
        [Input("fluidKinematicViscosity", "Fluid kinematic viscosity [m2/s]")]
        [Output("pressureLoss", "The static pressure loss due to fluid flow through the flow area.", typeof(Pressure))]
        public static double DuctPressureLoss(double ductLength, double ductHeight, double ductWidth, DuctProfile ductProfile,double volumetricFlowRate, double surfaceRoughness, double fluidDensity, double fluidKinematicViscosity)
        {
            if (ductLength == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null duct length.");
                return -1;
            }

            if (ductHeight == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null duct height.");
                return -1;
            }

            if (ductWidth == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null duct width.");
                return -1;
            }

            if (volumetricFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null volumetric flow rate.");
                return -1;
            }

            if (surfaceRoughness == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null surface roughness.");
                return -1;
            }

            if (fluidDensity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null fluid density.");
                return -1;
            }

            if (fluidKinematicViscosity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null fluid kinematic viscosity.");
                return -1;
            }

            double circularDiameter = CircularEquivalentDiameter(ductHeight, ductWidth, ductProfile);
            double pressureLoss = PressureLossCalculations(surfaceRoughness, fluidDensity, fluidKinematicViscosity, circularDiameter, volumetricFlowRate, ductLength);

            return pressureLoss;
        }
        //Todo: Add standard air conditions options if fluid density and kinematic viscosity are left null
        //Todo: Fluid object which holds fluid properties, change all equations to take fluid object. This will reduce the amount of inputs into these methods.

        /***************************************************/
        /****   Private Methods                          ****/
        /***************************************************/

        private static double PressureLossCalculations(double surfaceRoughness, double fluidDensity, double fluidKinematicViscosity, double circularDiameter, double volumetricFlowRate, double ductLength)
        {
            double circularFlowAreaVelocity = CircularFlowAreaVelocity(volumetricFlowRate, circularDiameter);
            double reynoldsNumber = ReynoldsNumber(circularFlowAreaVelocity, circularDiameter, fluidKinematicViscosity);
            double frictionFactor = FrictionFactor(reynoldsNumber, circularDiameter, surfaceRoughness);
            double velocityPressure = VelocityPressure(circularFlowAreaVelocity, fluidDensity);
            double pressureLoss = PressureLoss(frictionFactor, ductLength, circularDiameter, velocityPressure);
            return pressureLoss;
        }
        /***************************************************/
    }
}