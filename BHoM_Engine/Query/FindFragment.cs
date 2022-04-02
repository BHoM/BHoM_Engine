/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

        [Description("Returns an instance of a BHoM Fragment if it exists on the object")]
        [Input("iBHoMObject", "A generic IBHoMObject object")]
        [Input("fragmentType", "The type of fragment to be queried and returned. If not specified, the generic type is taken.")]
        [Output("fragment", "The instance of that Fragment if it exists on the object, null otherwise")]
        public static T FindFragment<T>(this IBHoMObject iBHoMObject, Type fragmentType = null)
        {
            if (fragmentType == null)
                fragmentType = typeof(T);

            IFragment fragment;
            iBHoMObject.Fragments.TryGetValue(fragmentType, out fragment);

            return (T)System.Convert.ChangeType(fragment, fragmentType);
        }

        /***************************************************/

    }
}


