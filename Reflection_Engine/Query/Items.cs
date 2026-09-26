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

using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extracts all BHoM type constructors to be grouped as Create Adapter items in the UI.")]
        [Output("items", "All BHoM type constructors to be grouped as Create Adapter items.")]
        public static IEnumerable<MethodBase> AdapterConstructorItems()
        {
            return Engine.Base.Query.AdapterTypeList()
                .SelectMany(x => x.GetConstructors())
                .Where(x => !x.IsNotImplemented() && !x.IsDeprecated());
        }

        /***************************************************/

        [Description("Extracts all types valid in BHoM.")]
        [Output("items", "All types valid in BHoM.")]
        public static IEnumerable<Type> TypeItems()
        {
            return Engine.Base.Query.AllTypeList()
                .Where(x => x.Namespace.StartsWith("BH."))
                .Concat(SystemTypes())
                .Where(x => !x.IsNotImplemented() && !x.IsDeprecated());
        }

        /***************************************************/

        [Description("Extracts all types that have a valid public constructor.")]
        [Output("items", "All types that have a valid public constructor.")]
        public static IEnumerable<Type> ConstructableTypeItems()
        {
            return Engine.Base.Query.BHoMTypeList()
                .Where(x => x != null && !x.IsNotImplemented() && !x.IsDeprecated() && x.IsAutoConstructorAllowed() && !x.IsEnum && !x.IsAbstract)
                .Where(x => x.GetConstructors().Where(c => c.GetParameters().Count() > 0).Count() == 0);
        }

        /***************************************************/

        [Description("Extracts all enum types valid in BHoM.")]
        [Output("items", "All enum types valid in BHoM.")]
        public static IEnumerable<Type> EnumItems()
        {
            return Engine.Base.Query.BHoMEnumList()
                .Where(x => !x.IsNotImplemented() && !x.IsDeprecated());
        }

        /***************************************************/

        //[Description("Extracts names of all BHoM library items.")]
        //[Output("items", "Names of all BHoM library items.")]
        //public static List<string> LibraryItems()
        //{
        //    string datasetFolder = BH.Engine.Base.Query.BHoMFolderDatasets();
        //    string separator = Path.DirectorySeparatorChar.ToString();

        //    return Directory.GetFiles(datasetFolder, "*.json", SearchOption.AllDirectories)
        //        .Select(x => x.Replace(datasetFolder + separator, "").Replace(".json", ""))
        //        .ToList();
        //}

        ///***************************************************/

        [Description("Extracts all external methods in BHoM.")]
        [Output("items", "All external methods in BHoM.")]
        public static List<MethodBase> ExternalItems()
        {
            return Engine.Base.Query.ExternalMethodList();
        }

        /***************************************************/
    }
}
