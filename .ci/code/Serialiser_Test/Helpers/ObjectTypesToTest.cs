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

using BH.Engine.Reflection;
using BH.Engine.Serialiser;
using BH.Engine.Test;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Test.Serialiser
{
    public static partial class Helpers
    {
        /*************************************/
        /**** Public Methods              ****/
        /*************************************/

        public static List<Type> ObjectTypesToTest()
        {
            Engine.Base.Compute.LoadAllAssemblies();

            // It feels like the BHoMTypeList method should already return a clean list of Type but it doesn't at the moment
            return Engine.Base.Query.BHoMTypeList().Where(x => {
                return typeof(IObject).IsAssignableFrom(x)
                  && !x.IsAbstract
                  && !x.IsDeprecated()
                  && !x.GetProperties().Select(p => p.PropertyType.Namespace).Any(n => !n.StartsWith("BH.") && !n.StartsWith("System"));
            }).ToList();
        }

        /*************************************/
    }
}


