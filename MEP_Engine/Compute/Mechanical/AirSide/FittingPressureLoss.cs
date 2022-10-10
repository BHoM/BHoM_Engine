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

        [Description("Calculates the fitting dynamic losses for a duct fitting given fitting coefficient and velocity pressure. From ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 29 through 32")]
        [Input("localLossCoefficient", "Local loss coefficient as determined by fitting type and aspect ratio of flow areas, [unit-less]")]
        [Input("velocityPressure", "The velocity pressure due to fluid flow through the flow area.", typeof(Pressure))]
        [Output("fittingPressureLoss", "The fitting pressure losses due to fluid flow through the flow area.", typeof(Pressure))]
        public static double FittingPressureLoss(double localLossCoefficient, double velocityPressure)
        {
            if (localLossCoefficient == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the fitting pressure losses from a null local loss coefficient.");
                return -1;
            }

            if (velocityPressure == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the fitting pressure losses from a null velocity pressure.");
                return -1;
            }

            double fittingPressureLoss = localLossCoefficient * velocityPressure;
            return fittingPressureLoss;
        }

        [Description("Calculates the fitting dynamic losses for a duct fitting given fitting coefficient, fluid velocity, and fluid density. From ASHRAE 2021 Fundamentals (SI) Chapter 21 Duct Design, Equation 29 through 32")]
        [Input("localLossCoefficient", "Local loss coefficient as determined by fitting type and aspect ratio of flow areas, [unit-less]")]
        [Input("fluidVelocity", "Fluid flow velocity. For fitting pressure drops, ensure to use the velocity of the actual duct, not the circular equivalent velocity.", typeof(Velocity))]
        [Input("fluidDensity", "Fluid density.", typeof(Density))]
        [Output("fittingPressureLoss", "The fitting pressure losses due to fluid flow through the flow area.", typeof(Pressure))]
        public static double FittingPressureLoss(double localLossCoefficient, double fluidVelocity, double fluidDensity = double.NaN)
        {
            if (localLossCoefficient == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the fitting pressure losses from a null local loss coefficient.");
                return -1;
            }

            if (fluidVelocity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the velocity pressure from a null fluid velocity.");
                return -1;
            }

            double fittingPressureLoss = FittingPressureLoss(localLossCoefficient, VelocityPressure(fluidVelocity, fluidDensity));
            return fittingPressureLoss;
        }
        /***************************************************/
    }
}