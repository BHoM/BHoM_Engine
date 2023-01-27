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
using System.ComponentModel;

namespace BH.Engine.Base
{
    public partial class Query
    {
        static Query()
        {
            // The static ctor is called only once at the test class initialisation.

            AutoFaker.Configure(builder =>
            {
                builder
                  .WithRepeatCount(1);        // Configures the number of items in a collection.
            });
        }

        [SetUp]
        public void SetUp()
        {
            // This method is called once per Test execution (after the TestCaseSource attribute resolution).
        }

        private static IEnumerable<TestCaseData> GetTestContainers()
        {
            // BH.Engine.Base.Create.RandomObject() can't deal with generics. Using AutoFaker instead. Example:
            // AutoFaker creates 1 objects of the requested type per each IEnumerable property.
            // E.g. Container<Bar> will have 1 + 1 + 1 = 3 Bars.  
            yield return new TestCaseData(new AutoFaker<Container<Bar>>().Generate(), 3);
            yield return new TestCaseData(new AutoFaker<DictionaryContainer<Bar>>().Generate(), 4);
            yield return new TestCaseData(new AutoFaker<DictionaryListContainer<Bar>>().Generate(), 4);
            yield return new TestCaseData(new AutoFaker<ListOfDictionariesContainer<Bar>>().Generate(), 3);
            yield return new TestCaseData(new AutoFaker<ListOfListOfListContainer<Bar>>().Generate(), 3);
        }

        private static IEnumerable<TestCaseData> GetTestContainerOfContainers()
        {
            // Container of containers.
            // AutoFaker creates 1 objects of the requested type per each IEnumerable property.
            // E.g. Container<Container<Bar>> will have 3 + 3 + 3 = 9 Bars.  
            yield return new TestCaseData(new AutoFaker<Container<Container<Bar>>>().Generate(), 9);
            yield return new TestCaseData(new AutoFaker<DictionaryContainer<Container<Bar>>>().Generate(), 12);
            yield return new TestCaseData(new AutoFaker<DictionaryListContainer<Container<Bar>>>().Generate(), 12);
            yield return new TestCaseData(new AutoFaker<ListOfDictionariesContainer<Container<Bar>>>().Generate(), 9);
            yield return new TestCaseData(new AutoFaker<ListOfListOfListContainer<Container<Bar>>>().Generate(), 9);
        }

        [Test]
        [TestCaseSource(nameof(GetTestContainers))]
        public void Unpack<T>(Container<T> container, int numberOfObjects)
        {
            var result = container.Unpack();

            Assert.That(result.OfType<T>().Count(), Is.EqualTo(numberOfObjects));
        }

        [Test]
        [TestCaseSource(nameof(GetTestContainerOfContainers))]
        public void UnpackContainerOfContainers<T>(Container<T> container, int numberOfObjects)
        {
            var result = container.Unpack();

            Assert.That(result.OfType<Bar>().Count(), Is.EqualTo(numberOfObjects));
        }

        [Test]
        public void Unpack_DisregardCustomData()
        {
            var validContainer = new AutoFaker<Container<Bar>>().Generate();
            validContainer.CustomData["bar"] = new AutoFaker<Bar>().Generate();

            var result = validContainer.Unpack();

            Assert.That(result.OfType<Bar>().Count(), Is.EqualTo(3));
        }

        [Test]
        public void Unpack_DisregardFragments()
        {
            var validContainer = new AutoFaker<Container<Bar>>().Generate();
            validContainer.Fragments.Add(new TestFragment() { SomeObject = new Bar() });

            var result = validContainer.Unpack();

            Assert.That(result.OfType<Bar>().Count(), Is.EqualTo(3));
        }
    }
}