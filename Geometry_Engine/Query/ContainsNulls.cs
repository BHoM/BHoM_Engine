using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    /***************************************************/
    /**** Public Methods                            ****/
    /***************************************************/
    public static partial class Query
    {
        [Description("Checks if a List of Geometry is null and if any element in the List is null and outputs relevant error message.")]
        [Input("geometries", "The List of Geometry to test for null.")]
        [Input("methodName", "The name of the method to reference in the error message.")]
        [Input("msg", "Optional message to be returned in addition to the generated error message.")]
        [Output("isNull", "True if the List of Geometry is null or it contains nulls.")]
        public static bool ContainsNulls(this IEnumerable<IGeometry> geometries, string msg = "", [CallerMemberName] string methodName = "", bool deepCheck = false)
        {
            if (geometries.IsNullOrEmpty(methodName))
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    methodName = "Method";
                }
                return true;
            }
            return geometries.Any(x => IIsNull(x, msg, methodName, deepCheck));
        }
    }
}
