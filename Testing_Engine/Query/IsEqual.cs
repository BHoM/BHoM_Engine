using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Reflection;
using BH.oM.Base;
using BH.oM.Testing;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

using KellermanSoftware.CompareNetObjects;

namespace BH.Engine.Testing
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks two BHoMObjects for equality and returns the differences")]
        [Input("config", "Config to be used for the comparison. Can set numeric tolerance, wheter to check the guid, if custom data should be ignored and if any additional properties should be ignored")]
        [MultiOutputAttribute(0, "IsEqual", "Returns true if the two items are deemed to be equal")]
        [MultiOutputAttribute(1, "DiffProperty", "List of the names of the properties found to be different")]
        [MultiOutputAttribute(2, "Obj1DiffValue", "List of the values deemed different for object 1")]
        [MultiOutputAttribute(3, "Obj2DiffValue", "List of the values deemed different for object 2")]
        public static Output<bool, List<string>, List<string>, List<string>> IsEqual(this IBHoMObject obj1, IBHoMObject obj2, IsEqualConfig config = null)
        {
            //Use default config if null
            config = config ?? new IsEqualConfig();

            CompareLogic comparer = new CompareLogic();

            comparer.Config.MaxDifferences = 1000;

            comparer.Config.MembersToIgnore = config.PropertiesToIgnore;

            if (config.IgnoreCustomData)
                comparer.Config.MembersToIgnore.Add("CustomData");

            if (config.IgnoreGuid)
                comparer.Config.TypesToIgnore.Add(typeof(Guid));

            comparer.Config.DoublePrecision = config.NumericTolerance;


            ComparisonResult result = comparer.Compare(obj1, obj2);

            List<string> propsDifferent = result.Differences.Select(x => x.PropertyName).ToList();
            List<string> obj1DiffValues = result.Differences.Select(x => x.Object1Value).ToList();
            List<string> obj2DiffValues = result.Differences.Select(x => x.Object2Value).ToList();

            return new Output<bool, List<string>, List<string>, List<string>>
            {
                Item1 = result.AreEqual,
                Item2 = propsDifferent,
                Item3 = obj1DiffValues,
                Item4 = obj2DiffValues
            };

        }

        /***************************************************/
    }
}
