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
using System.Collections.Generic;
using BH.oM.Structure.Loads;
using BH.oM.Test.NUnit;
using NUnit.Framework;
using Shouldly;

namespace BH.Tests.Engine.Structure.Query
{
    public class ContainsItself : NUnitTest
    {
        [Test]
        [Description("Only Loadcases returns false.")]
        public void NoNestedCombinations()
        {
            LoadCombination combination = Combination(1, Case(1));

            BH.Engine.Structure.Query.ContainsItself(combination).ShouldBeFalse();
        }

        [Test]
        [Description("Direct self-reference returns true.")]
        public void DirectSelfReference()
        {
            LoadCombination combination = Combination(1, Case(1));
            combination.LoadCases.Add(new Tuple<double, ICase>(1.0, combination));

            BH.Engine.Structure.Query.ContainsItself(combination).ShouldBeTrue();
        }

        [Test]
        [Description("A > B > A returns true for A and B.")]
        public void IndirectSelfReference()
        {
            LoadCombination a = Combination(1, Case(1));
            LoadCombination b = Combination(2, a);
            a.LoadCases.Add(new Tuple<double, ICase>(1.0, b));

            BH.Engine.Structure.Query.ContainsItself(a).ShouldBeTrue();
            BH.Engine.Structure.Query.ContainsItself(b).ShouldBeTrue();
        }

        [Test]
        [Description("A > B > C > B returns false for A and true for B.")]
        public void LowerLoopOnlyFoundFromInsideIt()
        {
            LoadCombination c = Combination(3, Case(1));
            LoadCombination b = Combination(2, c);
            c.LoadCases.Add(new Tuple<double, ICase>(1.0, b));
            LoadCombination a = Combination(1, b);

            BH.Engine.Structure.Query.ContainsItself(a).ShouldBeFalse();
            BH.Engine.Structure.Query.ContainsItself(b).ShouldBeTrue();
        }

        [Test]
        [Description("A copy with the same Number returns true.")]
        public void CopyWithSameNumber()
        {
            LoadCombination copy = Combination(1, Case(1));
            LoadCombination combination = Combination(1, copy);

            BH.Engine.Structure.Query.ContainsItself(combination).ShouldBeTrue();
        }

        [Test]
        [Description("Unnumbered combinations are compared by reference.")]
        public void UnnumberedComparedByReference()
        {
            LoadCombination inner = Combination(0, Case(1));
            LoadCombination outer = Combination(0, inner);

            BH.Engine.Structure.Query.ContainsItself(outer).ShouldBeFalse();

            outer.LoadCases.Add(new Tuple<double, ICase>(1.0, outer));
            BH.Engine.Structure.Query.ContainsItself(outer).ShouldBeTrue();
        }

        [Test]
        [Description("Null returns false and records an error.")]
        public void NullCombination()
        {
            BH.Engine.Structure.Query.ContainsItself(null).ShouldBeFalse();
            BH.Engine.Base.Query.CurrentEvents().ShouldContain(e => e.Type == BH.oM.Base.Debugging.EventType.Error);
        }

        [Test]
        [Description("IsValid returns false for A when A > B > C > B.")]
        public void IsValidFindsLowerLoop()
        {
            LoadCombination c = Combination(3, Case(1));
            LoadCombination b = Combination(2, c);
            c.LoadCases.Add(new Tuple<double, ICase>(1.0, b));
            LoadCombination a = Combination(1, b);

            BH.Engine.Structure.Query.IsValid(a).ShouldBeFalse();
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Loadcase Case(int number)
        {
            return new Loadcase
            {
                Name = $"Case {number}",
                Number = number
            };
        }

        /***************************************************/

        private static LoadCombination Combination(int number, ICase loadCase)
        {
            return new LoadCombination
            {
                Name = $"Combination {number}",
                Number = number,
                LoadCases = new List<Tuple<double, ICase>> { new Tuple<double, ICase>(1.0, loadCase) }
            };
        }

        /***************************************************/
    }
}

