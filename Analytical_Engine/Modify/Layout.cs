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
        public static void ILayout(this ILayout layout, Graph graph, Point origin, List<Point> clusterPoints, List<string> ignoreClusters)
        {
            
            graph.FindClusters(ignoreClusters);
            if (clusterPoints.Count() < m_Clusters.Count())
            {
                Reflection.Compute.RecordError("Insufficient cluster points provided to support the total clusters found.");
                return;
            }
            
            graph.Entities.Values.ToList().ForEach(ent => ent.RemoveFragment(typeof(ProcessViewFragment)));

            Layout(layout as dynamic,graph, origin, clusterPoints);
        }

        /***************************************************/
        public static void Layout(this Radial layout, Graph graph, Point origin, List<Point> clusterPoints)
        {
            int count = 0;
            foreach(KeyValuePair<string, List<IBHoMObject>> kvp in m_Clusters)
            {
                double radius = origin.Distance(clusterPoints[count]);
                Vector startVector = clusterPoints[count] - origin;
                double startAngle = Vector.XAxis.Angle(startVector);
                double theta = layout.SweepAngle / kvp.Value.Count();
                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    ProcessViewFragment view = new ProcessViewFragment();
                    double x = radius * Math.Cos(theta * j + startAngle);
                    double y = radius * Math.Sin(theta * j + startAngle);
                    view.Position = new Point() { X = x, Y = y, Z = 0 };
                    graph.Entities[kvp.Value[j].BHoM_Guid] = graph.Entities[kvp.Value[j].BHoM_Guid].AddFragment(view);
                }
                count++;
            }
        }
        /***************************************************/
        public static void Layout(this Stack layout, Graph graph, Point origin, List<Point> clusterPoints)
        {
            int count = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Clusters)
            {
               
                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    ProcessViewFragment view = new ProcessViewFragment();
                    view.Position = clusterPoints[count] + Vector.XAxis * j * layout.HorizontalSpace;
                    graph.Entities[kvp.Value[j].BHoM_Guid] = graph.Entities[kvp.Value[j].BHoM_Guid].AddFragment(view);
                }
                count++;
            }
        }
        /***************************************************/
        public static void Layout(this Columns layout, Graph graph, Point origin, List<Point> clusterPoints)
        {
            int count = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Clusters)
            {

                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    ProcessViewFragment view = new ProcessViewFragment();
                    view.Position = clusterPoints[count] + Vector.YAxis * j * layout.VerticalSpace;
                    graph.Entities[kvp.Value[j].BHoM_Guid] = graph.Entities[kvp.Value[j].BHoM_Guid].AddFragment(view);
                }
                count++;
            }
        }
        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        private static void Layout(this ILayout layout, Graph graph, Point origin, List<Point> clusterPoints)
        {
            // Do nothing
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/

        private static void FindClusters(this Graph graph, List<string> ignoreClusters)
        {
            m_Clusters = new Dictionary<string, List<IBHoMObject>>();
            foreach (IBHoMObject entity in graph.Entities.Values.ToList())
            {
                ClusterFragment clusterFrag = entity.FindFragment<ClusterFragment>();
                if (clusterFrag == null)
                {
                    Reflection.Compute.RecordWarning("No cluster found on entity :" + entity.Name);
                    continue;
                }

                if (ignoreClusters.Contains(clusterFrag.ClusterName))
                    continue;

                if (m_Clusters.ContainsKey(clusterFrag.ClusterName))
                    m_Clusters[clusterFrag.ClusterName].Add(entity);
                else
                    m_Clusters[clusterFrag.ClusterName] = new List<IBHoMObject>() { entity };
            }
            
        }

        /***************************************************/

        private static Dictionary<string, List<IBHoMObject>> m_Clusters { get; set; }
    }
}
