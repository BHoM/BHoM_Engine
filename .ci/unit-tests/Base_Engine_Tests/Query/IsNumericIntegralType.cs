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
using BH.Engine.Base;
using BH.Tests.Engine.Base.Query.Objects;
using NUnit.Framework;
using AutoBogus;
using Shouldly;
using BH.oM.Structure.Constraints;

namespace BH.Tests.Engine.Base.Query
{
    public class IsNumericIntegralTypeTests
    {
        [Test]
        public void AreEnumsIntegral()
        {
            BH.Engine.Base.Query.IsNumericIntegralType(typeof(DOFType), false).ShouldBe(false);
            BH.Engine.Base.Query.IsNumericIntegralType(typeof(DOFType), true).ShouldBe(true);

            BH.Engine.Base.Query.IsNumeric(typeof(DOFType)).ShouldBe(true, "By default, IsNumericIntegralType() considers enums as a numeric integral type.");
        }

        [Test]
        public void AreIntsIntegral()
        {
            BH.Engine.Base.Query.IsNumeric(10.GetType(), true).ShouldBe(true);
            BH.Engine.Base.Query.IsNumeric(10.GetType(), false).ShouldBe(true);

            BH.Engine.Base.Query.IsNumeric(10.GetType()).ShouldBe(true);
        }
    }
}

