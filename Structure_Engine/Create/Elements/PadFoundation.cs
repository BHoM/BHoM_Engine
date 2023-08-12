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

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a structural PadFoundation from its fundamental parts and a basis.")]
        [Input("profile", "The section profile defining the edges of the pad. All section constants are derived based on the dimensions of this.")]
        [InputFromProperty("property")]
        [Input("coordinates", "The Cartesian coordinate system to control the position and orientation of the PadFoundation.")]
        public static PadFoundation PadFoundation(IProfile topSurface, ISurfaceProperty property, Basis orientation)
        {
            return new PadFoundation() { TopSurface = topSurface, Property = property, Orientation = orientation };
        }

        /***************************************************/

        [Description("Creates a structural Panel from its fundamental parts and an local orientation vector.")]
        [Input("width", "The width of the PadFoundation aligned with Global X.")]
        [Input("length", "The length of the PadFoundation aligned with Global Y.")]
        [InputFromProperty("property")]
        [Input("coordinates", "The Cartesian coordinate system to control the position and orientation of the PadFoundation.")]
        public static PadFoundation PadFoundation(double width, double length, ConstantThickness thickness, Basis orientation)
        {
            return PadFoundation(Engine.Spatial.Create.RectangleProfile(length, width), thickness, orientation);
        }
    }
}