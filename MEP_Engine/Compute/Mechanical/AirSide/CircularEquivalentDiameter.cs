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

        [Description("Returns the Circular Equivalent Diameter for elements that are non-circular, equivalent in length, fluid resistance and airflow.")]
        [Input("ductHeight", "Height of duct opening", typeof(Length))]
        [Input("ductWidth", "Width of duct opening", typeof(Length))]
        [Input("ductProfile", "Duct opening profile shape", typeof(DuctProfile))]
        [Output("circularEquivalentDiameter", "Circular Equivalent Diameter for element section profiles that are non-circular, equivalent in length, fluid resistance and airflow.", typeof(Length))]
        public static double CircularEquivalentDiameter(double ductHeight, double ductWidth, DuctProfile ductProfile)
        {
            switch (ductProfile)
            {
                case DuctProfile.Oval:
                    return OvalCircularEquivalentDiameter(ductHeight, ductWidth);
                default:
                case DuctProfile.Rectangular:
                    return RectangularCircularEquivalentDiameter(ductHeight, ductWidth);
                case DuctProfile.Circular:
                    BH.Engine.Base.Compute.RecordError("Cannot compute the circular equivalent diameter for a circular duct.");
                    return -1;
            }
        }

        ////assuming new physical representation of ducts 
        //[Description("Returns the Circular Equivalent Diameter for elements that are non-circular, equivalent in length, fluid resistance and airflow.")]
        //[Input("duct", "Duct object", typeof(Duct))]
        //[Input("ductProfile", "Duct opening profile shape", typeof(DuctProfile))]
        //[Output("circularEquivalentDiameter", "Circular Equivalent Diameter for element section profiles that are non-circular, equivalent in length, fluid resistance and airflow.", typeof(Length))]
        //public static double CircularEquivalentDiameter(this Duct duct, DuctProfile ductProfile)
        //{
        //    switch (ductProfile)
        //    {
        //        case DuctProfile.Oval:
        //            return OvalCircularEquivalentDiameter(duct.Height, duct.Width);
        //        default:
        //        case DuctProfile.Rectangular:
        //            return RectangularCircularEquivalentDiameter(duct.Height, duct.Width);
        //        case DuctProfile.Circular:
        //            BH.Engine.Base.Compute.RecordError("Cannot compute the circular equivalent diameter for a circular duct.");
        //            return -1;
        //    }
        //}

        /***************************************************/
        /****   Private Methods                          ****/
        /***************************************************/
        private static double RectangularCircularEquivalentDiameter(double ductHeight, double ductWidth)
        {
            return (1.30 * Math.Pow(ductHeight * ductWidth, 0.625) / Math.Pow(ductHeight + ductWidth, 0.250));
        }

        private static double OvalCircularEquivalentDiameter(double ductHeight, double ductWidth)
        {
            double flatspan = ductWidth - ductHeight;
            double perimeter = 2 * flatspan + Math.PI * ductHeight;
            double rectangularSectionArea = flatspan * ductHeight;
            double circularSectionArea = (Math.Pow(ductHeight, 2) * Math.PI) / 4;
            double ovalDuctArea = rectangularSectionArea + circularSectionArea;
            double a = 1.55 * Math.Pow(ovalDuctArea, 0.625);
            double b = Math.Pow(perimeter, 0.250);
            return a / b;
        }
    }
}