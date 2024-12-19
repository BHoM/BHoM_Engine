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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.Engine.Reflection;

namespace BH.Engine.Library
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<IBHoMObject> PartialMatch(string libraryName, string objectName, bool removeWhiteSpace = true, bool ignoreCase = true)
        {
            objectName = removeWhiteSpace ? objectName.Replace(" ", "") : objectName;
            objectName = ignoreCase ? objectName.ToLower() : objectName;

            Func<IBHoMObject, bool> conditionalMatch = delegate (IBHoMObject x)
            {
                string name = x.Name;
                name = removeWhiteSpace ? name.Replace(" ", "") : name;
                name = ignoreCase ? name.ToLower() : name;
                return name.Contains(objectName);
            };

            return Library(libraryName).Where(conditionalMatch).ToList();
        }
    }
}






