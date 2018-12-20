/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsLegal(this MethodInfo method)
        {
            try
            {
                method.GetParameters();
                return method.ReturnType != null;   //void is not null
            }
            catch
            {
                return false;
            }
        }


        /***************************************************/

        public static bool IsLegal(this Type type) //TODO: Check if there is a better way to do this, instead of using a try-catch
        {
            try
            {
                //Checking that all the constructors have loaded parameter types
                type.GetConstructors().SelectMany(x => x.GetParameters()).ToList(); //ToList() there to execute the linq query
            }
            catch
            {
                return false;
            }
            return true;
        }


        /***************************************************/



    }
}
