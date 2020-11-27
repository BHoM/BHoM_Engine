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
        public static void RepresentationFragment(this Boxes component, List<IBHoMObject> data,ViewConfig viewConfig, List<IScale> scales)
        {
            IScale xScale = scales.Find(s => s.Name == "xScale");
            IScale yScale = scales.Find(s => s.Name == "yScale");
            //properties to map to x
            List<object> xObs = DataList(data.PropertyValue(component.X)).Distinct().ToList();
            List<object> yObs = DataList(data.PropertyValue(component.Y)).Distinct().ToList();
            var groups = data.GroupBy(d => d.PropertyValue(component.Group));
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
            foreach (var group in groups)
            {
                int i = 0;
                foreach (var obj in group)
                {
                    EntityRepresentation representation = new EntityRepresentation();

                    double x = 0; 
                    double y = 0; 
                    if (component.IsHorizontal)
                    {
                        x = System.Convert.ToDouble(xScale.IScale(i));
                        y = System.Convert.ToDouble(yScale.IScale(obj.PropertyValue(component.Group)));
                    }
                    else
                    {
                        x = System.Convert.ToDouble(xScale.IScale(obj.PropertyValue(component.Group)));
                        y = System.Convert.ToDouble(yScale.IScale(i));
                    }
                    Point basePt = Geometry.Create.Point(x, y, 0);
                    representation.Boundary = Box(basePt, xSpace, ySpace);
                    representation.Text = obj.Name;
                    representation.TextPosition = SetTextPosition(basePt, xSpace, ySpace, component.Padding);
                    representation.OutgoingRelationPoint = basePt;
                    representation.IncomingRelationPoint = basePt;
                    obj.Fragments.AddOrReplace(representation);
                    i++;
                }
                
            }
        }
        private static List<object> DataList(object obj)
        {
            List<object> list = new List<object>();
            if (obj is IEnumerable<object>)
            {
                var enumerator = ((IEnumerable<object>)obj).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                        list.Add(enumerator.Current);
                }
            }
            return list;
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

        private static Point SetTextPosition(Point basePt, double x, double y, double padding)
        {
            return basePt + Vector.XAxis * x * padding + Vector.YAxis * y * padding;
        }
    }
}
