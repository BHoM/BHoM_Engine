using System;

using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Structure.Properties;
using BH.Engine.Geometry;
using BH.Engine.Reflection;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Edge Edge(ICurve curve, Constraint4DOF constraint = null, string name = "")
        {
            return new Edge
            {
                Curve = curve,
                Constraint = constraint,
                Name = name
            };
        }

        /***************************************************/

    }
}