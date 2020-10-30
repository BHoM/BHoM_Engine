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
        public static void ILayout(this Graph graph, ILayout layout)
        {
            
            graph.FindClusters();
            if (layout.ClusterPoints.Count() < m_Clusters.Count())
            {
                Reflection.Compute.RecordError("Insufficient cluster points provided to support the total clusters found.");
                return;
            }
            
            graph.AddProcessViewFragment(layout);

            Layout(layout as dynamic, graph);
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/
        private static void Layout(this Radial layout, Graph graph)
        {
            ProcessViewFragment processview = graph.FindFragment<ProcessViewFragment>();
            int count = 0;
            foreach(KeyValuePair<string, List<IBHoMObject>> kvp in m_Clusters)
            {
                EntityCluster entityCluster = processview.EntityClusters.Find(c => c.Name.Equals(kvp.Key));

                double radius = layout.Centre.Distance(layout.ClusterPoints[count]);
                Vector startVector = layout.ClusterPoints[count] - layout.Centre;
                double startAngle = Vector.XAxis.Angle(startVector);
                double theta = layout.SweepAngle / (kvp.Value.Count()-1);
                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    entityCluster.EntityGuids.Add(kvp.Value[j].BHoM_Guid);

                    EntityViewFragment view = graph.Entities[kvp.Value[j].BHoM_Guid].FindFragment<EntityViewFragment>();

                    double x = radius * Math.Cos(theta * j + startAngle);
                    double y = radius * Math.Sin(theta * j + startAngle);
                    view.Position = new Point() { X = x, Y = y, Z = 0 };
                    
                }
                count++;
            }
        }
        /***************************************************/
        private static void Layout(this Stacks layout, Graph graph)
        {
            LayoutColumnsStacks(layout, graph, Vector.XAxis, layout.HorizontalSpace);
        }
        /***************************************************/
        private static void Layout(this Columns layout, Graph graph)
        {
            LayoutColumnsStacks(layout, graph, Vector.YAxis, layout.VerticalSpace); 
        }
        /***************************************************/
        private static void LayoutColumnsStacks(ILayout layout, Graph graph, Vector direction, double space)
        {
            ProcessViewFragment processview = graph.FindFragment<ProcessViewFragment>();
            int count = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Clusters)
            {
                EntityCluster entityCluster = processview.EntityClusters.Find(c => c.Name.Equals(kvp.Key));

                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    EntityViewFragment view = graph.Entities[kvp.Value[j].BHoM_Guid].FindFragment<EntityViewFragment>();

                    entityCluster.EntityGuids.Add(kvp.Value[j].BHoM_Guid);

                    view.Position = layout.ClusterPoints[count] + direction * j * space;

                }
                count++;
            }
        }
        /***************************************************/

        private static void FindClusters(this Graph graph)
        {
            graph.CheckEntityViewFragments();
            m_Clusters = new SortedDictionary<string, List<IBHoMObject>>();
            foreach (IBHoMObject entity in graph.Entities.Values.ToList())
            {
                EntityViewFragment viewFrag = entity.FindFragment<EntityViewFragment>();

                if (m_Clusters.ContainsKey(viewFrag.ClusterName))
                    m_Clusters[viewFrag.ClusterName].Add(entity);
                else
                    m_Clusters[viewFrag.ClusterName] = new List<IBHoMObject>() { entity };
            }
            
        }
        /***************************************************/
        private static void CheckEntityViewFragments(this Graph graph)
        {
            List<IBHoMObject> entities = graph.Entities.Values.ToList();
            for (int i = 0; i < entities.Count(); i++)
            {
                IBHoMObject entity = entities[i];
                EntityViewFragment viewFrag = entity.FindFragment<EntityViewFragment>();

                if (viewFrag == null)
                {
                    viewFrag = new EntityViewFragment();
                    viewFrag.ClusterName = "New EntityViewFragments";
                    entity.Fragments.Add(viewFrag);

                    Reflection.Compute.RecordWarning("No EntityViewFragment found on entity :" + entity.Name + ". Entity has been assigned to \"New EntityViewFragments\" cluster.");

                }
            }
            
        }

        private static void AddProcessViewFragment(this Graph graph, ILayout layout)
        {
            ProcessViewFragment processViewFragment = new ProcessViewFragment();
            int i = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Clusters)
            {
                processViewFragment.EntityClusters.Add(new EntityCluster() { Name = kvp.Key, Postion = layout.ClusterPoints[i] });
                i++;
            }
            graph.Fragments.AddOrReplace(processViewFragment);
        }
        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static void Layout(this ILayout layout, Graph graph, Point origin, List<Point> clusterPoints)
        {
            // Do nothing
        }

        /***************************************************/

        private static SortedDictionary<string, List<IBHoMObject>> m_Clusters { get; set; }
    }
}
