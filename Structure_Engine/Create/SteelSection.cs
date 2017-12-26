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
            Dictionary<string, object> constants = Geometry.Compute.Integrate(edges);

            ISectionDimensions dimensions = new PolygonDimensions();
            constants["J"] = dimensions.ITorsionalConstant();
            constants["Iw"] = dimensions.IWarpingConstant();

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
            Dictionary<string, object> constants = Geometry.Compute.Integrate(edges);

            constants["J"] = dimensions.ITorsionalConstant();
            constants["Iw"] = dimensions.IWarpingConstant();

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
    }
}
