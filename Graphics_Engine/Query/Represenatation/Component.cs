using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Graphics;
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
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates representations.")]
        [Input("component", "The configuration properties for the representation.")]
        [Input("dataset", "Dataset to generate a view of.")]
        [Input("viewConfig", "The configuration properties for the view.")]
        public static List<IRepresentation> IComponent(this BH.oM.Graphics.Components.IComponent component, Dataset dataset, ViewConfig viewConfig)
        {
            return Component(component as dynamic, dataset, viewConfig);
        }

        /***************************************************/
        /****           Fallback Methods                ****/
        /***************************************************/
        private static List<IRepresentation> Component(this BH.oM.Graphics.Components.IComponent component, Dataset dataset, ViewConfig viewConfig)
        {
            return new List<IRepresentation>();
        }
    }
}