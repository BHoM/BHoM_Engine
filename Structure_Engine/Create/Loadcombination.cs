using BH.oM.Structural.Loads;
using System;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LoadCombination LoadCombination(string name, List<Tuple<double, ICase>> loadCases)
        {
            return new LoadCombination { LoadCases = loadCases, Name = name };
        }

        /***************************************************/
    }
}
