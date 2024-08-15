///*
// * This file is part of the Buildings and Habitats object Model (BHoM)
// * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
// *
// * Each contributor holds copyright over their respective contributions.
// * The project versioning (Git) records all such contribution source information.
// *                                           
// *                                                                              
// * The BHoM is free software: you can redistribute it and/or modify         
// * it under the terms of the GNU Lesser General Public License as published by  
// * the Free Software Foundation, either version 3.0 of the License, or          
// * (at your option) any later version.                                          
// *                                                                              
// * The BHoM is distributed in the hope that it will be useful,              
// * but WITHOUT ANY WARRANTY; without even the implied warranty of               
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
// * GNU Lesser General Public License for more details.                          
// *                                                                            
// * You should have received a copy of the GNU Lesser General Public License     
// * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
// */

using BH.oM.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// COMMENTED OUT BECAUSE THE IGNORE ATTRIBUTE IS NOT FUNCTIONING AS INTENDED FOR CI
//namespace BH.Tests.Engine.Serialiser
//{
//    [Ignore("Ignoring NUnit versioning tests as no 100% clear action for every single class to be IObject. Usefull to be able to get this information out, but not required to be run automatically. Test kept for debugging purposes.")]
//    public class AllTypesIObject : BaseLoader
//    {
//        [Test]
//        public static void CheckAllTypesIObject()
//        {
//            List<Type> allTypes = BH.Engine.Base.Query.AllTypeList().Where(x => x.Namespace.StartsWith("BH.oM") || x.Namespace.StartsWith("BH.Revit.oM")).Where(x => !x.IsEnum).ToList();

//            List<Type> nonIObjects = allTypes.Where(x => !typeof(IObject).IsAssignableFrom(x)).ToList();

//            List<string> interfaceTypes = nonIObjects.Where(x => x.IsInterface).Select(x => x.FullName).Distinct().ToList();
//            List<string> classTypes = nonIObjects.Where(x => !x.IsInterface).Select(x => x.FullName).Distinct().ToList();

//            Console.WriteLine("Failing interface types:");
//            foreach (string type in interfaceTypes) 
//            {
//                Console.WriteLine(type);
//            }

//            Console.WriteLine();
//            Console.WriteLine("Failing class types:");
//            foreach (string type in classTypes)
//            {
//                Console.WriteLine(type);
//            }
//        }
//    }
//}

