using BH.oM.Reflection.Attributes;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
//using Duct = BH.oM.MEP.System.Duct;
using BH.oM.Spatial.ShapeProfiles;
using BH.Engine.Geometry;

namespace BH.Engine.MEP
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Query the distance between two parallel ventilation ducts.")]
        [Input("duct1", "BHoM ventilation duct object.")]
        [Input("duct2", "BHoM ventilation duct object.")]
        [Output("distance", "Distance between two parallel ducts.")]
        public static double Distance(this BH.oM.MEP.System.Duct duct1, BH.oM.MEP.System.Duct duct2)
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

            // Lines
            Line line1 = BH.Engine.Geometry.Create.Line(duct1.StartPoint, duct1.EndPoint);
            Line line2 = BH.Engine.Geometry.Create.Line(duct2.StartPoint, duct2.EndPoint);

            // Ensure that the ducts are parallel
            if (line1.IsParallel(line2) == 0)
            {
                BH.Engine.Reflection.Compute.RecordError("The specified ventilation ducts are not parallel. Please ensure that the two specified ventilation ducts run in parallel to each other.");

                return 0;
            }

            // Vectors
            Vector vector1 = BH.Engine.Geometry.Create.Vector(line1.Start, line1.End);
            Vector vector2 = BH.Engine.Geometry.Create.Vector(line2.Start, line2.End);

            // Plane oriented to duct1
            Plane plane = BH.Engine.Geometry.Create.Plane(line1.Start, vector1);

            // The point at which duct2 intersects the plane oriented to duct1
            Point planeIntersectionOfLine2 = line2.PlaneIntersection(plane, true);

            //// Distance between the centres of two parallel ducts, which is the distance between the start point of the first duct and the point at which the second duct intersects the plane
            //double distanceBetweenDuctCentres = line1.Start.Distance(planeIntersectionOfLine2);

            ////// Duct curves
            ////ICurve curve1 = duct1 as ICurve;
            ////ICurve curve2 = duct2 as ICurve;


            //// If both ducts are circular
            //ShapeType ductShape1 = duct1.SectionProperty.SectionProfile.ElementProfile.Shape;
            //ShapeType ductShape2 = duct2.SectionProperty.SectionProfile.ElementProfile.Shape;
            //if (ductShape1 == ShapeType.Tube && ductShape1 == ShapeType.Tube)
            //{
            //    // Radius of duct 1
            //    double ductRadius1 = (duct1.SectionProperty.SectionProfile.ElementProfile as TubeProfile).Diameter / 2;

            //    // Radius of duct 2
            //    double ductRadius2 = (duct2.SectionProperty.SectionProfile.ElementProfile as TubeProfile).Diameter / 2;

            //    // Insulation thickness of duct 1
            //    double insulationThickness1 = (duct1.SectionProperty.SectionProfile.InsulationProfile as TubeProfile).Thickness;

            //    // Insulation thickness of duct 2
            //    double insulationThickness2 = (duct2.SectionProperty.SectionProfile.InsulationProfile as TubeProfile).Thickness;

            //    // Subtract duct radiuses and insulation thicknesses to get the separation distance between two ducts
            //    return distanceBetweenDuctCentres - ductRadius1 - ductRadius2 - insulationThickness1 - insulationThickness2;
            //}

            //BH.Engine.Reflection.Compute.RecordError($"Unable to calculate the distance between the two selected ducts.");

            return 0;
        }

        /***************************************************/
    }
}