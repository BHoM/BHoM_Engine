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
            return SteelSectionFromProfile(ISectionProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius), material, name);
        }

        /***************************************************/

        public static SteelSection SteelFabricatedISection(double height, double webThickness, double topFlangeWidth, double topFlangeThickness, double botFlangeWidth, double botFlangeThickness,  double weldSize, Material material = null, string name = null)
        {
            return SteelSectionFromProfile(FabricatedISectionProfile(height, topFlangeWidth, botFlangeWidth, webThickness, topFlangeThickness, botFlangeThickness, weldSize), material, name);
        }

        /***************************************************/

        public static SteelSection SteelBoxSection(double height, double width, double thickness, double innerRadius = 0, double outerRadius = 0, Material material = null, string name = null)
        {
            return SteelSectionFromProfile(BoxProfile(height, width, thickness, innerRadius, outerRadius), material, name);
        }

        /***************************************************/

        public static SteelSection FabricatedSteelBoxSection(double height, double width, double webThickness, double flangeThickness, double weldSize, Material material = null, string name = null)
        {
            return SteelSectionFromProfile(FabricatedBoxProfile(height, width, webThickness, flangeThickness, flangeThickness, weldSize), material, name);


        }

        /***************************************************/

        public static SteelSection SteelTubeSection(double diameter, double thickness, Material material = null, string name = null)
        {
            return SteelSectionFromProfile(TubeProfile(diameter, thickness), material, name);
        }

        /***************************************************/

        public static SteelSection SteelRectangleSection(double height, double width, double cornerRadius=0, Material material = null, string name = null)
        {
            return SteelSectionFromProfile(RectangleProfile(height, width, cornerRadius), material, name);
        }

        /***************************************************/

        public static SteelSection SteelCircularSection(double diameter, Material material = null, string name = null)
        {
            return SteelSectionFromProfile(CircleProfile(diameter), material, name);
        }

        /***************************************************/

        public static SteelSection SteelTSection(double height, double webThickness, double flangeWidth, double flangeThickness,  double rootRadius = 0, double toeRadius = 0, Material material = null, string name = null)
        {
            return SteelSectionFromProfile(TSectionProfile(height, flangeWidth, webThickness, flangeThickness, rootRadius, toeRadius), material, name);

        }

        /***************************************************/

        public static SteelSection SteelAngleSection(double height, double webThickness, double width, double flangeThickness, double rootRadius = 0, double toeRadius = 0, Material material = null, string name = null)
        {
            return SteelSectionFromProfile(AngleProfile(height, width, webThickness, flangeThickness, rootRadius, toeRadius), material, name);
        }

        /***************************************************/

        public static SteelSection SteelFreeFormSection(List<ICurve> edges, Material material = null, string name = null)
        {
            return SteelSectionFromProfile(FreeFormProfile(edges), material, name);
        }

        /***************************************************/

        public static SteelSection SteelSectionFromProfile(IProfile profile, Material material = null, string name = "")
        {

            List<ICurve> edges = profile.Edges.ToList();
            Dictionary<string, object> constants = Geometry.Compute.Integrate(edges);

            constants["J"] = profile.ITorsionalConstant();
            constants["Iw"] = profile.IWarpingConstant();

            SteelSection section = new SteelSection(profile,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"], (double)constants["Wely"],
                (double)constants["Welz"], (double)constants["Wply"], (double)constants["Wplz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"],
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"]);

            //section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            //section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            section.Material = material == null ? Query.Default(MaterialType.Steel) : material;

            if(name != null)
                section.Name = name;

            return section;

        }

        /***************************************************/
    }
}
