using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Data.Collections;
using BH.oM.Graphics.Scales;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Graphics.Scales
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Given a value in the input domain, returns the corresponding value in the output range of a scale.")]
        [Input("scale", "The scale to query.")]
        [Input("value", "A value in the input domain.")]
        [Output("output", "The value from output range.")]
        public static object IScale(this IScale scale, object value)
        {
            return Scale(scale as dynamic, value);
        }

        /***************************************************/

        [Description("Given a value in the input domain, returns the corresponding value in the output range of a scale.")]
        [Input("scale", "The scale to query.")]
        [Input("value", "A value in the input domain.")]
        [Output("output", "The value from output range.")]
        public static object Scale(this ScaleLinear scale, object value)
        {
            return Map(scale.Domain, scale.Range, System.Convert.ToDouble(value));
        }

        /***************************************************/

        [Description("Given a value in the input domain, returns the corresponding value in the output range of a scale.")]
        [Input("scale", "The scale to query.")]
        [Input("value", "A value in the input domain.")]
        [Output("output", "The value from output range.")]
        public static object Scale(this ScaleOrdinal scale, object value)
        {
            int i = scale.Domain.IndexOf(value.ToString());
            if (i == -1)
                return null;
            if (scale.Range.All(r => r.IsNumericType()) && scale.Range.Count == 2)
            {
                Domain d = new Domain(0, scale.Domain.Count);
                Domain r = new Domain(System.Convert.ToDouble(scale.Range[0]), System.Convert.ToDouble(scale.Range[1]));
                return Map(d, r, i);
            }
                
            else
            {
                if (i > scale.Range.Count -1)
                    i = i % (scale.Range.Count);
                return scale.Range[i];
            }
                
        }

        /***************************************************/
        /**** Fallback Methods                          ****/
        /***************************************************/

        [Description("Given a value in the input domain, returns the corresponding value in the output range of a scale.")]
        [Input("scale", "The scale to query.")]
        [Input("value", "A value in the input domain.")]
        [Output("output", "The value from output range.")]
        public static object Scale(this IScale scale, object value)
        {
            return null;
        }

        /***************************************************/
        /**** Private Methods                            ****/
        /***************************************************/

        private static double Map(Domain domain, Domain range, double value)
        {
            double domainSpan = domain.Max - domain.Min;
            double rangeSpan = range.Max - range.Min;
            double mapped = value / domainSpan * rangeSpan + range.Min;
            return mapped;
        }
    }
}
