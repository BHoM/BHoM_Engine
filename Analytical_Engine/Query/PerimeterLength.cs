using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************/

        public static double PerimeterLength<TEdge, TOpening>(this IPanel<TEdge, TOpening> panel) where TEdge : IEdge where TOpening : IOpening<TEdge>
        {
            return panel.ExternalEdges.Sum(x => x.Curve.ILength());
        }

        /***************************************/
    }
}
