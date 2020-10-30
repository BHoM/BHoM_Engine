using BH.Engine.Geometry;
using BH.Engine.Reflection;
using BH.Engine.Spatial;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Physical.FramingProperties;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Physical
{
    public static partial class Query
    {

        [Description("Returns the top centreline of an IFramingElement")]
        [Input("BHoM.Physical.Elements.IFramingElement", "The IFramingEment to query the top centreline of")]
        [Output("ICurve", "The top centreline of the IFramingElement")]

        public static ICurve TopCentreline(this BH.oM.Physical.Elements.IFramingElement element)
        {
            ICurve location = element.Location;

            Vector normal;
            
            if(location.IIsLinear())
            { 
                normal = BH.Engine.Physical.Query.Normal(element);
            }
            else
            {
                Engine.Reflection.Compute.RecordError("IFramingElement must have linear location line");
                normal = null;
            }

            double height = 0;
           
            object propValue = element.PropertyValue("Property.Profile.Height");

            if (propValue is IConvertible)
            {
                height = ((IConvertible)propValue).ToDouble(null);

                ICurve topCentreline = location.ITranslate(normal * 0.5 * height);

                return topCentreline;
            }
            else
            {
                Engine.Reflection.Compute.RecordError("Failed extracting height");

                return null;
            }
            
         

            

        }

    }
    
}
