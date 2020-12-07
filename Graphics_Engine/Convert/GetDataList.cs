using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Convert
    {
        [Description("Attempts to convert an object to a collection of objects.")]
        [Input("object", "The object to convert.")]
        [Output("dataList", "A collection of objects, empty if the conversion was not possible.")]
        public static List<object> ToDataList(object obj)
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
