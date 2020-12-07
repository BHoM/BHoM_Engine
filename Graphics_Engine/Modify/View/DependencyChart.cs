using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Graphics;
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

        [Description("Modifies a dataset by adding representation fragments to define a view of the data.")]
        [Input("view", "The configuration properties for the view representation.")]
        [Input("dataset", "Dataset of a BH.oM.Analytical.Elements.Graph where Graph.Entities are one element of type BHoMGroup in Dataset.Data and Graph.Relations are another element of type BHoMGroup in Dataset.Data.")]
        public static void View(this DependencyChart view, Dataset dataset)
        {

            view.Boxes.IRepresentationFragment(dataset, view.ViewConfig);

            view.Links.IRepresentationFragment(dataset, view.ViewConfig);
        }
       
    }
}
