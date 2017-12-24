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

        public static ConcreteSection ConcreteRectangleSection(double height, double width)
        {
            return ConcreteSectionFromDimensions(new RectangleSectionDimensions(height, width, 0));
        }

        /***************************************************/

        public static ConcreteSection ConcreteTeeSection(double height, double webThickness, double flangeWidth, double flangeThickness)
        {
            return ConcreteSectionFromDimensions(new StandardTeeSectionDimensions(height, flangeWidth, webThickness, flangeThickness, 0, 0));
        }


        /***************************************************/

        public static ConcreteSection ConcreteCircularSection(double diameter)
        {
            return ConcreteSectionFromDimensions(new CircleDimensions(diameter));
        }

        /***************************************************/

        public static ConcreteSection ConcreteFreeFormSection(List<ICurve> edges)
        {

            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            constants["J"] = 0;
            constants["Iw"] = 0;

            ConcreteSection section = new ConcreteSection(edges, new PolygonDimensions(),
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
                (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            return section;
        }

        /***************************************************/

        public static ConcreteSection ConcreteSectionFromDimensions(ISectionDimensions dimensions)
        {

            List<ICurve> edges = dimensions.IGetEdgeCUrves();
            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            constants["J"] = dimensions.IGetTorsionalConstant();
            constants["Iw"] = dimensions.IGetWarpingConstant();

            ConcreteSection section = new ConcreteSection(edges, dimensions,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
                (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            return section;

        }
    }
}
