using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.Layouts;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.Engine.Spatial;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural PileFoundation from a pile cap and piles.")]
        [Input("pileCap", "The pile cap defining the outer edge of the PileFoundation and location in 3D.")]
        [Input("piles", "Piles contained within the outline of the pile cap.")]
        [Output("pileFoundation", "The created PileFoundation containing the pile cap and pile elements.")]
        public static PileFoundation PileFoundation(PadFoundation pileCap, List<Pile> piles)
        {
            if (piles.IsNullOrEmpty())
            {
                Base.Compute.RecordError("The pile list is null or empty.");
                return null;
            }

            if (piles.Any(x => x.IsNull()))
                return null;

            if (pileCap.IsNull())
                return null;

            return new PileFoundation() { PileCap = pileCap, Piles = piles };
        }

        /***************************************************/

        [Description("Creates a structural PileFoundation from the piles, an offset and the PileCap properties using the GrahamScan method to determine the pile cap outline.")]
        [Input("piles", "One or more Piles used to define the outline of the PileCap.")]
        [Input("offset", "The offset from the centre of the piles to the edge of the PileCap.")]
        [InputFromProperty("property")]
        [InputFromProperty("orientationAngle")]
        [Output("pileFoundation", "The created PileFoundation containing the dervied pile cap and pile elements.")]
        public static PileFoundation PileFoundation(List<Pile> piles, double offset = 0, ConstantThickness property = null, double orientationAngle = 0)
        {
            List<Point> pts = new List<Point>();

            foreach (Pile pile in piles)
            {
                pts.Add(pile.TopNode.Position);
            }

            List<Point> convexHull = Geometry.Compute.GrahamScan(pts);
            convexHull.Add(convexHull[0]);

            return PileFoundation(PadFoundation((PolyCurve)Geometry.Create.Polyline(convexHull).Offset(offset), property, orientationAngle), piles);
        }

        /***************************************************/

    }
}