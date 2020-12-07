using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Graphics.Components;
using BH.oM.Graphics.Scales;
using BH.oM.Graphics.Views;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Adds representation fragments to IBHoMObjects.")]
        [Input("component", "The configuration properties for the representation.")]
        [Input("dataset", "Dataset of a BH.oM.Analytical.Elements.Graph where Graph.Entities are one element of type BHoMGroup in Dataset.Data and Graph.Relations are another element of type BHoMGroup in Dataset.Data.")]
        [Input("viewConfig", "The configuration properties for the view.")]
        public static void IRepresentationFragment(this BH.oM.Graphics.Components.IComponent component, Dataset dataset, ViewConfig viewConfig)
        {
            RepresentationFragment(component as dynamic, dataset, viewConfig);
        }

        /***************************************************/
        /****           Fallback Methods                ****/
        /***************************************************/
        private static void RepresentationFragment(this BH.oM.Graphics.Components.IComponent component, Dataset dataset, ViewConfig viewConfig)
        {

        }
    }
}
