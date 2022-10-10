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

        [Description("Calculates the  friction factor for a duct given reynolds number, circular diameter, and surface roughness. Typically used in pressure drop calculations")]
        [Input("reynoldsNumber", "Reynolds number, [unitless]")]
        [Input("circularDiameter", "Circular diameter of a fluid flow area, typically referred to as equivalent circular diameter given any non-ciruclar flow area.", typeof(Length))]
        [Input("surfaceRoughness", "Surface roughness.", typeof(Length))]
        [Output("frictionFactor", "The friction factor, unitless.")]
        public static double FrictionFactor(double reynoldsNumber, double circularDiameter, double surfaceRoughness)
        {
            if(reynoldsNumber == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null reynolds number.");
                return -1;
            }

            if(circularDiameter == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null circular diameter.");
                return -1;
            }

            if (surfaceRoughness == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null surface roughness.");
                return -1;
            }

            return _FrictionFactor(surfaceRoughness, reynoldsNumber, circularDiameter);
        }

        [Description("Calculates the friction factor for a duct given duct object and surface roughness. Only for use with ducts pulled from Revit.")]
        [Input("duct", "Duct Object.", typeof(Duct))]
        [Input("surfaceRoughness", "Surface Roughness", typeof(Length))]
        [Output("frictionFactor", "The friction factor, unitless.")]
        public static double FrictionFactor(this Duct duct, double surfaceRoughness)
        {
            if (duct == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null duct object.");
                return -1;
            }

            //Todo: add surface roughness to ducts, figure a way to pull roughness from duct object.
            if (surfaceRoughness == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the friction factor from a null surface roughness.");
                return -1;
            }

            double reynoldsNumber = ReynoldsNumber(duct);
            double circularDiameter = duct.SectionProperty.CircularEquivalentDiameter;

            return _FrictionFactor(surfaceRoughness, reynoldsNumber, circularDiameter);
        }

        private static double _FrictionFactor(double surfaceRoughness, double reynoldsNumber, double circularDiameter)
        {
            double componentA = Math.Pow(2.457 * Math.Log(1.0 / (Math.Pow(7.0 / reynoldsNumber, 0.9) + (0.27 * surfaceRoughness / circularDiameter))), 16);
            double componentB = Math.Pow((37530.0 / reynoldsNumber), 16);
            double frictionFactor = 8 * Math.Pow(Math.Pow(8.0 / reynoldsNumber, 12) + (1.0 / Math.Pow(componentA + componentB, 1.5)), 1.0 / 12);
            return frictionFactor;
        }
        /***************************************************/
    }
}