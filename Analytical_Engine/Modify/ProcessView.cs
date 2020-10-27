using BH.Engine.Base;
using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Base;
using BH.oM.Geometry;
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
        public static Graph ProcessView(this Graph graph)
        {
            Graph clone = graph.DeepClone();
            clone.Entities.Values.ToList().ForEach(ent => ent.RemoveFragment(typeof(ProcessViewFragment)));
            IBHoMObject root = clone.AddRootEntity();

            Dictionary<Guid, int> depths = clone.DepthDictionary(root.BHoM_Guid);
            
            List<int> distinctDepths = depths.Values.Distinct().ToList();
            distinctDepths.Sort();
            distinctDepths.Reverse();
            double x = 0;
            foreach (int d in distinctDepths)
            {
                //all the entities at this level
                IEnumerable<Guid> level = depths.Where(kvp => kvp.Value == d).Select(kvp => kvp.Key);
                double y = 0;
                foreach (Guid entity in level)
                {
                   
                    ProcessViewFragment view = new ProcessViewFragment();

                    view.Position = Geometry.Create.Point(x, y, 0);
                    y--;
                    clone.Entities[entity] = clone.Entities[entity].AddFragment(view, true);
                }
                x++;
            }
            clone.RemoveRootEntity(root);
            return clone;
        }
        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/
        private static IBHoMObject AddRootEntity(this Graph graph)
        {
            m_OriginalSources = graph.Sources();
            IBHoMObject root = graph.Entities.Values.ToList()[0].DeepClone();
            root.BHoM_Guid = Guid.NewGuid();
            graph.Entities.Add(root.BHoM_Guid, root);
            foreach (Guid guid in m_OriginalSources)
            {
                Relation relation = new Relation()
                {
                    Source = root.BHoM_Guid,
                    Target = guid
                };
                graph.Relations.Add(relation);
            }
            return root;
        }
        /***************************************************/
        private static IBHoMObject RemoveRootEntity(this Graph graph, IBHoMObject root)
        {
            graph.Entities.Remove(root.BHoM_Guid);
            foreach (Guid guid in m_OriginalSources)
            {
                IRelation relation = graph.Relations.Find(rel => rel.Source.Equals(root.BHoM_Guid) && rel.Target.Equals(guid));
                graph.Relations.Remove(relation);
            }
            return root;
        }
        /***************************************************/
        /****           Private Fields                  ****/
        /***************************************************/
        private static List<Guid> m_OriginalSources { get; set; }
    }
}
