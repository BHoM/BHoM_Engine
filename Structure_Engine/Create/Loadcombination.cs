using BH.oM.Structural.Loads;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static LoadCombination LoadCombination(string name, int number, IEnumerable<Tuple<double, ICase>> factoredCases)
        {
            return new LoadCombination { LoadCases = factoredCases.ToList(), Name = name, Number = number };
        }

        /***************************************************/

        public static LoadCombination LoadCombination(string name, int number, List<Loadcase> cases, List<double> factors, bool excludeZeroFactorCases = true)
        {
            if (cases.Count != factors.Count)
                throw new ArgumentException("Loadcombinations require the same number of cases and factors");

            List<Tuple<double, ICase>> factoredCases = new List<Tuple<double, ICase>>();

            for (int i = 0; i < cases.Count; i++)
            {
                if (!excludeZeroFactorCases || factors[i] > 0)
                    factoredCases.Add(new Tuple<double, ICase>(factors[i], cases[i]));
            }

            return LoadCombination(name, number, factoredCases);
        }

        /***************************************************/
    }
}
