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

        [Description("Calculates the circular flow area velocity given a volumetric flow rate and circular diameter.")]
        [Input("volumetricFlowRate", "Volumetric flow rate of fluid through fluid flow area.", typeof(VolumetricFlowRate))]
        [Input("circularDiameter", "Circular diameter of a fluid flow area, typically referred to as equivalent circular diameter given any non-circular flow area.", typeof(Length))]
        [Output("circularFlowAreaVelocity", "The velocity of the fluid through the flow area.", typeof(Velocity))]
        public static double CircularFlowAreaVelocity(double volumetricFlowRate, double circularDiameter)
        {
            if (volumetricFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the circular flow area velocity from a null volumetric flow rate.");
                return -1;
            }

            if (circularDiameter == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the circular flow area velocity from a null circular diameter.");
                return -1;
            }

            double circularArea = Math.PI * Math.Pow((circularDiameter / 2.0), 2);
            double velocity = volumetricFlowRate / circularArea;
            return velocity;
        }

        [Description("Calculates the circular flow area velocity given a volumetric flow rate and circular diameter.")]
        [Input("volumetricFlowRate", "Volumetric flow rate of fluid through fluid flow area.", typeof(VolumetricFlowRate))]
        [Output("circularFlowAreaVelocity", "The velocity of the fluid through the flow area.", typeof(Velocity))]
        public static double CircularFlowAreaVelocity(double volumetricFlowRate, double ductWidth, double ductHeight, DuctProfile ductProfile)
        {
            if (volumetricFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the circular flow area velocity from a null volumetric flow rate.");
                return -1;
            }

            double area;

            switch (ductProfile)
            {
                case DuctProfile.Oval:
                    area = OvalArea(ductHeight, ductWidth);
                    break;
                default:
                case DuctProfile.Rectangular:
                    area = RectangularArea(ductHeight, ductWidth);
                    break;
                case DuctProfile.Circular:
                    return 0;
            }
            double velocity = volumetricFlowRate / area;
            return velocity;
        }

        [Description("Calculates the circular flow area velocity given a duct object. Only for use with ducts pulled from Revit.")]
        [Input("duct", "Duct Object", typeof(Duct))]
        [Output("circularFlowAreaVelocity", "The velocity of the fluid through the flow area.", typeof(Velocity))]
        public static double CircularFlowAreaVelocity(this Duct duct)
        {
            if (duct == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the circular flow area velocity from a null duct object.");
                return -1;
            }

            double volumetricFlowRate = duct.FlowRate;
            double circularDiameter = duct.SectionProperty.CircularEquivalentDiameter;
            double circularArea = Math.PI * Math.Pow((circularDiameter / 2.0), 2);
            double velocity = volumetricFlowRate / circularArea;
            return velocity;
        }
        
        /***************************************************/
        /****   Private Methods                          ****/
        /***************************************************/

        private static double RectangularArea(double ductHeight, double ductWidth)
        {

            double rectangularDuctArea = ductHeight * ductWidth;
            return rectangularDuctArea;
        }

        private static double OvalArea(double ductHeight, double ductWidth)
        {
            double flatspan = ductWidth - ductHeight;
            double rectangularSectionArea = flatspan * ductHeight;
            double circularSectionArea = (Math.Pow(ductHeight, 2) * Math.PI) / 4;
            double ovalDuctArea = rectangularSectionArea + circularSectionArea;
            return ovalDuctArea;
        }

        /***************************************************/
    }
}