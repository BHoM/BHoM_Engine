using BH.oM.Diffing;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Create
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Create a simple DiffConfig.")]
        [Input("numericTolerance", "Tolerance used to determine numerical differences." +
            "\nDefaults to Tolerance.Distance (1e-6).")]
        [Input("propertiesToConsider", "By default, diffing considers all the properties of the objects." +
            "\nHere you can specify a list of property names. Only the properties with a name matching any of this list will be considered for diffing." +
            "\nE.g., if you input 'Name' only the differences in terms of name will be returned." +
            "\nNOTE: these can be only top-level properties of the object (not the sub-properties).")]
        [Output("diffConfig", "DiffConfig.")]
        public static DiffConfig DiffConfig(double numericTolerance = oM.Geometry.Tolerance.Distance, List<string> propertiesToConsider = null)
        {
            DiffConfig diffConfig = new DiffConfig()
            {
                NumericTolerance = numericTolerance,
                PropertiesToConsider = propertiesToConsider ?? new List<string>(),
            };

            return diffConfig;
        }
    }
}
