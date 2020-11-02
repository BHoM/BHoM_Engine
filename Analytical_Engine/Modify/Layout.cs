using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Constructors             ****/
        /***************************************************/

        [Description("Modifies a Graph by configuring ViewFragments for each entity.")]
        [Input("graph", "The Graph to modify.")]
        [Input("layout", "ILayout for the view of the of the Graph.")]

        public static void ILayout(this Graph graph, ILayout layout)
        {
            Layout(layout as dynamic, graph);
        }


        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static void Layout(this ILayout layout, Graph graph)
        {
            // Do nothing
        }

        /***************************************************/

    }
}
