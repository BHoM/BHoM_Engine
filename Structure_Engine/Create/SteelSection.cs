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

        public static SteelSection SteelISection(double height, double webThickness, double flangeWidth, double flangeThickness, double rootRadius = 0, double toeRadius = 0, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(ISectionProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius), material, name);
        }

        /***************************************************/

        public static SteelSection SteelFabricatedISection(double height, double webThickness, double topFlangeWidth, double topFlangeThickness, double botFlangeWidth, double botFlangeThickness,  double weldSize, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(FabricatedISectionProfile(height, topFlangeWidth, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize), material, name);
        }

        /***************************************************/

        public static SteelSection SteelBoxSection(double height, double width, double thickness, double innerRadius = 0, double outerRadius = 0, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(BoxProfile(height, width, thickness, innerRadius, outerRadius), material, name);
        }

        /***************************************************/

        public static SteelSection FabricatedSteelBoxSection(double height, double width, double webThickness, double flangeThickness, double weldSize, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(FabricatedBoxProfile(height, width, webThickness, flangeThickness, flangeThickness, weldSize), material, name);


        }

        /***************************************************/

        public static SteelSection SteelTubeSection(double diameter, double thickness, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(TubeProfile(diameter, thickness), material, name);
        }

        /***************************************************/

        public static SteelSection SteelRectangleSection(double height, double width, double cornerRadius=0, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(RectangleProfile(height, width, cornerRadius), material, name);
        }

        /***************************************************/

        public static SteelSection SteelCircularSection(double diameter, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(CircleProfile(diameter), material, name);
        }

        /***************************************************/

        public static SteelSection SteelTSection(double height, double webThickness, double flangeWidth, double flangeThickness,  double rootRadius = 0, double toeRadius = 0, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(TSectionProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius), material, name);

        }

        /***************************************************/

        public static SteelSection SteelAngleSection(double height, double webThickness, double width, double flangeThickness, double rootRadius = 0, double toeRadius = 0, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(AngleProfile(height, width, webThickness, flangeThickness, rootRadius, toeRadius), material, name);
        }

        /***************************************************/

        public static SteelSection SteelFreeFormSection(List<ICurve> edges, Material material = null, string name = null)
        {
            return SteelSectionFromDimensions(FreeFormProfile(edges), material, name);
        }

        /***************************************************/

        public static SteelSection SteelSectionFromDimensions(IProfile dimensions, Material material = null, string name = "")
        {

            List<ICurve> edges = dimensions.Edges.ToList();
            Dictionary<string, object> constants = Geometry.Compute.Integrate(edges);

            constants["J"] = dimensions.ITorsionalConstant();
            constants["Iw"] = dimensions.IWarpingConstant();

            SteelSection section = new SteelSection(dimensions,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"],
                (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            //section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            //section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            if(material != null)
                section.Name = name;

            if (material != null)
                section.Material = material;
            return section;

        }

        /***************************************************/
    }
}
