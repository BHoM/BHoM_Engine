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
        public static Graph ILayout(this Graph graph, ProcessView view)
        {
               
            graph.FindGroups(view.ViewConfig.GroupsToIgnore);

            if (view.Layout.GroupPoints.Count() < m_Groups.Count())
            {
                Reflection.Compute.RecordError("Insufficient group points provided to support the total groups found.");
                return graph;
            }
            m_ProcessView = view;
            m_PaddingX = Vector.XAxis * view.ViewConfig.Padding;
            m_PaddingY = Vector.YAxis * view.ViewConfig.Padding;

            graph.SetUpGraphViewFragment();
            Layout(view.Layout as dynamic, graph);

            return graph;
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/
        private static void Layout(this Radial layout, Graph graph)
        {
            int count = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                EntityGroup group = m_GraphViewFragment.EntityGroups.Find(x => x.Name == kvp.Key);
                group.LabelPosition = layout.GroupPoints[count] - 2*m_PaddingX -2* m_PaddingY;

                double radius = layout.Centre.Distance(layout.GroupPoints[count]);
                Vector startVector = layout.GroupPoints[count] - layout.Centre;
                double startAngle = Vector.XAxis.Angle(startVector);
                double theta = layout.SweepAngle / (kvp.Value.Count() - 1);
                double endAngle = 0;
                double paddingAngle = Math.Asin( m_ProcessView.ViewConfig.Padding / 2 / radius) * 2;
                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    EntityViewFragment view = graph.Entities[kvp.Value[j].BHoM_Guid].FindFragment<EntityViewFragment>();

                    double x = radius * Math.Cos(theta * j + startAngle);
                    double y = radius * Math.Sin(theta * j + startAngle);
                    view.Position = new Point() { X = x, Y = y, Z = 0 };

                    group.EntityLabelPosition.Add(view.Position);

                    group.EntityLabelDirection.Add(new Vector() { X = x, Y = y, Z = 0 });

                    x = (radius - m_ProcessView.ViewConfig.Padding) * Math.Cos(theta * j + startAngle - paddingAngle);
                    y = (radius - m_ProcessView.ViewConfig.Padding) * Math.Sin(theta * j + startAngle - paddingAngle);

                    Point corner = new Point() { X = x, Y = y, Z = 0 };

                    Polyline bound = PolylineBoundary(corner, m_ProcessView.ViewConfig.EntityBoxX, m_ProcessView.ViewConfig.EntityBoxY);

                    TransformMatrix transform = Geometry.Create.RotationMatrix(corner, Vector.ZAxis, theta * j + startAngle);
                    
                    group.EntityBoundaries.Add(bound.Transform(transform));

                     
                }
                endAngle = theta * kvp.Value.Count() + startAngle;

                count++;
            }
        }
        /***************************************************/
        private static void Layout(this Stacks layout, Graph graph)
        {
            LayoutColumnsStacks(layout, graph, Vector.XAxis, m_ProcessView.ViewConfig.EntityBoxX + m_ProcessView.ViewConfig.Padding);
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                EntityGroup group = m_GraphViewFragment.EntityGroups.Find(x => x.Name == kvp.Key);
                double boundX = group.Members.Count() * (m_ProcessView.ViewConfig.EntityBoxX + m_ProcessView.ViewConfig.Padding) + m_ProcessView.ViewConfig.Padding;
                double boundY = m_ProcessView.ViewConfig.EntityBoxY + 2 * m_ProcessView.ViewConfig.Padding;

                group.Boundary = PolylineBoundary(group.LabelPosition, boundX ,boundY );
            }
        }
        /***************************************************/
        private static void Layout(this Columns layout, Graph graph)
        {
            LayoutColumnsStacks(layout, graph, Vector.YAxis, m_ProcessView.ViewConfig.EntityBoxY + m_ProcessView.ViewConfig.Padding);
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                EntityGroup group = m_GraphViewFragment.EntityGroups.Find(x => x.Name == kvp.Key);
                double boundX = m_ProcessView.ViewConfig.EntityBoxX + 2 * m_ProcessView.ViewConfig.Padding;
                double boundY = group.Members.Count() * (m_ProcessView.ViewConfig.EntityBoxY + m_ProcessView.ViewConfig.Padding) + m_ProcessView.ViewConfig.Padding;
                group.Boundary = PolylineBoundary(group.LabelPosition, boundX, boundY );
            }
        }
        /***************************************************/
        private static void Layout(this Bubbles layout, Graph graph)
        {
            
            int count = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                
                double theta = layout.SweepAngle / (kvp.Value.Count() - 1);

                for (int j = 0; j < kvp.Value.Count(); j++)
                {

                    EntityViewFragment view = graph.Entities[kvp.Value[j].BHoM_Guid].FindFragment<EntityViewFragment>();

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
            
            int count = 0;
            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                EntityGroup group = m_GraphViewFragment.EntityGroups.Find(x => x.Name == kvp.Key);
                group.LabelPosition = layout.GroupPoints[count] - 2*m_PaddingX - 2*m_PaddingY;
                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    
                    EntityViewFragment view = graph.Entities[kvp.Value[j].BHoM_Guid].FindFragment<EntityViewFragment>();

                    view.Position = layout.GroupPoints[count]  + direction * j * space ;

                    group.EntityLabelPosition.Add(view.Position );

                    group.EntityLabelDirection.Add(Vector.XAxis);

                    group.EntityBoundaries.Add(PolylineBoundary(view.Position - m_PaddingX - m_PaddingY, m_ProcessView.ViewConfig.EntityBoxX, m_ProcessView.ViewConfig.EntityBoxY));

                }

                
                count++;
            }
        }

        /***************************************************/

        private static Polyline PolylineBoundary(Point point, double x, double y)
        {
            List<Point> points = new List<Point>();
            points.Add(point);
            points.Add(point + Vector.XAxis * x);
            points.Add(point + Vector.XAxis * x + Vector.YAxis * y);
            points.Add(point + Vector.YAxis * y);
            points.Add(point);
            return Geometry.Create.Polyline(points);
        }

        /***************************************************/
        private static void FindGroups(this Graph graph, List<string> groupsToIgnore)
        {
            graph.CheckEntityViewFragments();
            m_Groups = new SortedDictionary<string, List<IBHoMObject>>();
            foreach (IBHoMObject entity in graph.Entities.Values.ToList())
            {
                EntityViewFragment viewFrag = entity.FindFragment<EntityViewFragment>();
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
        private static void SetUpGraphViewFragment(this Graph graph)
        {
            m_GraphViewFragment = new GraphViewFragment();

            foreach (KeyValuePair<string, List<IBHoMObject>> kvp in m_Groups)
            {
                EntityGroup group = new EntityGroup() { Name = kvp.Key };

                m_GraphViewFragment.EntityGroups.Add(group);

                for (int j = 0; j < kvp.Value.Count(); j++)
                {
                    group.Members.Add(kvp.Value[j].BHoM_Guid);
                    group.EntityNames.Add(kvp.Value[j].Name);
                }
            }

            graph.Fragments.Add(m_GraphViewFragment);
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
                    viewFrag.GroupNames.Add("New EntityViewFragments");
                    entity.Fragments.Add(viewFrag);

                    Reflection.Compute.RecordWarning("No EntityViewFragment found on entity :" + entity.Name + ". Entity has been assigned to \"New EntityViewFragments\" group.");

                }
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

        private static SortedDictionary<string, List<IBHoMObject>> m_Groups { get; set; }

        private static GraphViewFragment m_GraphViewFragment { get; set; }

        private static ProcessView m_ProcessView { get; set; }

        private static Vector m_PaddingX { get; set; }

        private static Vector m_PaddingY { get; set; }
    }
}