using BH.Engine.Graphics.Scales;
using BH.Engine.Reflection;
using BH.oM.Base;
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
            List<object> x = DataList(data.PropertyValue(component.X));
            List<object> y = DataList(data.PropertyValue(component.Y));
            var groups = data.GroupBy(d => d.PropertyValue(component.Group));
            
            foreach (IBHoMObject obj in data)
            {
                EntityRepresentation representation = new EntityRepresentation();

                xScale.IScale(obj.PropertyValue(component.X));
                yScale.IScale(obj.PropertyValue(component.Y));
                obj.Fragments.AddOrReplace(representation);
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
    }
}
