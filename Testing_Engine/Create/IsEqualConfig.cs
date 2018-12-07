using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Testing;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Testing
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Config to be used to controll the IsEqual method.")]
        [Input("numericTolerance", "Tolerance used when compare doubles. Default value set to 1e-6")]
        [Input("ignoreGuid", "Toggles wheter to check the BHoM_Guid when comparing the objects.  Defaults to true => ignoring")]
        [Input("ignoreCustomData", "Toggels whether the custom data shouls be compared. Defaults to true => ignoring")]
        [Input("propertiesToIgnore", "names of any additional proerties to be ignored in the comparison. Case sensitive!")]
        public static IsEqualConfig IsEqualConfig(double numericTolerance = 1e-6, bool ignoreGuid = true, bool ignoreCustomData = true, List<string> propertiesToIgnore = null)
        {
            propertiesToIgnore = propertiesToIgnore ?? new List<string>();
            return new IsEqualConfig { NumericTolerance = numericTolerance, IgnoreGuid = ignoreGuid, IgnoreCustomData = ignoreCustomData, PropertiesToIgnore = propertiesToIgnore };
        }

        /***************************************************/
    }
}
