using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Geometry;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural PileFoundation from a pile cap and pile groups.")]
        [Input("pileCap", "The pile cap defining the outer edge of the PileFoundation and location in 3D.")]
        [Input("pileGroups", "Groups of piles contained within the outline of the pile cap.")]
        public static PileFoundation PileFoundation(PadFoundation pileCap, List<PileGroup> pileGroups)
        {
            return new PileFoundation() { PileCap = pileCap, PileGroups = pileGroups };
        }

        /***************************************************/

        [Description("Creates a structural PileFoundation from the pile groups, an offset and the PileCap properties.")]
        [Input("pileGroups", "Groups of piles used to define the outline of the PileCap.")]
        [InputFromProperty("property")]
        [Input("offset", "The offset from the centre of the piles to the edge of the PileCap.")]
        [Input("coordinates", "The Cartesian coordinate system to control the position and orientation of the PadFoundation.")]
        public static PileFoundation PileFoundation(List<PileGroup> pileGroups, ConstantThickness property, double offset, Basis orientation)
        {
            List<Point> pts = new List<Point>();

            foreach (PileGroup pileGroup in pileGroups)
            {
                pts.AddRange(pileGroup.PileLayout.Points);
            }

            pts.Add(pts.First());

            List<Edge> edges = new List<Edge>() { new Edge() { Curve = Geometry.Create.Polyline(Geometry.Compute.GrahamScan(pts)).Offset(offset) } };

            return new PileFoundation() { PileCap = PadFoundation(edges, property, orientation), PileGroups = pileGroups };
        }

        /***************************************************/

    }
}