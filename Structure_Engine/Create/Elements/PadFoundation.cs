using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Spatial.ShapeProfiles;
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

        [Description("Creates a PadFoundation from an outline, property and orientation angle.")]
        [Input("topOutline", "The outer edges of the pad. All section constants are derived based on the dimensions of this.")]
        [InputFromProperty("thickness")]
        [Input("orientationAngle", "The rotation to be applied the local X of the PadFoundation about the normal of the PadFoundation. This does not affect the geometry but can be used to define prinicpal directions for reinforcement.")]
        [Output("padFoundation", "The created PadFoundation with the property and orientation applied.")]
        public static PadFoundation PadFoundation(List<Edge> topOutline, ISurfaceProperty thickness = null, double orientationAngle = 0)
        {
            if (topOutline.IsNullOrEmpty() || topOutline.Any(x => x.IsNull()))
                return null;

            return new PadFoundation() { TopOutline = topOutline, Property = thickness, OrientationAngle = orientationAngle };
        }

        /***************************************************/

        [Description("Creates a rectangular PadFoundation and orients it to the coordinate system provided.")]
        [Input("width", "The width of the PadFoundation aligned with Global X.")]
        [Input("length", "The length of the PadFoundation aligned with Global Y.")]
        [InputFromProperty("thickness")]
        [Input("coordinateSystem", "The Cartesian coordinate system to control the position and orientation of the PadFoundation to which the PadFoundation is mapped to.")]
        [Input("orientationAngle", "The rotation to be applied the local X of the PadFoundation about the normal of the PadFoundation. This does not affect the geometry but can be used to define prinicpal directions for reinforcement.")]
        [Output("padFoundation", "The created PadFoundation with a rectangular outline mapped to the coordinate system provided.")]
        public static PadFoundation PadFoundation(double width, double length, ConstantThickness thickness, Cartesian coordinateSystem, double orientationAngle)
        {
            List<Edge> edges = Spatial.Create.RectangleProfile(length, width).Edges.Select(x => new Edge() { Curve = x }).ToList();

            edges.Select(x => x.Orient(Geometry.Create.CartesianCoordinateSystem(Point.Origin, Vector.XAxis, Vector.YAxis), coordinateSystem));

            return PadFoundation(edges, thickness, orientationAngle);
        }
    }
}