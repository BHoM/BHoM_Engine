using System.Collections.Generic;
using System.Collections.ObjectModel;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;


namespace BH.Engine.Structure
{
    public static partial class Create
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SteelSection StandardSteelISection(double height, double webThickness, double flangeWidth, double flangeThickness, double rootRadius = 0, double toeRadius = 0)
        {
            return SteelSectionFromDimensions(new StandardISectionDimensions(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius));
        }

        /***************************************************/

        public static SteelSection FabricatedSteelISection(double height, double webThickness, double topFlangeWidth, double topFlangeThickness, double botFlangeWidth, double botFlangeThickness,  double weldSize)
        {
            return SteelSectionFromDimensions(new FabricatedISectionDimensions(height, topFlangeWidth, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize));
        }

        /***************************************************/

        public static SteelSection StandardSteelBoxSection(double height, double width, double thickness, double innerRadius = 0, double outerRadius = 0)
        {
            return SteelSectionFromDimensions(new StandardBoxDimensions(height, width, thickness, innerRadius, outerRadius));
        }

        /***************************************************/

        public static SteelSection FabricatedSteelBoxSection(double height, double width, double webThickness, double flangeThickness, double weldSize)
        {
            return SteelSectionFromDimensions(new FabricatedBoxDimensions(height, width, webThickness, flangeThickness, flangeThickness, weldSize));


        }

        /***************************************************/

        public static SteelSection SteelTubeSection(double diameter, double thickness)
        {
            return SteelSectionFromDimensions(new TubeDimensions(diameter, thickness));
        }

        /***************************************************/

        public static SteelSection SteelRectangleSection(double height, double width, double cornerRadius=0)
        {
            return SteelSectionFromDimensions(new RectangleSectionDimensions(height, width, cornerRadius));
        }

        /***************************************************/

        public static SteelSection SteelCircularSection(double diameter)
        {
            return SteelSectionFromDimensions(new CircleDimensions(diameter));
        }

        /***************************************************/

        public static SteelSection SteelTeeSection(double height, double webThickness, double flangeWidth, double flangeThickness,  double rootRadius = 0, double toeRadius = 0)
        {
            return SteelSectionFromDimensions(new StandardTeeSectionDimensions(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius));

        }

        /***************************************************/

        public static SteelSection SteelAngleSection(double height, double webThickness, double width, double flangeThickness, double rootRadius = 0, double toeRadius = 0)
        {
            return SteelSectionFromDimensions(new StandardAngleSectionDimensions(height, width, webThickness, flangeThickness, rootRadius, toeRadius));
        }

        /***************************************************/

