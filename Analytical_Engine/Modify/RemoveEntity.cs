using BH.oM.Analytical.Elements;
using BH.oM.Base;
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

        [Description("Modifies a Graph by removing the specified entity and any dependent relations.")]
        [Input("graph", "The Graph to modify.")]
        [Input("entityToRemove", "The IBHoMObject entity to remove.")]
        [Output("graph", "The modified Graph with the specified entity and any dependent relations removed.")]

        public static void RemoveEntity(this Graph graph, IBHoMObject entityToRemove)
        {
            graph.RemoveEntity(entityToRemove.BHoM_Guid);
        }
        /***************************************************/

        [Description("Modifies a Graph by removing the specified entity and any dependent relations.")]
        [Input("graph", "The Graph to modify.")]
        [Input("entityToRemove", "The Guid of the entity to remove.")]
        [Output("graph", "The modified Graph with the specified entity and any dependent relations removed.")]

        public static void RemoveEntity(this Graph graph, Guid entityToRemove)
        {
            if (graph.Entities.ContainsKey(entityToRemove))
            {
                List<IRelation> relations = graph.Relations.FindAll(rel => rel.Source.Equals(entityToRemove) || rel.Target.Equals(entityToRemove)).ToList();
                graph.Relations = graph.Relations.Except(relations).ToList();
                graph.Entities.Remove(entityToRemove);
            }
            else
                Reflection.Compute.RecordWarning("Entity was not found in the graph.");
        }
    }
}
