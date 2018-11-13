using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using BH.oM.Base;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static List<IElement1D> GetOutline(this Opening opening)
        {
            return opening.Edges.Select(e => e as IElement1D).ToList();
        }

        /***************************************************/

        public static List<IElement1D> GetOutline(this PanelPlanar panelPlanar)
        {
            return panelPlanar.ExternalEdges.Select(e => e as IElement1D).ToList();
        }

        /***************************************************/
    }
}
