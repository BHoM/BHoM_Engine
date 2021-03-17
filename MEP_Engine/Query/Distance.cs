using BH.oM.Reflection.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Duct = BH.oM.MEP.System.Duct;
using BH.oM.Spatial.ShapeProfiles;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Query the distance between two parallel ventilation ducts.")]
        [Input("duct1", "First BHoM ventilation duct converted from Revit.")]
        [Input("duct2", "Second BHoM ventilation duct converted from Revit.")]
        [Output("distance", "Distance between two parallel ducts in mm.")]
        public static double Distance(this Duct duct1, Duct duct2)
        {
            // Input check
            if (duct1 == null)
            {
                BH.Engine.Reflection.Compute.RecordError("The 'duct1' argument is invalid. Please correct the input.");

                return 0;
            }

            if (duct2 == null)
            {
                BH.Engine.Reflection.Compute.RecordError("The 'duct2' argument is invalid. Please correct the input.");

                return 0;
            }

            // Duct 1 as a line
            Line line1 = BH.Engine.Geometry.Create.Line(duct1.StartPoint, duct1.EndPoint);

            // Duct 2 as a line
            Line line2 = BH.Engine.Geometry.Create.Line(duct2.StartPoint, duct2.EndPoint);

            // Vectors
            //Vector vector1 = BH.Engine.Geometry.Create.


            //// Duct curves
            //ICurve curve1 = duct1 as ICurve;
            //ICurve curve2 = duct2 as ICurve;
            
            // Get the distance between the centres of the ducts represented by curves of linear ducts
            double distanceBetweenDuctCentres = BH.Engine.Geometry.Query.Distance(line1, line2);

            // If both ducts are circular
            ShapeType ductShape1 = duct1.SectionProperty.SectionProfile.ElementProfile.Shape;
            ShapeType ductShape2 = duct2.SectionProperty.SectionProfile.ElementProfile.Shape;
            if (ductShape1 == ShapeType.Tube && ductShape1 == ShapeType.Tube)
            {
                // Radius of duct 1
                double ductRadius1 = (duct1.SectionProperty.SectionProfile.ElementProfile as TubeProfile).Diameter / 2;

                // Radius of duct 2
                double ductRadius2 = (duct2.SectionProperty.SectionProfile.ElementProfile as TubeProfile).Diameter / 2;

                // Insulation thickness of duct 1
                double insulationThickness1 = (duct1.SectionProperty.SectionProfile.InsulationProfile as TubeProfile).Thickness;

                // Insulation thickness of duct 2
                double insulationThickness2 = (duct2.SectionProperty.SectionProfile.InsulationProfile as TubeProfile).Thickness;

                // Subtract duct radiuses and insulation thicknesses to get the separation distance between two ducts
                return distanceBetweenDuctCentres - ductRadius1 - ductRadius2 - insulationThickness1 - insulationThickness2;
            }

            BH.Engine.Reflection.Compute.RecordError($"Unable to calculate the distance between the two selected ducts.");
            
            return 0;
        }

        /***************************************************/
    }
}