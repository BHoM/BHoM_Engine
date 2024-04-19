using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Facade.Elements;
using BH.oM.Geometry;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Facade
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the geometry of a CurtainWall at its centre. Method required for automatic display in UI packages.")]
        [Input("curtainWall", "CurtainWall to get the planar surface geometry from.")]
        [Output("surface", "The geometry of the CurtainWall at its centre.")]
        public static PlanarSurface Geometry(this CurtainWall curtainWall)
        {
            return Engine.Geometry.Create.PlanarSurface(curtainWall?.ExternalEdges?.Select(x => x?.Curve).ToList().IJoin().FirstOrDefault());
        }

        /***************************************************/

        [Description("Gets the geometry of a facade Opening as an outline curve. Method required for automatic display in UI packages.")]
        [Input("opening", "Facade Opening to get the outline geometry from.")]
        [Output("outline", "The geometry of the facade Opening.")]
        public static PolyCurve Geometry(this Opening opening)
        {
            return new PolyCurve { Curves = opening?.Edges?.Select(x => x?.Curve).ToList().IJoin().Cast<ICurve>().ToList() };
        }

        /***************************************************/
    }
}
