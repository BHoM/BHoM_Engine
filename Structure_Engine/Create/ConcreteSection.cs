using System.Collections.Generic;
using System.Collections.ObjectModel;
using BH.oM.Structural.Properties;
using BH.oM.Geometry;
using BH.oM.Common.Materials;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConcreteSection ConcreteRectangleSection(double height, double width, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(RectangleProfile(height, width, 0), material, name, reinforcement);
        }

        /***************************************************/

        public static ConcreteSection ConcreteTSection(double height, double webThickness, double flangeWidth, double flangeThickness, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(TSectionProfile(height, flangeWidth, webThickness, flangeThickness, 0, 0), material, name, reinforcement);
        }


        /***************************************************/

        public static ConcreteSection ConcreteCircularSection(double diameter, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(CircleProfile(diameter), material, name, reinforcement);
        }

        /***************************************************/

        public static ConcreteSection ConcreteFreeFormSection(List<ICurve> edges, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            return ConcreteSectionFromProfile(FreeFormProfile(edges), material, name, reinforcement);
        }

        /***************************************************/

        public static ConcreteSection ConcreteSectionFromProfile(IProfile profile, Material material = null, string name = "", List<Reinforcement> reinforcement = null)
        {
            List<ICurve> edges = profile.Edges.ToList();
            Dictionary<string, object> constants = Geometry.Compute.Integrate(edges, Tolerance.MicroDistance);

            constants["J"] = profile.ITorsionalConstant();
            constants["Iw"] = profile.IWarpingConstant();

            ConcreteSection section = new ConcreteSection(profile,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
                (double)constants["Wely"], (double)constants["Welz"], (double)constants["Wply"], (double)constants["Wplz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            //section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            //section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            section.Material = material == null ? Query.Default(MaterialType.Concrete) : material;

            if (name != null)
                section.Name = name;

            if (reinforcement != null)
                section.Reinforcement = reinforcement;

            return section;
        }

        /***************************************************/
    }
}
