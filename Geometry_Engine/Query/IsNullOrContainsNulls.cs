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
        public static bool IsNullOrContainsNulls(this IEnumerable<IGeometry> geometries, string msg = "", [CallerMemberName] string methodName = "")
        {
            //check the collection
            if (geometries == null)
            {
                if (string.IsNullOrEmpty(methodName))
                {
                    methodName = "Method";
                }
                Reflection.Compute.RecordError($"Cannot evaluate {methodName} because the Geometry failed a null check. {msg} The collection is null.");

                return true;
            }
            //check the geometry contained in the collection
            return geometries.Any(x => IIsNull(x as dynamic, msg + " One or more of elements the in the collection failed a null test.", methodName));
        }
    }
}
