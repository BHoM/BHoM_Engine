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
using BH.Engine.Base;
using NUnit.Framework;
using AutoBogus;
using Shouldly;
using BH.oM.Structure.Elements;
using BH.oM.Test.NUnit;
using BH.oM.Structure.SectionProperties;

namespace BH.Tests.Engine.Base.Query
{
    public class Geometry : NUnitTest
    {
        private static ConcreteSection concreteSection = (ConcreteSection)BH.Engine.Base.Create.RandomObject(typeof(ConcreteSection));

        [Test]
        public static void CrossSectionGeometry3D()
        {
            var geom = BH.Engine.Structure.Query.Geometry(concreteSection);
            geom.ShouldNotBeNull();
        }

        [Test]
        public static void CrossSectionIGeometryExtensionMethodNotNull()
        {
            var geom = BH.Engine.Base.Query.IGeometry(concreteSection);
            geom.ShouldNotBeNull();
        }

        [Test]
        [Description("Calls an IGeometry extension method and another extension method. Useful to very that the matching lookup works when multiple delegates are stored.")]
        public static void MultipleGeometryExtensionMethodNotNull()
        {
            var geom = BH.Engine.Base.Query.IGeometry(concreteSection);
            geom.ShouldNotBeNull();

            Bar bar = BH.Engine.Structure.Create.Bar(new oM.Geometry.Line { Start = new oM.Geometry.Point(), End = new oM.Geometry.Point { X = 1 } }, BH.Engine.Structure.Create.SteelCircularSection(0.2), 0);
            var geom2 = BH.Engine.Base.Query.IGeometry(bar);
            geom2.ShouldNotBeNull();
        }
    }
}