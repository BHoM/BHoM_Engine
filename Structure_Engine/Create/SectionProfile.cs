using System.Collections.Generic;
using System.Collections.ObjectModel;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;
using BH.oM.Common.Materials;
using System;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ISectionProfile ISectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            List<ICurve> curves = ISectionCurves(flangeThickness, width, flangeThickness, width, webthickness, height - 2 * flangeThickness, rootRadius, toeRadius);
            return new ISectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/

        public static BoxProfile BoxProfile(double height, double width, double thickness, double outerRadius, double innerRadius)
        {
            List<ICurve> curves = BoxSectionCurves(width, height, thickness, thickness, innerRadius, outerRadius);
            return new BoxProfile(height, width, thickness, outerRadius, innerRadius, curves);
        }

        /***************************************************/

        public static AngleProfile AngleProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            List<ICurve> curves = AngleSectionCurves(width, height, flangeThickness, webthickness, rootRadius, toeRadius);
            return new AngleProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/

        public static ChannelProfile ChannelProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            List<ICurve> curves = ChannelSectionCurves(height, width, webthickness, flangeThickness, rootRadius, toeRadius);
            return new ChannelProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/

        public static CircleProfile CircleProfile(double diameter)
        {
            List<ICurve> curves = CircleSectionCurves(diameter / 2);
            return new CircleProfile(diameter, curves);
        }

        /***************************************************/

        public static FabricatedBoxProfile FabricatedBoxProfile(double height, double width, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            List<ICurve> curves = FabricatedBoxSectionCurves(width, height, webThickness, topFlangeThickness, botFlangeThickness);
            return new FabricatedBoxProfile(height, width, webThickness, topFlangeThickness, botFlangeThickness, weldSize, curves);
        }

        /***************************************************/

        public static FabricatedISectionProfile FabricatedISectionProfile(double height, double topFlangeWidth, double botFlangeWidth, double webThickness, double topFlangeThickness, double botFlangeThickness, double weldSize)
        {
            List<ICurve> curves = ISectionCurves(topFlangeThickness, topFlangeWidth, botFlangeThickness, botFlangeWidth, webThickness, height - botFlangeThickness - topFlangeThickness,0,0);
            return new FabricatedISectionProfile(height, topFlangeWidth, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize, curves);
        }

        /***************************************************/

        public static FreeFormProfile FreeFormProfile(IEnumerable<ICurve> edges)
        {
            return new FreeFormProfile(edges);
        }

        /***************************************************/

        public static RectangleProfile RectangleProfile(double height, double width, double cornerRadius)
        {
            List<ICurve> curves = RectangleSectionCurves(width, height, cornerRadius);
            return new RectangleProfile(height, width, cornerRadius, curves);
        }

        /***************************************************/

        public static TSectionProfile TSectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            List<ICurve> curves = TeeSectionCurves(flangeThickness, width, webthickness, height - flangeThickness, rootRadius, toeRadius);
            return new TSectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/

        public static TubeProfile TubeProfile(double diameter, double thickness)
        {
            List<ICurve> curves = TubeSectionCurves(diameter / 2, thickness);
            return new TubeProfile(diameter, thickness, curves);
        }

        /***************************************************/

        public static ZSectionProfile ZSectionProfile(double height, double width, double webthickness, double flangeThickness, double rootRadius, double toeRadius)
        {
            throw new NotImplementedException();
            //TODO: Section curves for z-profile
            //List<ICurve> curves = ZSectionCurves(flangeThickness, width, webthickness, height - flangeThickness, rootRadius, toeRadius);
            //return new ZSectionProfile(height, width, webthickness, flangeThickness, rootRadius, toeRadius, curves);
        }

        /***************************************************/
    }
}
