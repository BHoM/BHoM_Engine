using BH.Engine.Graphics.Scales;
using BH.Engine.Reflection;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Graphics.Components;
using BH.oM.Graphics.Fragments;
using BH.oM.Graphics.Scales;
using BH.oM.Graphics.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Create
    {
        public static void RepresentationFragment(this Boxes component, List<IBHoMObject> data, ViewConfig viewConfig, List<IScale> scales)
        {
            GetScales(scales);

            BHoMGroup<IBHoMObject> entityGroup = (BHoMGroup<IBHoMObject>)data.Find(x => x.Name == "Entities");
            List<IBHoMObject> entities = entityGroup.Elements;

            var groups = entities.GroupBy(d => d.PropertyValue(component.Group));
            var groupNames = groups.Select(g => g.Key).Cast<string>().ToList();
            int maxGroup = groups.Max(g => g.Count());

            double xSpace = 0; 
            double ySpace = 0;
            
            if (component.IsHorizontal)
            {
                xSpace = (viewConfig.Width / maxGroup) * (1 - component.Padding);
                ySpace = (viewConfig.Height / groupNames.Count) * (1 - component.Padding);
            }
            else
            {
                xSpace = (viewConfig.Width / groupNames.Count) * (1 - component.Padding);
                ySpace = (viewConfig.Height / maxGroup) * (1 - component.Padding);
            }
            List<GroupRepresentation> groupRepresentations = new List<GroupRepresentation>();
            GraphRepresentation graphRepresentation = new GraphRepresentation();
            foreach (var group in groups)
            {
                int i = 0;
                var orderedgroup = group.OrderBy(g => g.PropertyValue(component.GroupOrder));
                double x = 0;
                double y = 0;
                GroupRepresentation representation = new GroupRepresentation();
                if (component.IsHorizontal)
                {
                    x = System.Convert.ToDouble(m_Xscale.IScale(0));
                    y = System.Convert.ToDouble(m_Yscale.IScale(group.Key));
                    representation.Boundary = Box(Geometry.Create.Point(x, y, 0), xSpace * orderedgroup.Count(), ySpace );
                }
                else
                {
                    x = System.Convert.ToDouble(m_Xscale.IScale(group.Key));
                    y = System.Convert.ToDouble(m_Yscale.IScale(0));
                    representation.Boundary = Box(Geometry.Create.Point(x, y, 0), xSpace, ySpace * orderedgroup.Count());
                }

                groupRepresentations.Add(representation);
                foreach (var obj in orderedgroup)
                {
                    obj.SetEntityRepresentation(i, component, xSpace, ySpace);
                    i++;
                }
                
            }
            graphRepresentation.Groups = groupRepresentations;
            
        }

        private static void SetEntityRepresentation(this IBHoMObject obj,int seqNumber, Boxes component, double xSpace, double ySpace)
        {
            double x = 0;
            double y = 0;
            if (component.IsHorizontal)
            {
                x = System.Convert.ToDouble(m_Xscale.IScale(seqNumber));
                y = System.Convert.ToDouble(m_Yscale.IScale(obj.PropertyValue(component.Group)));
            }
            else
            {
                x = System.Convert.ToDouble(m_Xscale.IScale(obj.PropertyValue(component.Group)));
                y = System.Convert.ToDouble(m_Yscale.IScale(seqNumber));
            }

            EntityRepresentation representation = new EntityRepresentation();

            Point basePt = SetAnchorPoint(Geometry.Create.Point(x, y, 0), xSpace, ySpace, component.Padding);

            representation.Boundary = Box(basePt, xSpace, ySpace);
            representation.Text = obj.Name;
            representation.TextPosition = SetAnchorPoint(basePt, xSpace, ySpace, component.Padding);
            representation.OutgoingRelationPoint = SetAnchorPoint(basePt, xSpace, ySpace / 2, 1);
            representation.IncomingRelationPoint = SetAnchorPoint(basePt, 0, ySpace / 2, 1);
            representation.Colour = Query.ColourFromObject(m_Colourscale.IScale(obj.PropertyValue(component.Colour)));
            obj.Fragments.AddOrReplace(representation);
        }

        private static Polyline Box(Point basePt, double x, double y)
        {
            List<Point> points = new List<Point>();
            points.Add(basePt);
            points.Add(basePt + Vector.XAxis * x);
            points.Add(basePt + Vector.XAxis * x + Vector.YAxis * y);
            points.Add(basePt + Vector.YAxis * y);
            points.Add(basePt);
            return Geometry.Create.Polyline(points);
        }

        private static Point SetAnchorPoint(Point basePt, double x, double y, double padding)
        {
            return basePt + Vector.XAxis * x * padding + Vector.YAxis * y * padding;
        }
        private static void GetScales(List<IScale> scales)
        {
            m_Xscale = scales.Find(s => s.Name == "xScale");
            m_Yscale = scales.Find(s => s.Name == "yScale");
            m_Colourscale = scales.Find(s => s.Name == "colourScale");
        }

        private static IScale m_Xscale = null;
        private static IScale m_Yscale = null;
        private static IScale m_Colourscale = null;
    }
}
