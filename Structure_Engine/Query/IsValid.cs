/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Base.Attributes.Enums;
using BH.oM.Structure;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        [Description("Checks if a RetainignWall is valid by verifying that the Stem does not intersect the footing.")]
        [Input("retainingWall", "The RetainingWall to check.")]
        [Output("result", "Returns true if the RetainingWall is valid.")]
        public static bool IsValid(this RetainingWall retainingWall)
        {
            return IsValid(retainingWall.Stem, retainingWall.Footing);
        }

        /***************************************************/

        [Description("Checks if a Stem and PadFoundation are valid by performing null checks and a basic check that the stem does not go into the footing.")]
        [Input("stem", "The RetainingWall to check.")]
        [Input("footing", "The footing to check.")]
        [Output("result", "Returns true if the Stem and PadFoundation are valid.")]
        public static bool IsValid(this Stem stem, PadFoundation footing)
        {
            if (footing.IsNull() || stem.IsNull())
                return false;

            if (footing.Perimeter.IControlPoints().OrderBy(p => p.Z).First().Z - stem.Perimeter.IControlPoints().OrderBy(p => p.Z).First().Z > Tolerance.MicroDistance)
            {
                Base.Compute.RecordError("The footings highest control point is above the lowest control point of the stem. The two objects should not intersect.");
                return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Checks if a case, a Loadcase or a LoadCombination, is valid by performing null checks and basic checks on its properties. \n" +
            "For a LoadCombination, also checks the cases and factors it holds, validating each nested case, and that it does not contain itself.")]
        [Input("loadCase", "The Loadcase or LoadCombination to check.")]
        [Input("message", "Optional text added to the end of every error recorded, for example to say which LoadCombination the case is factored in.", UIExposure.Hidden)]
        [Output("result", "Returns true if the case is valid.")]
        public static bool IsValid(this ICase loadCase, string message = "")
        {
            if (loadCase.IsNull(nameof(IsValid), message))
                return false;

            if (string.IsNullOrWhiteSpace(loadCase.Name))
            {
                Base.Compute.RecordError(WithMessage($"{loadCase.GetType().Name} {loadCase.Number} has no name.", message));
                return false;
            }

            if (loadCase.Number <= 0)
            {
                Base.Compute.RecordError(WithMessage($"{loadCase.GetType().Name} '{loadCase.Name}' has the number {loadCase.Number}, which is less than or equal to zero.", message));
                return false;
            }

            if (loadCase is LoadCombination combination)
                return HasValidFactors(combination, message);

            return true;
        }

        /***************************************************/

        [Description("Checks a set of cases, Loadcases and/or LoadCombinations, is valid: each case is valid on its own, and no number or name is used twice.")]
        [Input("loadCases", "The Loadcases and LoadCombinations to check.")]
        [Output("result", "Returns true if every case is valid and no number or name is repeated.")]
        public static bool IsValid(this IEnumerable<ICase> loadCases)
        {
            if (loadCases == null)
            {
                Base.Compute.RecordError("The cases to check are null.");
                return false;
            }

            List<ICase> cases = loadCases.ToList();
            bool valid = true;

            for (int i = 0; i < cases.Count; i++)
            {
                if (cases[i] == null)
                {
                    Base.Compute.RecordError($"The case at position {i} of the list is null. The list indexing is 0-based, meaning the first item is at position 0.");
                    valid = false;
                    continue;
                }

                valid &= cases[i].IsValid();    // &= so every case is checked and every problem recorded
            }

            List<ICase> present = cases.Where(loadCase => loadCase != null).ToList();

            foreach (IGrouping<int, ICase> group in present.GroupBy(loadCase => loadCase.Number).Where(group => group.Count() > 1))
            {
                Base.Compute.RecordError($"The number {group.Key} is used by more than one case: {string.Join(", ", group.Select(loadCase => $"'{loadCase.Name}'"))}.");
                valid = false;
            }

            IEnumerable<IGrouping<string, ICase>> sameName = present
                .Where(loadCase => !string.IsNullOrWhiteSpace(loadCase.Name))
                .GroupBy(loadCase => loadCase.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1);
            foreach (IGrouping<string, ICase> group in sameName)
            {
                Base.Compute.RecordError($"The name '{group.Key}' is used by more than one case: numbers {string.Join(", ", group.Select(loadCase => loadCase.Number))}.");
                valid = false;
            }

            return valid;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Checks the cases and factors a combination holds, validating each nested case in turn. The message is added
        // to every error recorded, and passed on to the nested cases with this combination added to it.
        private static bool HasValidFactors(LoadCombination combination, string message)
        {
            // Checked first, as the nested cases are validated below, and a combination that contains itself would never stop doing so.
            if (ContainsItself(combination))
            {
                Base.Compute.RecordError(WithMessage($"LoadCombination {combination.Number} contains itself, directly or through the combinations it contains.", message));
                return false;
            }

            if (combination.LoadCases == null || combination.LoadCases.Count == 0)
            {
                Base.Compute.RecordError(WithMessage($"LoadCombination {combination.Number} must contain at least one case.", message));
                return false;
            }

            bool valid = true;
            string nestedMessage = WithMessage($"It is factored in LoadCombination {combination.Number}.", message);

            foreach (Tuple<double, ICase> item in combination.LoadCases)
            {
                valid &= (item?.Item2).IsValid(nestedMessage);    // &= so every case is checked and every problem recorded

                if (item != null && (double.IsNaN(item.Item1) || double.IsInfinity(item.Item1)))
                {
                    string caseName = item.Item2 == null ? "a null case" : $"case {item.Item2.Number}";
                    Base.Compute.RecordError(WithMessage($"LoadCombination {combination.Number} has the factor {item.Item1} on {caseName}, which is not a finite number.", message));
                    valid = false;
                }
            }

            // Packages identify cases by number, so a number repeated is the same case factored twice.
            IEnumerable<IGrouping<int, ICase>> repeated = combination.LoadCases
                .Where(item => item?.Item2 != null)
                .Select(item => item.Item2)
                .GroupBy(loadCase => loadCase.Number)
                .Where(group => group.Count() > 1);
            foreach (IGrouping<int, ICase> group in repeated)
            {
                Base.Compute.RecordError(WithMessage($"LoadCombination {combination.Number} contains case {group.Key} more than once.", message));
                valid = false;
            }

            return valid;
        }

        /***************************************************/

        // Adds the optional message to the end of an error.
        private static string WithMessage(string error, string message)
        {
            return string.IsNullOrWhiteSpace(message) ? error : $"{error} {message}";
        }

        /***************************************************/
    }
}

