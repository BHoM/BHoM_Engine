using System;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the static pressure losses for a duct given friction factor, duct length, circular diameter, fluid density, and fluid velocity. From ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 18 -- Darcy Equation.")]
        [Input("frictionFactor", "Friction factor, [unit-less]")]
        [Input("ductLength", "Length of duct", typeof(Length))]
        [Input("circularDiameter", "Circular diameter of a fluid flow area, typically referred to as equivalent circular diameter given any non-circular flow area.", typeof(Length))]
        [Input("velocityPressure", "The velocity pressure due to fluid flow through the flow area.", typeof(Pressure))]
        [Output("pressureLoss", "The static pressure losses due to fluid flow through the flow area.", typeof(Pressure))]
        public static double PressureLoss(double frictionFactor, double ductLength, double circularDiameter, double velocityPressure)
        {
            if (frictionFactor == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null friction factor.");
                return -1;
            }

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

            if (velocityPressure == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction losses from a null velocity pressure.");
                return -1;
            }

            double pressureLoss = (1000 * frictionFactor * ductLength / (circularDiameter * 1000)) * velocityPressure;
            return pressureLoss;

        }
        /***************************************************/
    }
}