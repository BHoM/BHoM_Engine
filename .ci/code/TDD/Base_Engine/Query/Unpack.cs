/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using AutoBogus;
using NUnit.Framework;
using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections;

namespace BH.Engine.Base
{
    public partial class Query
    {
        [SetUp]
        public void Setup()
        {
            // BH.Engine.Base.Create.RandomObject() can't deal with generics. Using AutoFaker instead. Example:
            // var test = new AutoFaker<Container<int>>().Generate();
        }

        private static IEnumerable<TestCaseData> GetTestContainers()
        {
            // AutoFaker creates 3 objects of the requested type per each IEnumerable property.
            // E.g. Container<Bar> will have 1 + 3 + 3 * 3 = 13 Bars.  
            yield return new TestCaseData(new AutoFaker<Container<Bar>>().Generate(), 13);
            yield return new TestCaseData(new AutoFaker<DictionaryContainer<Bar>>().Generate(), 16);
            yield return new TestCaseData(new AutoFaker<DictionaryListContainer<Bar>>().Generate(), 22);
            yield return new TestCaseData(new AutoFaker<ListOfDictionariesContainer<Bar>>().Generate(), 13);
            yield return new TestCaseData(new AutoFaker<ListOfListOfListContainer<Bar>>().Generate(), 13);
        }

        [Test, TestCaseSource(nameof(GetTestContainers))]
        public void Unpack<T>(Container<T> container, int numberOfObjects)
        {
            var result = container.Unpack();

            Assert.That(result.OfType<T>().Count(), Is.EqualTo(numberOfObjects));
        }

        [Test]
        public void Unpack_DiscardCustomData()
        {
            var validContainer = new AutoFaker<Container<Bar>>().Generate();
            validContainer.CustomData["bar"] = new AutoFaker<Bar>().Generate();

            var result = validContainer.Unpack();

            Assert.That(result.OfType<Bar>().Count(), Is.EqualTo(13));
        }

        [Test]
        public void Unpack_DiscardFragments()
        {
            var validContainer = new AutoFaker<Container<Bar>>().Generate();
            validContainer.Fragments.Add(new TestFragment() { SomeObject = new Bar() });

            var result = validContainer.Unpack();

            Assert.That(result.OfType<Bar>().Count(), Is.EqualTo(13));
        }
    }
}