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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsLegal(this MethodInfo method)
        {
            return method.IsLegal(out string reason);
        }


        /***************************************************/

        public static bool IsLegal(this Type type) 
        {
            return type.IsLegal(out string reason);
        }


        /***************************************************/
        /**** Internal Methods                          ****/
        /***************************************************/

        internal static bool IsLegal(this MethodInfo method, out string reason)
        {
            reason = null;
            if (method == null)
                return false;

            try
            {
                method.GetParameters();
                return method.ReturnType != null;   //void is not null
            }
            catch (Exception e)
            {
                reason = e.Message;
                return false;
            }
        }

        /***************************************************/

        internal static bool IsLegal(this Type type, out string reason) //TODO: Check if there is a better way to do this, instead of using a try-catch
        {
            reason = null;
            if (type == null)
                return false;

            try
            {
                //Checking that all the constructors have loaded parameter types
                type.GetConstructors().SelectMany(x => x.GetParameters()).ToList(); //ToList() there to execute the linq query
            }
            catch (Exception e)
            {
                reason = e.Message;
                return false;
            }
            return true;
        }


        /***************************************************/



    }
}







