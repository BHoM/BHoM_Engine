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
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Check if the input fragment can be added to an object of the input type.")]
        [Input("fragment", "The fragment object you want to add to an object.")]
        [Input("targetType", "The type of object you want to apply the fragment to.")]
        public static bool CanTarget(this IFragment fragment, Type targetType)
        {
            if (fragment == null)
                return false;

            List <TargetsAttribute> attributes = fragment.GetType().GetCustomAttributes(typeof(TargetsAttribute), false).OfType<TargetsAttribute>().ToList();
            if (attributes.Count == 0)
                return typeof(IBHoMObject).IsAssignableFrom(targetType);
            else
                return attributes.SelectMany(x => x.ValidTypes).Distinct().Any(x => x.IsAssignableFrom(targetType));
        }

        /***************************************************/

        [Description("Check if the input fragment can be added to a specific object.")]
        [Input("fragment", "The fragment object you want to add to that object.")]
        [Input("target", "The object you want to apply the fragment to.")]
        public static bool CanTarget(this IFragment fragment, object target)
        {
            if (target == null)
                return false;
            else
                return CanTarget(fragment, target.GetType());
        }

        /***************************************************/
    }
}




