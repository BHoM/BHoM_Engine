using BH.oM.Base;
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

        [Description("Create a simple DistinctConfig.")]
        [Input("numericTolerance", "Tolerance used to determine numerical differences." +
            "\nDefaults to Tolerance.Distance (1e-6).")]
        [Input("propertyNamesToConsider", "By default, all the properties of the objects are considered in determining uniqueness." +
            "\nHere you can specify a list of property names. Only the properties with a name matching any of this list will be considered." +
            "\nE.g., if you input 'Name' only the differences in terms of name will be returned.")]
        public static DistinctConfig DistinctConfig(double numericTolerance = oM.Geometry.Tolerance.Distance, List<string> propertyNamesToConsider = null)
        {
            DistinctConfig distinctConfig = new DistinctConfig()
            {
                NumericTolerance = numericTolerance,
                PropertyNamesToConsider = propertyNamesToConsider ?? new List<string>(),
            };

            return distinctConfig;
        }
    }
}
