using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data.Collections;
using BH.oM.Graphics.Scales;

namespace BH.Engine.Graphics.Scales
{
    public static partial class Query
    {
        public static object IScale(this IScale scale, object value)
        {
            return Scale(scale as dynamic, value);
        }
        public static object Scale(this ScaleLinear scale, object value)
        {
            return Map(scale.Domain, scale.Range, (double)value);
        }
        public static object Scale(this ScaleOrdinal scale, object value)
        {
            int i = scale.Domain.IndexOf((string)value);
            if (i == -1)
                return null;
            if (scale.Range.All(r => r.IsNumericType()) && scale.Range.Count == 2)
            {
                Domain d = new Domain(0, scale.Domain.Count - 1);
                Domain r = new Domain(System.Convert.ToDouble(scale.Range[0]), System.Convert.ToDouble(scale.Range[1]));
                return Map(d, r, i * 1.0 / (scale.Domain.Count - 1));
            }
                
            else
                return scale.Range[i];
        }
        public static object Scale(this IScale scale, object value)
        {
            return null;
        }
        private static double Map(Domain domain, Domain range, double value)
        {
            double domainSpan = domain.Max - domain.Min;
            double rangeSpan = range.Max - range.Min;
            double mapped = value / domainSpan * rangeSpan + range.Min;
            return mapped;
        }
    }
}
