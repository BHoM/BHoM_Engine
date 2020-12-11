using BH.Engine.Reflection;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Graphics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Convert
    {
        public static Dataset ToDataset(this Graph graph, IView view)
        {
            if (view is DependencyChart)
            {
                DependencyChart chart = view as DependencyChart;
                string groupPropertName = chart.Boxes.Group;
                foreach (IBHoMObject entity in graph.Entities())
                {
                    if (chart.Boxes.GroupsToIgnore.Contains(entity.PropertyValue(groupPropertName)))
                        graph.RemoveEntity(entity);
                }
            }

            BHoMGroup<IBHoMObject> entities = new BHoMGroup<IBHoMObject>();
            entities.Elements = graph.Entities();
            entities.Name = "Entities";

            BHoMGroup<IBHoMObject> relations = new BHoMGroup<IBHoMObject>();
            relations.Elements = graph.Relations.Cast<IBHoMObject>().ToList();
            relations.Name = "Relations";

            Dataset graphData = new Dataset();
            graphData.Data = new List<IBHoMObject>();
            graphData.Data.Add(entities);
            graphData.Data.Add(relations);

            return graphData;

        }
    }
}
