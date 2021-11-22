/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Diffing;
using BH.oM.Base;
using kellerman = KellermanSoftware.CompareNetObjects;
using System.Reflection;
using BH.Engine.Base;
using BH.Engine.Reflection;

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Given two numbers, checks if the ")]
        public static bool NumericalDifferenceInclusion(this object object1, object object2, string propertyFullName, BaseComparisonConfig cc = null)
        {
            return NumericalDifferenceInclusion(object1, object2, propertyFullName, cc.NumericTolerance, cc.SignificantFigures, cc.PropertyNumericTolerances, cc.PropertySignificantFigures);
        }

        /***************************************************/

        public static bool NumericalDifferenceInclusion(this object object1, object object2, string propertyFullName,
            double globalNumericTolerance = double.MinValue,
            int globalSignificantFigures = int.MaxValue,
            HashSet<NamedNumericTolerance> customNumericTolerances = null,
            HashSet<NamedSignificantFigures> customSignificantFigures = null)
        {
            // Check if we specified CustomTolerances and if this difference is a number difference.
            if (globalNumericTolerance != double.MinValue || globalSignificantFigures != int.MaxValue
                || (customNumericTolerances?.Any() ?? false) || (customSignificantFigures?.Any() ?? false)
                && object1.GetType().IsNumeric() && object2.GetType().IsNumeric()) // GetType() is slow; call only after checking that any custom tolerance is present.
            {
                // We have specified some custom tolerance in the ComparisonConfig AND this property difference is numeric.
                // Because we have set Kellerman to retrieve any possible numerical variation,
                // we now want to "filter out" number variations following our BHoM ComparisonConfig settings.
                if (globalNumericTolerance!= double.MinValue || (customNumericTolerances?.Any() ?? false))
                {
                    double toleranceToUse = BH.Engine.Base.Query.NumericTolerance(customNumericTolerances, globalNumericTolerance, propertyFullName);
                    if (toleranceToUse != double.MinValue)
                    {
                        double value1 = double.Parse(object1.ToString()).RoundWithTolerance(toleranceToUse);
                        double value2 = double.Parse(object2.ToString()).RoundWithTolerance(toleranceToUse);

                        // If, once rounded, the numbers are the same, it means that we do not want to consider this Difference. Skip.
                        if (value1 == value2)
                            return false;
                    }
                }

                if (globalSignificantFigures != int.MaxValue || (customSignificantFigures?.Any() ?? false))
                {
                    int significantFiguresToUse = BH.Engine.Base.Query.SignificantFigures(customSignificantFigures, globalSignificantFigures, propertyFullName);
                    if (significantFiguresToUse != int.MaxValue)
                    {
                        double value1 = double.Parse(object1.ToString()).RoundToSignificantFigures(significantFiguresToUse);
                        double value2 = double.Parse(object2.ToString()).RoundToSignificantFigures(significantFiguresToUse);

                        // If, once rounded, the numbers are the same, it means that we do not want to consider this Difference. Skip.
                        if (value1 == value2)
                            return false;
                    }
                }
            }

            return true;
        }
    }
}


