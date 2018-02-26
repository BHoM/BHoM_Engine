using System.Collections.Generic;
using System.Collections.ObjectModel;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;
using BH.oM.Common.Materials;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConcreteSection ConcreteRectangleSection(double height, double width, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromDimensions(new RectangleSectionDimensions(height, width, 0), material, name, reinforcement);
        }

        /***************************************************/

        public static ConcreteSection ConcreteTeeSection(double height, double webThickness, double flangeWidth, double flangeThickness, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromDimensions(new StandardTeeSectionDimensions(height, flangeWidth, webThickness, flangeThickness, 0, 0), material, name, reinforcement);
        }


        /***************************************************/

        public static ConcreteSection ConcreteCircularSection(double diameter, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromDimensions(new CircleDimensions(diameter), material, name, reinforcement);
        }

        /***************************************************/

        public static ConcreteSection ConcreteFreeFormSection(List<ICurve> edges, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {

            Dictionary<string, object> constants = Geometry.Compute.Integrate(edges);

            constants["J"] = 0;
            constants["Iw"] = 0;

            ConcreteSection section = new ConcreteSection(edges, new PolygonDimensions(),
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
                (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            //section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            //section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            if (material != null)
                section.Name = name;

            if (material != null)
                section.Material = material;

            if (reinforcement != null)
                section.Reinforcement = reinforcement;


            return section;
        }

        /***************************************************/

        public static ConcreteSection ConcreteSectionFromDimensions(ISectionDimensions dimensions, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            List<ICurve> edges = dimensions.IGetEdgeCUrves();
            Dictionary<string, object> constants = Geometry.Compute.Integrate(edges);

            constants["J"] = dimensions.ITorsionalConstant();
            constants["Iw"] = dimensions.IWarpingConstant();

            ConcreteSection section = new ConcreteSection(edges, dimensions,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
                (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            //section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            //section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            if (material != null)
                section.Name = name;

            if (material != null)
                section.Material = material;

            if (reinforcement != null)
                section.Reinforcement = reinforcement;

            return section;
        }

        /***************************************************/
    }
}
