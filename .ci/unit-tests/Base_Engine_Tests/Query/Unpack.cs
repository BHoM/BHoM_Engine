/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using BH.oM.Test.NUnit;
using BH.Engine.Base;
using BH.Tests.Engine.Base.Query.Objects;
using NUnit.Framework;
using AutoBogus;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace BH.Tests.Engine.Base.Query
{
    public class UnpackTests : NUnitTest
    {
        /// <summary>
        /// The static constructor is called only once at the test class initialisation, before everything else.
        /// Some 'OneTimeSetup' configurations have to be done here, e.g. those dealing with Attributes like `TestCaseSource`.
        /// </summary>
        static UnpackTests()
        {
            // BH.Engine.Base.Create.RandomObject() cannot deal with generics and cannot fix the number of items in generated collections.
            // Using AutoFaker instead.

            // Configures the number of items in any collection generated by AutoFaker.
            AutoFaker.Configure(builder =>
            {
                builder.WithRepeatCount(1);
            });
        }

        [TearDown]
        [Description("Tear down method handling collection and reporting of BHoM events recorded during the tests.")]
        public void TearDown()
        {
            var events = BH.Engine.Base.Query.CurrentEvents();
            if (events.Any())
            {
                foreach (var ev in events)
                {
                    if (ev.Type == oM.Base.Debugging.EventType.Warning || ev.Type == oM.Base.Debugging.EventType.Error)
                        Assert.Warn($"{ev.Type}: {ev.Message}");
                    else
                        TestContext.Out.Write($"{ev.Type}: {ev.Message}");
                }
            }

            BH.Engine.Base.Compute.ClearCurrentEvents();
        }

        private static IEnumerable<TestCaseData> GetTestContainers()
        {
            // The verification is done against the number of unpacked objects expected, which can be determined once AutoFaker's `WithRepeatCount` is configured.
            // Example:
            // AutoFaker creates 1 objects of the requested type per each IEnumerable property.
            // It follows that Container<Bar> will have 1 + 1 + 1 = 3 Bars.  
            yield return new TestCaseData(new AutoFaker<Container<BHoMObject>>().Generate(), 3);
            yield return new TestCaseData(new AutoFaker<DictionaryContainer<BHoMObject>>().Generate(), 4);
            yield return new TestCaseData(new AutoFaker<DictionaryListContainer<BHoMObject>>().Generate(), 4);
            yield return new TestCaseData(new AutoFaker<ListOfDictionariesContainer<BHoMObject>>().Generate(), 3);
            yield return new TestCaseData(new AutoFaker<ListOfListOfListContainer<BHoMObject>>().Generate(), 3);
        }

        private static IEnumerable<TestCaseData> GetTestContainerOfContainers()
        {
            // Container of containers.
            // The verification is done against the number of unpacked objects expected, which can be determined once AutoFaker's `WithRepeatCount` is configured.
            // Example:
            // AutoFaker creates 1 objects of the requested type per each IEnumerable property.
            // It follows that Container<Container<Bar>> will have 3 + 3 + 3 = 9 Bars.  
            yield return new TestCaseData(new AutoFaker<Container<Container<BHoMObject>>>().Generate(), 9);
            yield return new TestCaseData(new AutoFaker<DictionaryContainer<Container<BHoMObject>>>().Generate(), 12);
            yield return new TestCaseData(new AutoFaker<DictionaryListContainer<Container<BHoMObject>>>().Generate(), 12);
            yield return new TestCaseData(new AutoFaker<ListOfDictionariesContainer<Container<BHoMObject>>>().Generate(), 9);
            yield return new TestCaseData(new AutoFaker<ListOfListOfListContainer<Container<BHoMObject>>>().Generate(), 9);
        }

        [Test]
        [TestCaseSource(nameof(GetTestContainers))]
        [Description("Tests that general cases for the unpack is functioning as intended against the data defined in the GetTestContainers().")]
        public void Unpack<T>(Container<T> container, int numberOfObjects)
        {
            var result = container.Unpack();

            result.OfType<T>().Count().ShouldBeEquivalentTo(numberOfObjects);
        }

        [Test]
        [TestCaseSource(nameof(GetTestContainerOfContainers))]
        [Description("Tests that general cases for the unpack is functioning as intended for containers containing other container against the data defined in the GetTestContainerOfContainers().")]
        public void UnpackContainerOfContainers<T>(Container<T> container, int numberOfObjects)
        {
            var result = container.Unpack();

            result.OfType<BHoMObject>().Count().ShouldBeEquivalentTo(numberOfObjects);
        }

        [Test]
        [Description("Test method that checks that CustomData is ignored when unpacking a container.")]
        public void Unpack_DisregardCustomData()
        {
            var validContainer = new AutoFaker<Container<BHoMObject>>().Generate();
            validContainer.CustomData["bar"] = new AutoFaker<BHoMObject>().Generate();

            var result = validContainer.Unpack();

            result.OfType<BHoMObject>().Count().ShouldBeEquivalentTo(3);
        }

        [Test]
        [Description("Test method that checks that Fragments are ignored when unpacking a container.")]
        public void Unpack_DisregardFragments()
        {
            var validContainer = new AutoFaker<Container<BHoMObject>>().Generate();
            validContainer.Fragments.Add(new TestFragment() { SomeObject = new BHoMObject() });

            var result = validContainer.Unpack();

            result.OfType<BHoMObject>().Count().ShouldBeEquivalentTo(3);
        }
    }
}

