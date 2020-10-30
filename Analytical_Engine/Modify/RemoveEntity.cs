using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
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
        public static void RemoveEntity(this Graph graph, IBHoMObject entityToRemove)
        {
            graph.RemoveEntity(entityToRemove.BHoM_Guid);
        }
        /***************************************************/
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
