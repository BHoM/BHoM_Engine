using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Data.Library;
using BH.oM.Graphics;
using BH.oM.Graphics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        public static List<IRepresentation> Representation(IView view, Graph graph)
        {

            Graph processGraph = graph.DeepClone();

            Dataset graphData = Convert.ToDataset(processGraph, view);

            return Graphics.Create.IView(view, graphData);
        }
    }
}
