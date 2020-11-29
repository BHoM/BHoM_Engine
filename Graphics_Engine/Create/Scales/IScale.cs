using BH.oM.Data.Collections;
using BH.oM.Graphics.Scales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Create 
    {
        public static IScale IScale(List<object> domain, List<object> range)
        {
            IScale scale = null;

            if (domain.All(d => d is string))
            {
                List<string> d = domain.Cast<string>().ToList().Distinct().ToList();

                scale = new ScaleOrdinal()
                {
                    Domain = d,
                    Range = range
                };
            }

            if (domain.All(d => d.IsNumericType()) && range.All(d => d.IsNumericType()))
            {
                List<double> d = new List<double>();
                domain.ForEach(x => d.Add(System.Convert.ToDouble(x)));
                List<double> r = new List<double>();
                range.ForEach(x => r.Add(System.Convert.ToDouble(x)));

                scale = new ScaleLinear()
                {
                    Domain = Data.Create.Domain(d),
                    Range = Data.Create.Domain(r)
                };
            }
            //date time
            //non linear
            //others to do
            return scale;
        }

        
        
    }
}
