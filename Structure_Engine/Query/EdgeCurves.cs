using BH.oM.Geometry;
using BH.oM.Structural.Properties;
using System;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this RectangleSectionDimensions dimensions)
        {
            return Create.RectangleSectionCurves(dimensions.Width, dimensions.Height, dimensions.CornerRadius);
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this CircleDimensions dimensions)
        {
            return Create.CircleSectionCurves(dimensions.Diameter / 2);
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this TubeDimensions dimensions)
        {
            return Create.TubeSectionCurves(dimensions.Diameter / 2, dimensions.Thickness);
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this FabricatedBoxDimensions dimensions)
        {
            //TODO: Unequal plate thickness and welds
            return Create.BoxSectionCurves(dimensions.Width, dimensions.Height, dimensions.WebThickness, dimensions.BotFlangeThickness, 0, 0);
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this StandardBoxDimensions dimensions)
        {
            //TODO: Unequal plate thickness and welds
            return Create.BoxSectionCurves(dimensions.Width, dimensions.Height, dimensions.Thickness, dimensions.Thickness, dimensions.InnerRadius, dimensions.OuterRadius);
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this FabricatedISectionDimensions dimensions)
        {
            return Create.ISectionCurves(dimensions.TopFlangeThickness, dimensions.TopFlangeWidth, dimensions.BotFlangeThickness, dimensions.BotFlangeWidth, dimensions.WebThickness, dimensions.Height - dimensions.TopFlangeThickness - dimensions.BotFlangeThickness, 0, 0);
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this StandardISectionDimensions dimensions)
        {
            return Create.ISectionCurves(dimensions.FlangeThickness, dimensions.Width, dimensions.FlangeThickness, dimensions.Width, dimensions.WebThickness, dimensions.Height - dimensions.FlangeThickness * 2, dimensions.RootRadius, dimensions.ToeRadius);
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this PolygonDimensions dimensions)
        {
            return new List<ICurve>();
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this StandardAngleSectionDimensions dimensions)
        {
            return Create.AngleSectionCurves(dimensions.Width, dimensions.Height, dimensions.FlangeThickness, dimensions.WebThickness, dimensions.RootRadius, dimensions.ToeRadius);
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this StandardChannelSectionDimensions dimensions)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this StandardTeeSectionDimensions dimensions)
        {
            return Create.TeeSectionCurves(dimensions.FlangeThickness, dimensions.Width, dimensions.WebThickness, dimensions.Height - dimensions.FlangeThickness, dimensions.RootRadius, dimensions.ToeRadius);
        }

        /***************************************************/

        public static List<ICurve> GetEdgeCurves(this StandardZedSectionDimensions dimensions)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<ICurve> IGetEdgeCUrves(this ISectionDimensions dimensions)
        {
            return GetEdgeCurves(dimensions as dynamic);
        }

    }
}