        public static SteelSection SteelFreeFormSection(List<ICurve> edges)
        {
            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            ISectionDimensions dimensions = new PolygonDimensions();
            constants["J"] = dimensions.IGetTorsionalConstant();
            constants["Iw"] = dimensions.IGetWarpingConstant();

            SteelSection section = new SteelSection(edges, dimensions,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
                (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            return section;


        }



        /***************************************************/

        public static SteelSection SteelSectionFromDimensions(ISectionDimensions dimensions, string name = "")
        {

            List<ICurve> edges = dimensions.IGetEdgeCUrves();
            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            constants["J"] = dimensions.IGetTorsionalConstant();
            constants["Iw"] = dimensions.IGetWarpingConstant();

            SteelSection section = new SteelSection(edges, dimensions,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
                (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            section.Name = name;
            return section;

        }

        /***************************************************/
        /****** Older methods - To be deleted              */
        /***************************************************/


        /***************************************************/


        //private static SteelSection SteelSectionCreationHelper(IEnumerable<ICurve> edges, ISectionDimensions dimensions,
        //    Dictionary<string, object> constants)
        //{
        //    SteelSection section = new SteelSection(edges, dimensions,
        //        (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
        //        (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
        //        (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

        //    section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
        //    section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

        //    return section;

        //}


        //public static SteelSection SteelISection(double height, double webThickness, double flangeWidth, double flangeThickness, double innerRadius = 0, double toeRadius = 0)
        //{
        //    return SteelISection(height, webThickness, flangeWidth, flangeThickness, flangeWidth, flangeThickness, innerRadius, toeRadius);
        //}

        ///***************************************************/

        //public static SteelSection SteelISection(double height, double webThickness, double topFlangeWidth, double topFlangeThickness, double botFlangeWidth, double botFlangeThickness, double innerRadius = 0, double toeRadius = 0)
        //{
        //    double webHeight = height - topFlangeThickness - botFlangeThickness;

        //    List<ICurve> edges = ISecctionCurves(topFlangeThickness, topFlangeWidth, botFlangeThickness, botFlangeWidth, webThickness, webHeight, innerRadius, toeRadius);

        //    Dictionary<string, object> constants = Query.IntegrateCurve(edges);

        //    constants["J"] = Query.GetTorsionalConstantThinWalled(ShapeType.ISection, (double)constants["TotalDepth"], (double)constants["TotalWidth"], topFlangeWidth, botFlangeWidth, topFlangeThickness, botFlangeThickness, webThickness);
        //    constants["Iw"] = Query.GetWarpingConstant(ShapeType.ISection, (double)constants["TotalDepth"], (double)constants["TotalWidth"], topFlangeWidth, botFlangeWidth, topFlangeThickness, botFlangeThickness, webThickness);

        //    return SteelSectionCreationHelper(edges, ShapeType.ISection, topFlangeWidth, botFlangeWidth, 0, webThickness, topFlangeThickness, botFlangeThickness, innerRadius, toeRadius, 0, constants);


        //}

        ///***************************************************/

        //public static SteelSection SteelBoxSection(double height, double width, double thickness, double innerRadius = 0, double outerRadius = 0)
        //{
        //    return SteelBoxSection(height, width, thickness, thickness, innerRadius, outerRadius);
        //}

        ///***************************************************/

        //public static SteelSection SteelBoxSection(double height, double width, double webThickness, double flangeThickness, double innerRadius = 0, double outerRadius = 0)
        //{
        //    List<ICurve> edges = BoxSectionCurves(width, height, webThickness, flangeThickness, innerRadius, outerRadius);

        //    Dictionary<string, object> constants = Query.IntegrateCurve(edges);

        //    constants["J"] = Query.GetTorsionalConstantThinWalled(ShapeType.Box, (double)constants["TotalDepth"], (double)constants["TotalWidth"], width, height, flangeThickness, flangeThickness, webThickness);
        //    constants["Iw"] = Query.GetWarpingConstant(ShapeType.Box, (double)constants["TotalDepth"], (double)constants["TotalWidth"], width, height, flangeThickness, flangeThickness, webThickness);

        //    return SteelSectionCreationHelper(edges, ShapeType.Box, width, 0, 0, webThickness, flangeThickness, 0, innerRadius, outerRadius, 0, constants);


        //}

        ///***************************************************/

        //public static SteelSection SteelTubeSection(double diameter, double thickness)
        //{
        //    List<ICurve> edges = TubeSectionCurves(diameter / 2, thickness);

        //    Dictionary<string, object> constants = Query.IntegrateCurve(edges);

        //    constants["J"] = Query.GetTorsionalConstantThinWalled(ShapeType.Tube, (double)constants["TotalDepth"], (double)constants["TotalWidth"], diameter, 0, thickness, thickness, thickness);
        //    constants["Iw"] = Query.GetWarpingConstant(ShapeType.Tube, (double)constants["TotalDepth"], (double)constants["TotalWidth"], diameter, 0, thickness, thickness, thickness);

        //    return SteelSectionCreationHelper(edges, ShapeType.Tube, diameter, 0, 0, thickness, thickness, 0, 0, 0, 0, constants);


        //}

        ///***************************************************/

        //public static SteelSection SteelRectangleSection(double height, double width, double radius = 0)
        //{
        //    List<ICurve> edges = RectangleSectionCurves(width, height, radius);

        //    Dictionary<string, object> constants = Query.IntegrateCurve(edges);

        //    constants["J"] = Query.GetTorsionalConstantThinWalled(ShapeType.Rectangle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], 0, 0, 0, 0, 0);
        //    constants["Iw"] = Query.GetWarpingConstant(ShapeType.Rectangle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], 0, 0, 0, 0, 0);

        //    return SteelSectionCreationHelper(edges, ShapeType.Rectangle, width, height, 0, 0, 0, 0, radius, 0, 0, constants);


        //}

        ///***************************************************/

        //public static SteelSection SteelCircularSection(double diameter)
        //{
        //    List<ICurve> edges = CircleSectionCurves(diameter / 2);

        //    Dictionary<string, object> constants = Query.IntegrateCurve(edges);

        //    constants["J"] = Query.GetTorsionalConstantThinWalled(ShapeType.Circle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], diameter, 0, 0, 0, 0);
        //    constants["Iw"] = Query.GetWarpingConstant(ShapeType.Circle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], diameter, 0, 0, 0, 0);

        //    return SteelSectionCreationHelper(edges, ShapeType.Circle, diameter, 0, 0, 0, 0, 0, 0, 0, 0, constants);


        //}

        ///***************************************************/

        //public static SteelSection SteelTeeSection(double height, double webThickness, double flangeWidth, double flangeThickness, double r1 = 0, double r2 = 0)
        //{
        //    double webHeight = height - flangeThickness;
        //    List<ICurve> edges = TeeSectionCurves(flangeThickness, flangeWidth, webThickness, webHeight, r1, r2);

        //    Dictionary<string, object> constants = Query.IntegrateCurve(edges);

        //    constants["J"] = Query.GetTorsionalConstantThinWalled(ShapeType.Circle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], flangeWidth, height, flangeThickness, flangeThickness, webThickness);
        //    constants["Iw"] = Query.GetWarpingConstant(ShapeType.Circle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], flangeWidth, height, flangeThickness, flangeThickness, webThickness);

        //    return SteelSectionCreationHelper(edges, ShapeType.Circle, height, flangeThickness, 0, webThickness, flangeThickness, flangeThickness, r1, r2, 0, constants);


        //}
        ///***************************************************/

        //public static SteelSection SteelAngleSection(double height, double webThickness, double width, double flangeThickness, double innerRadius = 0, double toeRadius = 0)
        //{
        //    double webHeight = height - flangeThickness;
        //    List<ICurve> edges = AngleSectionCurves(width, height, flangeThickness, webThickness, innerRadius, toeRadius);

        //    Dictionary<string, object> constants = Query.IntegrateCurve(edges);

        //    constants["J"] = Query.GetTorsionalConstantThinWalled(ShapeType.Circle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], width, height, flangeThickness, flangeThickness, webThickness);
        //    constants["Iw"] = Query.GetWarpingConstant(ShapeType.Circle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], width, height, flangeThickness, flangeThickness, webThickness);

        //    return SteelSectionCreationHelper(edges, ShapeType.Circle, height, flangeThickness, 0, webThickness, flangeThickness, flangeThickness, innerRadius, toeRadius, 0, constants);


        //}

        ///***************************************************/

        //public static SteelSection SteelFreeFormSection(List<ICurve> edges)
        //{
        //    Dictionary<string, object> constants = Query.IntegrateCurve(edges);

        //    constants["J"] = Query.GetTorsionalConstantThinWalled(ShapeType.Polygon, (double)constants["TotalDepth"], (double)constants["TotalWidth"], 0, 0, 0, 0, 0);
        //    constants["Iw"] = Query.GetWarpingConstant(ShapeType.Polygon, (double)constants["TotalDepth"], (double)constants["TotalWidth"], 0, 0, 0, 0, 0);

        //    return SteelSectionCreationHelper(edges, ShapeType.Polygon, 0, 0, 0, 0, 0, 0, 0, 0, 0, constants);


        //}

        ///***************************************************/


        //private static SteelSection SteelSectionCreationHelper(IEnumerable<ICurve> edges, ISectionDimensions dimensions,
        //    Dictionary<string, object> constants)
        //{
        //    SteelSection section = new SteelSection(edges, dimensions,
        //        (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
        //        (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
        //        (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

        //    section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
        //    section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

        //    return section;

        //}

    }
}
