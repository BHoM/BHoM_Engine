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
            return new PileFoundation() { PileCap = pileCap, Piles = piles };
        }

        /***************************************************/

        [Description("Creates a structural PileFoundation from the piles, an offset and the PileCap properties using the GrahamScan method to determine the pile cap outline.")]
        [Input("piles", "One or more Piles used to define the outline of the PileCap.")]
        [Input("offset", "The offset from the centre of the piles to the edge of the PileCap.")]
        [InputFromProperty("property")]
        [InputFromProperty("orientationAngle")]
        [Output("pileFoundation", "The created PileFoundation containing the dervied pile cap and pile elements.")]
        public static PileFoundation PileFoundation(List<Pile> piles, double offset, ConstantThickness property = null, double orientationAngle = 0)
        {
            List<Point> pts = new List<Point>();

            foreach (Pile pile in piles)
            {
                pts.Add(pile.TopNode.Position);
            }

            List<Point> convexHull = Geometry.Compute.GrahamScan(pts);
            convexHull.Add(convexHull[0]);

            List<Edge> edges = new List<Edge>() { new Edge() { Curve = Geometry.Create.Polyline(convexHull).Offset(offset) } };

            return new PileFoundation() { PileCap = PadFoundation(edges, property, orientationAngle), Piles = piles };
        }

        /***************************************************/

    }
}