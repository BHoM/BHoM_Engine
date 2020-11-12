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
        public static void ILayout(this Graph graph, ILayout layout, List<string> groupsToIgnore = null)
        {

            graph.FindGroups(groupsToIgnore);
            if (layout.GroupPoints.Count() < m_Groups.Count())
            {
                Reflection.Compute.RecordError("Insufficient group points provided to support the total groups found.");
                return;
            }

            graph.AddLayoutHelperFragment(layout);

            Layout(layout as dynamic, graph);
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/
        private static void Layout(this Radial layout, Graph graph)
        {
            LayoutHelperFragment layoutHelper = graph.FindFragment<LayoutHelperFragment>();
            int count = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                EntityGroup entityGroup = layoutHelper.EntityGroups.Find(c => c.Name.Equals(kvp.Key));

                double radius = layout.Centre.Distance(layout.GroupPoints[count]);
                Vector startVector = layout.GroupPoints[count] - layout.Centre;
                double startAngle = Vector.XAxis.Angle(startVector);
                double theta = layout.SweepAngle / (kvp.Value.Count() - 1);
                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    entityGroup.EntityGuids.Add(kvp.Value[j].BHoM_Guid);

                    ProcessViewFragment view = graph.Entities[kvp.Value[j].BHoM_Guid].FindFragment<ProcessViewFragment>();

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
        private static void Layout(this Bubbles layout, Graph graph)
        {
            LayoutHelperFragment layoutHelper = graph.FindFragment<LayoutHelperFragment>();
            int count = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                EntityGroup entityGroup = layoutHelper.EntityGroups.Find(c => c.Name.Equals(kvp.Key));

                double theta = layout.SweepAngle / (kvp.Value.Count() - 1);
                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    entityGroup.EntityGuids.Add(kvp.Value[j].BHoM_Guid);

                    ProcessViewFragment view = graph.Entities[kvp.Value[j].BHoM_Guid].FindFragment<ProcessViewFragment>();

                    double x = layout.Radius * Math.Cos(theta * j) + layout.GroupPoints[count].X;
                    double y = layout.Radius * Math.Sin(theta * j) + layout.GroupPoints[count].Y;

                    view.Position = new Point() { X = x, Y = y, Z = 0 };

                }
                count++;
            }
        }
        /***************************************************/
        private static void LayoutColumnsStacks(ILayout layout, Graph graph, Vector direction, double space)
        {
            LayoutHelperFragment layoutHelper = graph.FindFragment<LayoutHelperFragment>();
            int count = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                EntityGroup entityGroup = layoutHelper.EntityGroups.Find(c => c.Name.Equals(kvp.Key));

                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    ProcessViewFragment view = graph.Entities[kvp.Value[j].BHoM_Guid].FindFragment<ProcessViewFragment>();

                    entityGroup.EntityGuids.Add(kvp.Value[j].BHoM_Guid);

                    view.Position = layout.GroupPoints[count] + direction * j * space;

                }
                count++;
            }
        }
        /***************************************************/

        private static void FindGroups(this Graph graph, List<string> groupsToIgnore)
        {
            graph.CheckProcessViewFragments();
            m_Groups = new SortedDictionary<string, List<IBHoMObject>>();
            foreach (IBHoMObject entity in graph.Entities.Values.ToList())
            {
                ProcessViewFragment viewFrag = entity.FindFragment<ProcessViewFragment>();
                List<string> groupNames = new List<string>();
                foreach (string group in viewFrag.GroupNames)
                {
                    if (!groupsToIgnore.Contains(group))
                        groupNames.Add(group);
                }
                string groupName = string.Join("_", groupNames.Where(s => !string.IsNullOrWhiteSpace(s)).ToList());
                if (m_Groups.ContainsKey(groupName))
                    m_Groups[groupName].Add(entity);
                else
                    m_Groups[groupName] = new List<IBHoMObject>() { entity };
            }

        }
        /***************************************************/
        private static void CheckProcessViewFragments(this Graph graph)
        {
            List<IBHoMObject> entities = graph.Entities.Values.ToList();
            for (int i = 0; i < entities.Count(); i++)
            {
                IBHoMObject entity = entities[i];
                ProcessViewFragment viewFrag = entity.FindFragment<ProcessViewFragment>();

                if (viewFrag == null)
                {
                    viewFrag = new ProcessViewFragment();
                    viewFrag.GroupNames.Add("New ProcessViewFragments");
                    entity.Fragments.Add(viewFrag);

                    Reflection.Compute.RecordWarning("No ProcessViewFragment found on entity :" + entity.Name + ". Entity has been assigned to \"New EntityViewFragments\" group.");

                }
            }

        }
        /***************************************************/
        private static void AddLayoutHelperFragment(this Graph graph, ILayout layout)
        {
            LayoutHelperFragment processViewFragment = new LayoutHelperFragment();
            int i = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                processViewFragment.EntityGroups.Add(new EntityGroup() { Name = kvp.Key, Position = layout.GroupPoints[i] });
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

        private static SortedDictionary<string, List<IBHoMObject>> m_Groups { get; set; }
    }
}