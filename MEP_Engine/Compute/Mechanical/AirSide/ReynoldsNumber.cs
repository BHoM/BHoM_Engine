using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.MEP.System;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the reynolds number for a duct given fluid velocity, duct circular diameter, and fluid kinematic viscosity.From ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 20 and 21.")]
        [Input("fluidVelocity", "Fluid flow velocity.", typeof(Velocity))]
        [Input("circularDiameter", "Circular diameter of a fluid flow area, typically referred to as equivalent circular diameter given any non-ciruclar flow area.",typeof(Length))]
        [Input("fluidKinematicViscosity", "Fluid kinematice viscosity: If fluid kinematic viscosity is ommited, default values apply. [m2/s] ")]
        [Output("reynoldsNumber", "The reynolds number for fluid flow through the flow area.[unitless]")]
        public static double ReynoldsNumber(double fluidVelocity, double circularDiameter, double fluidKinematicViscosity = double.NaN)
        {
            
            if (fluidVelocity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null fluid velocity.");
                return -1;
            }

            if (circularDiameter == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null circular diameter.");
                return -1;
            }

            if (fluidKinematicViscosity == double.NaN)
            {
                //For standard air and temperature between 4C and 38C, it is acceptable to use the equation below.
                //The coefficienct of "66.4" comes from a simplified equation as shown in ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 21. 
                BH.Engine.Base.Compute.RecordNote("Reynolds number computed from a simplified equation as shown in ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 21.");
                return 66.4 * circularDiameter * 1000 * fluidVelocity;
                
            }

            return (circularDiameter * 1000 * fluidVelocity) / (1000 * fluidKinematicViscosity);

        }

        [Description("Calculates the reynolds number for a duct object and fluid kinematic viscosity. From ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 20 and 21. Only for use with ducts pulled from Revit.")]
        [Input("duct", "Duct Object", typeof(Duct))]
        [Input("fluidKinematicViscosity", "Fluid kinematic viscosity. If fluid kinematic viscosity is ommited, default values apply. [m2/s] ")]
        [Output("reynoldsNumber", "The reynolds number for fluid flow through the flow area.[unitless]")]
        public static double ReynoldsNumber(this Duct duct, double fluidKinematicViscosity = double.NaN)
        {
            
            if (duct == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null fluid velocity.");
                return -1;
            }

            double circularDiameter = duct.SectionProperty.CircularEquivalentDiameter;
            double fluidVelocity = duct.CircularFlowAreaVelocity();

            if (fluidKinematicViscosity == double.NaN)
            {
                //For standard air and temperature between 4C and 38C, it is acceptable to use the equation below.
                //The coefficienct of "66.4" comes from a simplified equation as shown in ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 21. 
                BH.Engine.Base.Compute.RecordNote("Reynolds number computed from a simplified equation as shown in ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 21.");
                return 66.4 * circularDiameter * 1000 * fluidVelocity;
            }

            return (circularDiameter * 1000 * fluidVelocity) / (1000 * fluidKinematicViscosity);
        }
        /***************************************************/
    }
}