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
        [Description("Calculates the velocity pressure for a duct or fitting given fluid velocity and fluid density. From ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 8 and 9.")]
        [Input("fluidVelocity", "Fluid flow velocity. For fitting pressure drops, ensure to use the velocity of the actual duct, not the circular equivalent velocity.", typeof(Velocity))]
        [Input("fluidDensity", " If fluid density is omitted, default values apply.", typeof(Density))]
        [Output("velocityPressure", "The velocity pressure due to fluid flow through the flow area.", typeof(Pressure))]
        public static double VelocityPressure(double fluidVelocity, double fluidDensity = double.NaN)
        {
            double velocityPressure;

            if (fluidVelocity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the velocity pressure from a null fluid velocity.");
                return -1;
            }

            if (fluidDensity == double.NaN)
            {
                velocityPressure = 0.602 * Math.Pow(fluidVelocity,  2);
                return velocityPressure;
            }

            velocityPressure = (fluidDensity / 2D) * Math.Pow(fluidVelocity,  2);
            return velocityPressure;
        }

        [Description("Calculates the velocity pressure for a duct object given a duct object and fluid density. Only for use with ducts pulled from Revit. From ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 8 and 9.")]
        [Input("duct", "Duct Object", typeof(Duct))]
        [Input("fluidDensity", "If fluid density is omitted, default values apply.", typeof(Density))]
        [Output("velocityPressure", "The velocity pressure due to fluid flow through the flow area.", typeof(Pressure))]
        public static double VelocityPressure(this Duct duct, double fluidDensity = double.NaN)
        {
            double velocityPressure;

            if (duct == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the velocity pressure from a null duct object.");
                return -1;
            }
            double fluidVelocity = duct.CircularFlowAreaVelocity();

            if (fluidDensity == double.NaN)
            {
                velocityPressure = 0.602 * Math.Pow(fluidVelocity, 2);
                return velocityPressure;
            }

            velocityPressure = (fluidDensity / 2D) * Math.Pow(fluidVelocity, 2);
            return velocityPressure;
        }
        /***************************************************/
    }
}