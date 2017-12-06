using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Structural;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
using BH.oM.Materials;
using BH.oM.Geometry;
using BH.oM.Base;


namespace BH.Engine.Structure
{
    public static partial class Create
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static SteelSection SteelISection(double tft, double tfw, double bft, double bfw, double wt, double wd, double r1=0, double r2=0)
        {
            List<ICurve> edges = ISecctionCurves(tft, tfw, bft, bfw, wt, wd, r1, r2);

            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            constants["J"] = Query.TorsionalConstant(ShapeType.ISection, (double)constants["TotalDepth"], (double)constants["TotalWidth"], tfw, bfw, tft, bft, wt);
            constants["Iw"] = Query.WarpingConstant(ShapeType.ISection, (double)constants["TotalDepth"], (double)constants["TotalWidth"], tfw, bfw, tft, bft, wt);

           return SteelSectionCreationHelper(edges, ShapeType.ISection, tfw, bfw, 0, wt, tft, bft, r1, r2, 0, constants);


        }

        /***************************************************/

        public static SteelSection SteelBoxSection(double height, double width, double webThickness, double flangeThickness, double innerRadius=0, double outerRadius=0)
        {
            List<ICurve> edges = BoxSectionCurves(width, height, webThickness, flangeThickness, innerRadius, outerRadius);

            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            constants["J"] = Query.TorsionalConstant(ShapeType.Box, (double)constants["TotalDepth"], (double)constants["TotalWidth"], width, 0, flangeThickness, flangeThickness, webThickness);
            constants["Iw"] = Query.WarpingConstant(ShapeType.Box, (double)constants["TotalDepth"], (double)constants["TotalWidth"], width, 0, flangeThickness, flangeThickness, webThickness);

            return SteelSectionCreationHelper(edges, ShapeType.Box, width, 0, 0, webThickness, flangeThickness, 0, innerRadius, outerRadius, 0, constants);


        }

        /***************************************************/

        public static SteelSection SteelTubeSection(double diameter, double thickness)
        {
            List<ICurve> edges = TubeSectionCurves(diameter / 2, thickness);

            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            constants["J"] = Query.TorsionalConstant(ShapeType.Tube, (double)constants["TotalDepth"], (double)constants["TotalWidth"], diameter, 0, thickness, thickness, thickness);
            constants["Iw"] = Query.WarpingConstant(ShapeType.Tube, (double)constants["TotalDepth"], (double)constants["TotalWidth"], diameter, 0, thickness, thickness, thickness);

            return SteelSectionCreationHelper(edges, ShapeType.Tube, diameter, 0, 0, thickness, thickness, 0, 0, 0, 0, constants);


        }

        /***************************************************/

        public static SteelSection SteelRectangleSection(double height, double width, double radius=0)
        {
            List<ICurve> edges = RectangleSectionCurves(width, height, radius);

            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            constants["J"] = Query.TorsionalConstant(ShapeType.Rectangle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], 0, 0, 0, 0,0);
            constants["Iw"] = Query.WarpingConstant(ShapeType.Rectangle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], 0, 0, 0, 0, 0);

            return SteelSectionCreationHelper(edges, ShapeType.Rectangle, width, height, 0, 0, 0, 0, radius, 0, 0, constants);


        }

        /***************************************************/

        public static SteelSection SteelCircularSection(double diameter)
        {
            List<ICurve> edges = CircleSectionCurves(diameter / 2);

            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            constants["J"] = Query.TorsionalConstant(ShapeType.Circle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], diameter, 0, 0, 0, 0);
            constants["Iw"] = Query.WarpingConstant(ShapeType.Circle, (double)constants["TotalDepth"], (double)constants["TotalWidth"], diameter, 0, 0, 0, 0);

            return SteelSectionCreationHelper(edges, ShapeType.Circle, diameter, 0, 0, 0, 0, 0, 0, 0, 0, constants);


        }

        /***************************************************/

        /***************************************************/

        public static SteelSection SteelFreeFormSection(List<ICurve> edges)
        {
            Dictionary<string, object> constants = Query.IntegrateCurve(edges);

            constants["J"] = Query.TorsionalConstant(ShapeType.Polygon, (double)constants["TotalDepth"], (double)constants["TotalWidth"], 0, 0, 0, 0, 0);
            constants["Iw"] = Query.WarpingConstant(ShapeType.Polygon, (double)constants["TotalDepth"], (double)constants["TotalWidth"], 0, 0, 0, 0, 0);

            return SteelSectionCreationHelper(edges, ShapeType.Polygon, 0, 0, 0, 0, 0, 0, 0, 0, 0, constants);


        }

        /***************************************************/


        private static SteelSection SteelSectionCreationHelper(IEnumerable<ICurve> edges, ShapeType shape,
            double b1,
            double b2,
            double b3,
            double tw,
            double tf1,
            double tf2,
            double r1,
            double r2,
            double spacing,
            Dictionary<string, object> constants)
        {
            SteelSection section =  new SteelSection(edges, shape, b1, b2, b3, tw, tf1, tf2, r1, r2, spacing,
                (double)constants["Area"], (double)constants["Rgy"], (double)constants["Rgz"], (double)constants["J"], (double)constants["Iy"], (double)constants["Iz"], (double)constants["Iw"], 
                (double)constants["Zy"], (double)constants["Zz"], (double)constants["Sy"], (double)constants["Sz"], (double)constants["CentreZ"], (double)constants["CentreY"], (double)constants["Vz"], 
                (double)constants["Vpz"], (double)constants["Vy"], (double)constants["Vpy"], (double)constants["Asy"], (double)constants["Asz"], (double)constants["TotalDepth"], (double)constants["TotalWidth"]);

            section.CustomData["VerticalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["VerticalSlices"]);
            section.CustomData["HorizontalSlices"] = new ReadOnlyCollection<IntegrationSlice>((List<IntegrationSlice>)constants["HorizontalSlices"]);

            return section;

        }

    }
}
