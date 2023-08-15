using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.Elements;
using BH.oM.Structure.SurfaceProperties;
using BH.Engine.Base;
using BH.Engine.Geometry;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a PadFoundation from its fundamental parts and a basis.")]
        [Input("profile", "The section profile defining the edges of the pad. All section constants are derived based on the dimensions of this.")]
        [InputFromProperty("property")]
        [Input("coordinates", "The Cartesian coordinate system to control the position and orientation of the PadFoundation.")]
        public static PadFoundation PadFoundation(List<Edge> topSurface, ISurfaceProperty property = null, Basis orientation = null)
        {
            if (topSurface.IsNullOrEmpty() || topSurface.Any(x => x.IsNull()))
                return null;

            if (orientation == null)
                orientation = new Basis(new Vector() { X = 1 }, new Vector() { Y = 1 }, new Vector() { Z = 1 });

            return new PadFoundation() { TopSurface = topSurface, Property = property, Orientation = orientation };
        }

        /***************************************************/

        [Description("Creates a PadFoundation from its fundamental parts and a basis.")]
        [Input("width", "The width of the PadFoundation aligned with Global X.")]
        [Input("length", "The length of the PadFoundation aligned with Global Y.")]
        [InputFromProperty("property")]
        [Input("coordinates", "The Cartesian coordinate system to control the position and orientation of the PadFoundation.")]
        public static PadFoundation PadFoundation(double width, double length, ConstantThickness thickness, Basis orientation)
        {
            List<Edge> edges = Spatial.Create.RectangleProfile(length, width).Edges.Select(x => new Edge() { Curve = x }).ToList();

            return PadFoundation(edges, thickness, orientation);
        }
    }
}